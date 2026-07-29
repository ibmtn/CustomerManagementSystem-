using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiSayacService : ISayacService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiSayacService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };

        }

        public async Task<List<Sayac>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Sayac>>("/api/Sayaclar", _jsonOptions);
                return result ?? new List<Sayac>();
            }
            catch
            {
                return new List<Sayac>();
            }
        }

        public async Task<PagedResponse<Sayac>> GetPagedAsync(
            int page, 
            int pageSize, 
            string? seriNo, 
            int? durum)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrEmpty(seriNo)) queryParams.Add($"seriNo={Uri.EscapeDataString(seriNo)}");
                if (durum.HasValue) queryParams.Add($"durum={durum.Value}");

                string url = $"/api/Sayaclar/Paged?{string.Join("&", queryParams)}";
                
                var result = await _httpClient.GetFromJsonAsync<PagedResponse<Sayac>>(url, _jsonOptions);
                return result ?? new PagedResponse<Sayac>();
            }
            catch
            {
                return new PagedResponse<Sayac>();
            }
        }

        public async Task<Sayac?> GetByIdAsync(long id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Sayac>($"/api/Sayaclar/{id}", _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task CreateAsync(Sayac sayac)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Sayaclar", sayac, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Sayaç oluşturulamadı.");
            }
        }

        public async Task UpdateAsync(Sayac sayac)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Sayaclar/{sayac.sayac_id}", sayac, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Sayaç güncellenemedi.");
            }
        }

        public async Task DeleteAsync(long id)
        {
            await _httpClient.DeleteAsync($"/api/Sayaclar/{id}");
        }
    }
}

