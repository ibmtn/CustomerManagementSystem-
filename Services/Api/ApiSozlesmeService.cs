using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiSozlesmeService : ISozlesmeService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiSozlesmeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                var jsonStr = await _httpClient.GetStringAsync("/api/Sozlesmeler?page=1&pageSize=1");
                using var doc = JsonDocument.Parse(jsonStr);
                
                if (doc.RootElement.ValueKind == JsonValueKind.Array) return doc.RootElement.GetArrayLength();
                if (doc.RootElement.TryGetProperty("totalCount", out var tc)) return tc.GetInt32();
                if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) return data.GetArrayLength();
                
                return 0;
            }
            catch { return 0; }
        }

        public async Task<PaginatedResponse<Sozlesme>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                var allData = await GetAllAsync();
                var count = allData.Count;
                var pagedData = allData.OrderByDescending(x => x.sozlesme_id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PaginatedResponse<Sozlesme>
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
                return new PaginatedResponse<Sozlesme> { CurrentPage = page, PageSize = pageSize };
            }
        }

        public async Task<List<Sozlesme>> GetAllAsync()
        {
            try
            {
                var jsonElement = await _httpClient.GetFromJsonAsync<JsonElement>("/api/Sozlesmeler?page=1&pageSize=2000", _jsonOptions);
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
        }

        public async Task<Sozlesme?> GetByIdAsync(string sozlesmeNo)
        {
            try
            {
                var all = await GetAllAsync();
                return all.FirstOrDefault(x => x.sozlesme_no == sozlesmeNo);
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
        }

        public async Task UpdateAsync(Sozlesme sozlesme)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Sozlesmeler/{sozlesme.sozlesme_id}", sozlesme, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Sözleşme güncellenemedi. Detay: {errorContent}");
            }
        }

        public async Task DeleteAsync(string sozlesmeNo)
        {
            await _httpClient.DeleteAsync($"/api/Sozlesmeler/{sozlesmeNo}");
        }
    }
}
