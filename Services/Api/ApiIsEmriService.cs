using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace KcetasWeb.Services.Api
{
    public class ApiIsEmriService : IIsEmriService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<ApiIsEmriService> _logger;
        private readonly IMemoryCache _cache;

        public ApiIsEmriService(HttpClient httpClient, ILogger<ApiIsEmriService> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _cache.GetOrCreateAsync("IsEmri_TotalCount", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                try
                {
                    var jsonStr = await _httpClient.GetStringAsync("/api/IsEmirleri?includeCompleted=true&page=1&pageSize=1");
                    using var doc = JsonDocument.Parse(jsonStr);
                    
                    if (doc.RootElement.ValueKind == JsonValueKind.Array) return doc.RootElement.GetArrayLength();
                    if (doc.RootElement.TryGetProperty("totalCount", out var tc)) return tc.GetInt32();
                    if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) return data.GetArrayLength();
                    
                    return 0;
                }
                catch { return 0; }
            });
        }

        public async Task<List<IsEmri>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync("IsEmri_GetAll", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                try
                {
                    List<IsEmri> list = new List<IsEmri>();
                    int currentPage = 1;
                    int totalPages = 1;

                    do
                    {
                        try
                        {
                            var response = await _httpClient.GetAsync($"/api/IsEmirleri?includeCompleted=true&page={currentPage}&pageSize=500");
                            response.EnsureSuccessStatusCode();
                            
                            var json = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                            
                            if (json.TryGetProperty("totalPages", out var tp) && tp.ValueKind == JsonValueKind.Number)
                            {
                                totalPages = tp.GetInt32();
                                // GÜVENLİK/PERFORMANS: Sunucudaki 1 milyon veriyi RAM'e çekip çökmesini önlemek için
                                // Maksimum 10 sayfa (10 x 500 = 5000 kayıt) çekmesine izin veriyoruz.
                                if (totalPages > 10) totalPages = 10;
                            }

                            if (json.ValueKind == JsonValueKind.Array)
                            {
                                var items = json.Deserialize<List<IsEmri>>(_jsonOptions);
                                if (items != null) list.AddRange(items);
                                break;
                            }
                            else if (json.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
                            {
                                var items = data.Deserialize<List<IsEmri>>(_jsonOptions);
                                if (items != null) list.AddRange(items);
                            }
                            else if (json.TryGetProperty("Data", out var capitalData) && capitalData.ValueKind == JsonValueKind.Array)
                            {
                                var items = capitalData.Deserialize<List<IsEmri>>(_jsonOptions);
                                if (items != null) list.AddRange(items);
                            }
                            else if (json.TryGetProperty("items", out var itemsNode) && itemsNode.ValueKind == JsonValueKind.Array)
                            {
                                var items = itemsNode.Deserialize<List<IsEmri>>(_jsonOptions);
                                if (items != null) list.AddRange(items);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"IsEmirleri API'den çekilirken Sayfa {currentPage} üzerinde veri hatası oluştu. Bu sayfa atlanıyor.");
                        }
                        
                        currentPage++;
                    } while (currentPage <= totalPages);
                    
                    return list;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "İş Emirleri API'den çekilirken hata oluştu.");
                    return new List<IsEmri>();
                }
            }) ?? new List<IsEmri>();
        }

        public async Task<IsEmri?> GetByIdAsync(long id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IsEmri>($"/api/IsEmirleri/{id}", _jsonOptions);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"İş Emri (ID: {id}) API'den çekilirken hata oluştu.");
                throw;
            }
        }

        public async Task<List<IsEmri>> FiltreleAsync(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var all = await GetAllAsync();
            var query = all.AsQueryable();

            if (!string.IsNullOrEmpty(tip))
                query = query.Where(x => ((int)x.tip).ToString() == tip || x.tip.ToString() == tip);

            if (!string.IsNullOrEmpty(durum))
                query = query.Where(x => ((int)x.durum).ToString() == durum || x.durum.ToString() == durum);

            if (baslangic.HasValue)
                query = query.Where(x => x.planlanan_tarih >= baslangic.Value);

            if (bitis.HasValue)
                query = query.Where(x => x.planlanan_tarih <= bitis.Value);

            if (!string.IsNullOrEmpty(arama))
            {
                arama = arama.ToLower();
                query = query.Where(x => 
                    (x.is_emri_no != null && x.is_emri_no.ToLower().Contains(arama))
                );
            }

            return query.ToList();
        }

        public async Task TutanakKaydetAsync(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks)
        {
            var isEmri = await GetByIdAsync(isEmriId);
            if (isEmri == null) return;

            isEmri.tutanak_no = tutanakNo;
            isEmri.saha_sonucu = sahaSonucu;
            isEmri.gerekce = gerekce;
            isEmri.muhur_no = muhurNo;
            isEmri.durum = KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi;
            isEmri.updated_at = DateTime.Now;
            
            // Backend validation hatasını önlemek için eksik olan required navigation nesnelerini dolduruyoruz
            if (isEmri.TuketimNoktasi == null || isEmri.TuketimNoktasi.ToString() == "{ }") {
                try {
                    var tnResponse = await _httpClient.GetAsync("/api/TuketimNoktasi?page=1&pageSize=5000");
                    if (tnResponse.IsSuccessStatusCode) {
                        var tnList = await tnResponse.Content.ReadFromJsonAsync<List<KcetasWeb.Models.TuketimNoktasi>>(_jsonOptions);
                        isEmri.TuketimNoktasi = tnList?.FirstOrDefault(x => x.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);
                    }
                } catch { }
                
                if (isEmri.TuketimNoktasi == null) {
                    isEmri.TuketimNoktasi = new KcetasWeb.Models.TuketimNoktasi { 
                        tuketim_noktasi_id = isEmri.tuketim_noktasi_id, 
                        tekil_kod = "GEÇİCİ",
                        ilce_id = 1,
                        tuketici_grubu = "MESKEN",
                        baglanti_gucu_kw = 1
                    };
                }
            }
            if (string.IsNullOrEmpty(isEmri.oncelik)) {
                isEmri.oncelik = "NORMAL";
            }

            var response = await _httpClient.PutAsJsonAsync($"/api/IsEmirleri/{isEmriId}", isEmri, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Tutanak kaydedilirken hata oluştu. Detay: {err}");
            }
            _cache.Remove("IsEmri_GetAll"); _cache.Remove("IsEmri_TotalCount");
        }

        public async Task<IsEmri> EkleAsync(IsEmri isEmri)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/IsEmirleri", isEmri, _jsonOptions);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<IsEmri>(_jsonOptions);
            _cache.Remove("IsEmri_GetAll"); _cache.Remove("IsEmri_TotalCount");
            return result ?? isEmri;
        }

        public async Task DurumGuncelleAsync(long id, KcetasWeb.Models.Enums.IsEmriDurumu yeniDurum)
        {
            var isEmri = await GetByIdAsync(id);
            if (isEmri == null) return;

            isEmri.durum = yeniDurum;
            isEmri.updated_at = DateTime.Now;
            
            // Backend validation hatasını önlemek için eksik olan required navigation nesnelerini dolduruyoruz
            if (isEmri.TuketimNoktasi == null || isEmri.TuketimNoktasi.ToString() == "{ }") {
                try {
                    var tnResponse = await _httpClient.GetAsync("/api/TuketimNoktasi?page=1&pageSize=5000");
                    if (tnResponse.IsSuccessStatusCode) {
                        var tnList = await tnResponse.Content.ReadFromJsonAsync<List<KcetasWeb.Models.TuketimNoktasi>>(_jsonOptions);
                        isEmri.TuketimNoktasi = tnList?.FirstOrDefault(x => x.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);
                    }
                } catch { }
                
                if (isEmri.TuketimNoktasi == null) {
                    isEmri.TuketimNoktasi = new KcetasWeb.Models.TuketimNoktasi { 
                        tuketim_noktasi_id = isEmri.tuketim_noktasi_id, 
                        tekil_kod = "GEÇİCİ",
                        ilce_id = 1,
                        tuketici_grubu = "MESKEN",
                        baglanti_gucu_kw = 1
                    };
                }
            }
            if (string.IsNullOrEmpty(isEmri.oncelik)) {
                isEmri.oncelik = "NORMAL";
            }

            var jsonStr = System.Text.Json.JsonSerializer.Serialize(isEmri, _jsonOptions);
            Console.WriteLine("PUT PAYLOAD: " + jsonStr);

            var response = await _httpClient.PutAsJsonAsync($"/api/IsEmirleri/{id}", isEmri, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - İş Emri durumu güncellenemedi. Detay: {err}");
            }
            _cache.Remove("IsEmri_GetAll"); _cache.Remove("IsEmri_TotalCount");
        }

        public async Task PersonelAtaAsync(long id, long personelId)
        {
            var requestDto = new { isEmriId = id, ustaId = personelId };
            var response = await _httpClient.PostAsJsonAsync("/api/IsEmirleri/atama-yap", requestDto, _jsonOptions);
            
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - İş Emri personeli atanamadı. Detay: {err}");
            }
            _cache.Remove("IsEmri_GetAll"); _cache.Remove("IsEmri_TotalCount");
        }
    }
}


