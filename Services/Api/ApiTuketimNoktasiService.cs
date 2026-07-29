using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace KcetasWeb.Services.Api
{
    public class ApiTuketimNoktasiService : ITuketimNoktasiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IMemoryCache _cache;

        public ApiTuketimNoktasiService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _cache.GetOrCreateAsync("TuketimNoktasi_TotalCount", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                try
                {
                    var jsonStr = await _httpClient.GetStringAsync("/api/TuketimNoktasi?page=1&pageSize=1");
                    using var doc = JsonDocument.Parse(jsonStr);
                    
                    if (doc.RootElement.ValueKind == JsonValueKind.Array) return doc.RootElement.GetArrayLength();
                    if (doc.RootElement.TryGetProperty("totalCount", out var tc)) return tc.GetInt32();
                    if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) return data.GetArrayLength();
                    
                    return 0;
                }
                catch { return 0; }
            });
        }

        public async Task<List<TuketimNoktasi>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync("TuketimNoktasi_GetAll", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                try
                {
                    var jsonStr = await _httpClient.GetStringAsync("/api/TuketimNoktasi?page=1&pageSize=1000");
                    using var doc = JsonDocument.Parse(jsonStr);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        return JsonSerializer.Deserialize<List<TuketimNoktasi>>(jsonStr, _jsonOptions) ?? new List<TuketimNoktasi>();
                    }
                    else if (doc.RootElement.TryGetProperty("data", out var dataProp))
                    {
                        return JsonSerializer.Deserialize<List<TuketimNoktasi>>(dataProp.GetRawText(), _jsonOptions) ?? new List<TuketimNoktasi>();
                    }
                    return new List<TuketimNoktasi>();
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText("tuketim_err.txt", ex.ToString());
                    return new List<TuketimNoktasi>();
                }
            });
        }

        public async Task<TuketimNoktasi?> GetByIdAsync(string tekilKod)
        {
            try
            {
                var all = await GetAllAsync();
                return all.FirstOrDefault(x => x.tekil_kod == tekilKod);
            }
            catch
            {
                return null;
            }
        }

        public async Task CreateAsync(TuketimNoktasi tuketimNoktasi)
        {
            var dto = new
            {
                ilceId = tuketimNoktasi.ilce_id,
                mahalle = tuketimNoktasi.mahalle ?? "Bilinmiyor",
                binaNo = tuketimNoktasi.bina_no,
                bagimsizBolumNo = tuketimNoktasi.bagimsiz_bolum_no,
                acikAdres = tuketimNoktasi.acik_adres ?? "Belirtilmemiş",
                koordinatLat = tuketimNoktasi.koordinat_lat,
                koordinatLon = tuketimNoktasi.koordinat_lot,
                baglantiGucuKw = tuketimNoktasi.baglanti_gucu_kw,
                tuketiciGrubu = tuketimNoktasi.tuketici_grubu ?? "Mesken",
                baglantiDurumu = tuketimNoktasi.baglanti_durumu
            };

            var response = await _httpClient.PostAsJsonAsync("/api/TuketimNoktasi", dto, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Tüketim noktası oluşturulamadı. Detay: {errorContent}");
            }
        }

        public async Task UpdateAsync(TuketimNoktasi tuketimNoktasi)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/TuketimNoktasi/{tuketimNoktasi.tuketim_noktasi_id}", tuketimNoktasi, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Tüketim noktası güncellenemedi. Detay: {errorContent}");
            }
        }

        public async Task DeleteAsync(string tekilKod)
        {
            await _httpClient.DeleteAsync($"/api/TuketimNoktasi/{tekilKod}");
        }
    }
}





