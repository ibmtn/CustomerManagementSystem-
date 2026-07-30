using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace KcetasWeb.Services.Api
{
    public class ApiAboneService : IAboneService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;

        public ApiAboneService(HttpClient httpClient, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }



                public async System.Threading.Tasks.Task<PaginatedResponse<Abone>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<PaginatedResponse<Abone>>($"/api/Aboneler/arama?page={page}&pageSize={pageSize}", _jsonOptions);
                return response ?? new PaginatedResponse<Abone> { CurrentPage = page, PageSize = pageSize };
            }
            catch
            {
                return new PaginatedResponse<Abone> { CurrentPage = page, PageSize = pageSize };
            }
        }

        public async System.Threading.Tasks.Task<KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.AboneListDto>> GetPagedCursorAsync(long? lastId, int limit)
        {
            try
            {
                var qs = lastId.HasValue ? $"?lastId={lastId}&limit={limit}" : $"?limit={limit}";
                var response = await _httpClient.GetFromJsonAsync<KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.AboneListDto>>($"/api/Aboneler/Cursor{qs}", _jsonOptions);
                return response ?? new KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.AboneListDto> { PageSize = limit };
            }
            catch
            {
                return new KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.AboneListDto> { PageSize = limit };
            }
        }

        public async System.Threading.Tasks.Task<List<Abone>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync("Abone_GetAll_2", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                var list = new List<Abone>();
                try
                {
                    // Fetch first page to get totalCount
                    var response = await _httpClient.GetAsync("/api/Aboneler/arama?page=1&pageSize=100");
                    if (!response.IsSuccessStatusCode) return list;
                    
                    var jsonStr = await response.Content.ReadAsStringAsync();
                    using var doc = System.Text.Json.JsonDocument.Parse(jsonStr);
                    
                    int totalPages = 1;
                    if (doc.RootElement.TryGetProperty("totalCount", out var tc) && tc.ValueKind != System.Text.Json.JsonValueKind.Null)
                    {
                        if (tc.TryGetInt32(out int totalCount) && totalCount > 0)
                        {
                            totalPages = (int)Math.Ceiling((double)totalCount / 100.0);
                        }
                    }
                    
                    if (doc.RootElement.TryGetProperty("data", out var dataProp))
                    {
                        var items = System.Text.Json.JsonSerializer.Deserialize<List<Abone>>(dataProp.GetRawText(), _jsonOptions);
                        if (items != null) list.AddRange(items);
                    }
                    
                    if (totalPages > 1)
                    {
                        var tasks = new List<Task<List<Abone>>>();
                        using var semaphore = new SemaphoreSlim(15); // Limit concurrent requests to 15
                        
                        for (int i = 2; i <= totalPages; i++)
                        {
                            int currentPage = i;
                            tasks.Add(Task.Run(async () =>
                            {
                                await semaphore.WaitAsync();
                                try
                                {
                                    var pageResp = await _httpClient.GetAsync($"/api/Aboneler/arama?page={currentPage}&pageSize=100");
                                    if (pageResp.IsSuccessStatusCode)
                                    {
                                        var pJson = await pageResp.Content.ReadAsStringAsync();
                                        using var pDoc = System.Text.Json.JsonDocument.Parse(pJson);
                                        if (pDoc.RootElement.TryGetProperty("data", out var pData))
                                        {
                                            var pItems = System.Text.Json.JsonSerializer.Deserialize<List<Abone>>(pData.GetRawText(), _jsonOptions);
                                            if (pItems != null) return pItems;
                                        }
                                    }
                                }
                                catch { }
                                finally
                                {
                                    semaphore.Release();
                                }
                                return new List<Abone>();
                            }));
                        }
                        
                        var results = await Task.WhenAll(tasks);
                        foreach(var r in results)
                        {
                            list.AddRange(r);
                        }
                    }
                    
                    // Order list by abone_id just in case concurrent fetch messed up the order
                    return list.OrderBy(x => x.abone_id).ToList();
                }
                catch
                {
                    return list;
                }
            });
        }

        public async System.Threading.Tasks.Task<Abone?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Abone>($"/api/Aboneler/{id}", _jsonOptions);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async System.Threading.Tasks.Task CreateAsync(Abone abone)
        {
            var dto = new
            {
                aboneTipi = abone.abone_tipi ?? KcetasWeb.Models.Enums.AboneTipi.Bireysel,
                ad = NullIfWhiteSpace(abone.Ad),
                soyad = NullIfWhiteSpace(abone.Soyad),
                unvan = NullIfWhiteSpace(abone.Unvan),
                tckn = NullIfWhiteSpace(abone.tckn),
                vkn = NullIfWhiteSpace(abone.vkn),
                telefon = NullIfWhiteSpace(abone.telefon)
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Aboneler", dto, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var payload = System.Text.Json.JsonSerializer.Serialize(abone, _jsonOptions);
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Abone oluşturulamadı. \nPayload: {payload}\nDetay: {err}");
            }
            // Clear cache so new item appears immediately in searches
            _cache.Remove("Abone_TotalCount");
            _cache.Remove("Abone_GetAll");
            _cache.Remove("Abone_GetAll_2");
        }

        public async System.Threading.Tasks.Task UpdateAsync(Abone abone)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Aboneler/{abone.abone_id}", abone, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Abone güncellenemedi. Detay: {err}");
            }

            _cache.Remove("Abone_GetAll_2");
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Aboneler/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Abone silinemedi. Detay: {err}");
            }

            _cache.Remove("Abone_GetAll_2");
        }

        private static string NullIfWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}


