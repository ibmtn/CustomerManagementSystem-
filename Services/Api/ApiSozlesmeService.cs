using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

using Microsoft.Extensions.Caching.Memory;

namespace KcetasWeb.Services.Api
{
    public class ApiSozlesmeService : ISozlesmeService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IMemoryCache _cache;

        public ApiSozlesmeService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _cache.GetOrCreateAsync("Sozlesme_TotalCount", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                try
                {
                    var response = await GetPagedAsync(1, 1);
                    return response.TotalCount;
                }
                catch { return 0; }
            });
        }

        public async Task<PaginatedResponse<Sozlesme>> GetPagedAsync(int page, int pageSize, string? q = null, string? durum = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrWhiteSpace(q)) queryParams.Add($"q={Uri.EscapeDataString(q.Trim())}");
                if (!string.IsNullOrWhiteSpace(durum)) queryParams.Add($"durum={Uri.EscapeDataString(durum.Trim())}");

                var url = $"/api/Sozlesmeler/Paged?{string.Join("&", queryParams)}";
                var result = await _httpClient.GetFromJsonAsync<PaginatedResponse<Sozlesme>>(url, _jsonOptions);
                return result ?? new PaginatedResponse<Sozlesme> { CurrentPage = page, PageSize = pageSize };
            }
            catch
            {
                var allData = await GetAllAsync();
                if (!string.IsNullOrWhiteSpace(q))
                {
                    var search = q.Trim();
                    allData = allData.Where(s =>
                        (s.sozlesme_no?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (s.tekil_kod?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        s.sozlesme_id.ToString().Contains(search) ||
                        s.tuketim_noktasi_id.ToString().Contains(search)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(durum) &&
                    Enum.TryParse<KcetasWeb.Models.Enums.SozlesmeDurumu>(durum, ignoreCase: true, out var parsedDurum))
                {
                    allData = allData.Where(s => s.durum == parsedDurum).ToList();
                }

                var count = allData.Count;
                var pagedData = allData.OrderByDescending(x => x.sozlesme_id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PaginatedResponse<Sozlesme>
                {
                    Data = pagedData,
                    TotalCount = count,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
        }

        public async Task<List<Sozlesme>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync("Sozlesme_GetAll", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                try
                {
                    var jsonElement = await _httpClient.GetFromJsonAsync<JsonElement>("/api/Sozlesmeler/Paged?page=1&pageSize=50000", _jsonOptions);
                    if (jsonElement.ValueKind == JsonValueKind.Array)
                    {
                        var result = jsonElement.Deserialize<List<Sozlesme>>(_jsonOptions);
                        return result ?? new List<Sozlesme>();
                    }
                    else if (jsonElement.TryGetProperty("data", out var dataProp))
                    {
                        var result = dataProp.Deserialize<List<Sozlesme>>(_jsonOptions);
                        return result ?? new List<Sozlesme>();
                    }
                    return new List<Sozlesme>();
                }
                catch
                {
                    return new List<Sozlesme>();
                }
            }) ?? new List<Sozlesme>();
        }

        public async Task<Sozlesme?> GetByIdAsync(string sozlesmeNo)
        {
            try
            {
                var response = await GetPagedAsync(1, 5, sozlesmeNo);
                return response.Data.FirstOrDefault(x => x.sozlesme_no == sozlesmeNo)
                    ?? response.Data.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task<Sozlesme?> GetByIdAsync(long id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Sozlesme>($"/api/Sozlesmeler/{id}", _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task CreateAsync(Sozlesme sozlesme)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Sozlesmeler", sozlesme, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Sözleşme oluşturulamadı. Detay: {errorContent}");
            }
            _cache.Remove("Sozlesme_GetAll"); _cache.Remove("Sozlesme_TotalCount");
        }

        public async Task UpdateAsync(Sozlesme sozlesme)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Sozlesmeler/{sozlesme.sozlesme_id}", sozlesme, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Sözleşme güncellenemedi. Detay: {errorContent}");
            }
            _cache.Remove("Sozlesme_GetAll"); _cache.Remove("Sozlesme_TotalCount");
        }

        public async Task DeleteAsync(string sozlesmeNo)
        {
            await _httpClient.DeleteAsync($"/api/Sozlesmeler/{sozlesmeNo}");
            _cache.Remove("Sozlesme_GetAll"); _cache.Remove("Sozlesme_TotalCount");
        }
    }
}
