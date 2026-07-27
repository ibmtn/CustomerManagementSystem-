using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiAboneService : IAboneService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiAboneService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                var allData = await GetAllAsync();
                var count = allData.Count;
                var pagedData = allData.OrderByDescending(x => x.abone_id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PaginatedResponse<Abone>
                {
                    Data = pagedData,
                    TotalCount = count,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize)
                };
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
            try
            {
                var list = new List<Abone>();
                int currentPage = 1;
                int totalPages = 1;

                do
                {
                    try
                    {
                        var response = await _httpClient.GetAsync($"/api/Aboneler?page={currentPage}&pageSize=100");
                        if (!response.IsSuccessStatusCode) break;
                        var jsonStr = await response.Content.ReadAsStringAsync();
                        using var doc = System.Text.Json.JsonDocument.Parse(jsonStr);

                        if (doc.RootElement.TryGetProperty("totalPages", out var tp) && tp.ValueKind == System.Text.Json.JsonValueKind.Number)
                        {
                            totalPages = tp.GetInt32();
                            // Maksimum 50 sayfa (50 x 100 = 5000 kayit)
                            if (totalPages > 50) totalPages = 50;
                        }

                        if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            var items = System.Text.Json.JsonSerializer.Deserialize<List<Abone>>(jsonStr, _jsonOptions);
                            if (items != null) list.AddRange(items);
                        }
                        else if (doc.RootElement.TryGetProperty("data", out var dataProp))
                        {
                            var items = System.Text.Json.JsonSerializer.Deserialize<List<Abone>>(dataProp.GetRawText(), _jsonOptions);
                            if (items != null) list.AddRange(items);
                        }
                    }
                    catch { break; }
                    currentPage++;
                } while (currentPage <= totalPages);

                return list;
            }
            catch (Exception)
            {
                return new List<Abone>();
            }
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
            var response = await _httpClient.PostAsJsonAsync("/api/Aboneler", abone, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Abone oluşturulamadı. Detay: {err}");
            }
        }

        public async System.Threading.Tasks.Task UpdateAsync(Abone abone)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Aboneler/{abone.abone_id}", abone, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Abone güncellenemedi. Detay: {err}");
            }
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Aboneler/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Abone silinemedi.");
            }
        }
    }
}

