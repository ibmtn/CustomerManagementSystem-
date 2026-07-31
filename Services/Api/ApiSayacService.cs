using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            int? durum,
            long? tuketimNoktasiId = null,
            string? tuketimNoktasi = null,
            string? faz = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrWhiteSpace(seriNo)) queryParams.Add($"seriNo={Uri.EscapeDataString(seriNo.Trim())}");
                if (durum.HasValue) queryParams.Add($"durum={durum.Value}");
                if (tuketimNoktasiId.HasValue) queryParams.Add($"tuketimNoktasiId={tuketimNoktasiId.Value}");
                if (!string.IsNullOrWhiteSpace(tuketimNoktasi)) queryParams.Add($"tuketimNoktasi={Uri.EscapeDataString(tuketimNoktasi.Trim())}");
                if (!string.IsNullOrWhiteSpace(faz)) queryParams.Add($"faz={Uri.EscapeDataString(faz.Trim())}");

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

        public async Task<Sayac?> GetByTuketimNoktasiIdAsync(long tuketimNoktasiId)
        {
            try
            {
                var sayaclar = await GetPagedAsync(1, 10, null, null, tuketimNoktasiId);
                var eslesenSayac = sayaclar.Data.FirstOrDefault(s => s.durum == KcetasWeb.Models.Enums.SayacDurumu.Bagli || s.durum == KcetasWeb.Models.Enums.SayacDurumu.Takili)
                    ?? sayaclar.Data.FirstOrDefault();

                if (eslesenSayac != null)
                {
                    return eslesenSayac;
                }

                var detaylar = await GetTuketimNoktasiDetaylariAsync();
                var detay = detaylar.FirstOrDefault(x => x.TuketimNoktasiId == tuketimNoktasiId);
                return !string.IsNullOrWhiteSpace(detay?.AktifSayacSeriNo)
                    ? (await GetPagedAsync(1, 10, detay.AktifSayacSeriNo, null)).Data.FirstOrDefault()
                    : null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<List<TuketimNoktasiDetayDto>> GetTuketimNoktasiDetaylariAsync()
        {
            return await _cache.GetOrCreateAsync("TuketimNoktasi_GetWithDetails_ForSayac", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                try
                {
                    var result = await _httpClient.GetFromJsonAsync<List<TuketimNoktasiDetayDto>>("/api/TuketimNoktasi/GetWithDetails", _jsonOptions);
                    return result ?? new List<TuketimNoktasiDetayDto>();
                }
                catch
                {
                    return new List<TuketimNoktasiDetayDto>();
                }
            }) ?? new List<TuketimNoktasiDetayDto>();
        }

        private class TuketimNoktasiDetayDto
        {
            [JsonPropertyName("tuketimNoktasiId")]
            public long TuketimNoktasiId { get; set; }

            [JsonPropertyName("aktifSayacSeriNo")]
            public string? AktifSayacSeriNo { get; set; }
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
