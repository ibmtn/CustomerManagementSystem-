using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

using Microsoft.Extensions.Caching.Memory;

namespace KcetasWeb.Services.Api
{
    public class ApiSayacService : ISayacService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IMemoryCache _cache;

        public ApiSayacService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Sayac>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync("Sayac_GetAll", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                try
                {
                    var result = await _httpClient.GetFromJsonAsync<List<Sayac>>("/api/Sayaclar", _jsonOptions);
                    return result ?? new List<Sayac>();
                }
                catch
                {
                    return new List<Sayac>();
                }
            }) ?? new List<Sayac>();
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
            var dto = new
            {
                seriNo = sayac.seri_no,
                tuketimNoktasiId = sayac.tuketim_noktasi_id.HasValue && sayac.tuketim_noktasi_id.Value > 0 ? sayac.tuketim_noktasi_id : null,
                marka = string.IsNullOrWhiteSpace(sayac.marka) ? null : sayac.marka.Trim(),
                model = string.IsNullOrWhiteSpace(sayac.model) ? null : sayac.model.Trim(),
                uretimYili = sayac.uretim_yili <= 0 ? DateTime.Now.Year : sayac.uretim_yili,
                faz = sayac.faz ?? KcetasWeb.Models.Enums.SayacFaz.Monofaze,
                carpan = sayac.carpan <= 0 ? 1.0m : sayac.carpan,
                muhurNo = string.IsNullOrWhiteSpace(sayac.muhur_no) ? null : sayac.muhur_no.Trim(),
                durum = sayac.durum ?? KcetasWeb.Models.Enums.SayacDurumu.Depoda,
                status = string.IsNullOrWhiteSpace(sayac.status) ? "DEPODA" : sayac.status,
                createdBy = sayac.created_by ?? 1
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Sayaclar", dto, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var payload = JsonSerializer.Serialize(dto, _jsonOptions);
                throw new Exception($"API Hatası: {response.StatusCode} - Sayaç oluşturulamadı. Payload: {payload}. Detay: {errorContent}");
            }

            var created = await response.Content.ReadFromJsonAsync<Sayac>(_jsonOptions);
            if (created != null)
            {
                sayac.sayac_id = created.sayac_id;
                sayac.seri_no = created.seri_no;
            }

            _cache.Remove("Sayac_GetAll");
        }

        public async Task UpdateAsync(Sayac sayac)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Sayaclar/{sayac.sayac_id}", sayac, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Sayaç güncellenemedi.");
            }
            _cache.Remove("Sayac_GetAll");
        }

        public async Task DeleteAsync(long id)
        {
            await _httpClient.DeleteAsync($"/api/Sayaclar/{id}");
            _cache.Remove("Sayac_GetAll");
        }
    }
}
