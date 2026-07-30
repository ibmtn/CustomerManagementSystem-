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
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                try
                {
                    const int pageSize = 500;
                    const int maxPagesToFetch = 10;
                    var list = new List<IsEmri>();

                    var firstPage = await GetIsEmriPageAsync(1, pageSize);
                    if (firstPage.Json.ValueKind == JsonValueKind.Array)
                    {
                        HydrateCachedOncelikler(firstPage.Items);
                        return firstPage.Items
                            .OrderByDescending(x => x.created_at)
                            .ThenByDescending(x => x.is_emri_id)
                            .ToList();
                    }

                    var totalPages = GetTotalPages(firstPage.Json);
                    var startPage = Math.Max(1, totalPages - maxPagesToFetch + 1);
                    var endPage = totalPages;

                    // API eski kayıtları önce döndürdüğü için büyük veri setinde son sayfaları çekiyoruz.
                    // Böylece yeni oluşturulan iş emirleri liste ve filtrelerde görünür.
                    if (startPage == 1)
                    {
                        list.AddRange(firstPage.Items);
                    }

                    for (var currentPage = Math.Max(2, startPage); currentPage <= endPage; currentPage++)
                    {
                        try
                        {
                            var page = await GetIsEmriPageAsync(currentPage, pageSize);
                            list.AddRange(page.Items);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"IsEmirleri API'den çekilirken Sayfa {currentPage} üzerinde veri hatası oluştu. Bu sayfa atlanıyor.");
                        }
                    }

                    if (list.Count == 0)
                    {
                        list.AddRange(firstPage.Items);
                    }

                    HydrateCachedOncelikler(list);

                    return list
                        .GroupBy(x => x.is_emri_id)
                        .Select(g => g.First())
                        .OrderByDescending(x => x.created_at)
                        .ThenByDescending(x => x.is_emri_id)
                        .ToList();
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
                var isEmri = await _httpClient.GetFromJsonAsync<IsEmri>($"/api/IsEmirleri/{id}", _jsonOptions);
                HydrateCachedOncelik(isEmri);
                return isEmri;
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
                query = query.Where(x => ((int)x.durum).ToString() == durum || x.durum.ToString().Equals(durum, StringComparison.OrdinalIgnoreCase));

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
            var dto = new
            {
                tuketimNoktasiId = isEmri.tuketim_noktasi_id,
                sayacId = isEmri.sayac_id.HasValue && isEmri.sayac_id.Value > 0 ? isEmri.sayac_id : null,
                atananKullaniciId = isEmri.atanan_kullanici_id,
                tip = isEmri.tip,
                oncelik = NormalizeOncelik(isEmri.oncelik),
                durum = isEmri.durum == 0
                    ? (isEmri.atanan_kullanici_id.HasValue ? KcetasWeb.Models.Enums.IsEmriDurumu.Atandi : KcetasWeb.Models.Enums.IsEmriDurumu.Acik)
                    : isEmri.durum
            };

            var response = await _httpClient.PostAsJsonAsync("/api/IsEmirleri", dto, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - İş emri oluşturulamadı. Detay: {err}");
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var result = json.ValueKind == JsonValueKind.Object && json.TryGetProperty("data", out var data)
                ? data.Deserialize<IsEmri>(_jsonOptions)
                : json.Deserialize<IsEmri>(_jsonOptions);

            var created = result ?? isEmri;
            RememberCreatedOncelik(created, isEmri.oncelik);
            _cache.Remove("IsEmri_GetAll"); _cache.Remove("IsEmri_TotalCount");
            return created;
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

        private static string NormalizeOncelik(string? oncelik)
        {
            if (string.IsNullOrWhiteSpace(oncelik)) return "NORMAL";

            return oncelik.Trim().ToUpperInvariant() switch
            {
                "DÜŞÜK" or "DUSUK" => "DUSUK",
                "YÜKSEK" or "YUKSEK" => "YUKSEK",
                "ACİL" or "ACIL" => "ACIL",
                _ => "NORMAL"
            };
        }

        private void RememberCreatedOncelik(IsEmri isEmri, string? fallbackOncelik = null)
        {
            if (isEmri.is_emri_id <= 0) return;

            var oncelik = !string.IsNullOrWhiteSpace(isEmri.oncelik)
                ? isEmri.oncelik
                : fallbackOncelik;

            _cache.Set(GetCreatedOncelikCacheKey(isEmri.is_emri_id), NormalizeOncelik(oncelik), TimeSpan.FromHours(6));
        }

        private void HydrateCachedOncelikler(IEnumerable<IsEmri> isEmirleri)
        {
            foreach (var isEmri in isEmirleri)
            {
                HydrateCachedOncelik(isEmri);
            }
        }

        private void HydrateCachedOncelik(IsEmri? isEmri)
        {
            if (isEmri == null || !string.IsNullOrWhiteSpace(isEmri.oncelik)) return;

            if (_cache.TryGetValue<string>(GetCreatedOncelikCacheKey(isEmri.is_emri_id), out var oncelik))
            {
                isEmri.oncelik = oncelik;
            }
        }

        private static string GetCreatedOncelikCacheKey(long isEmriId) => $"IsEmri_Created_Oncelik_{isEmriId}";

        private async Task<(JsonElement Json, List<IsEmri> Items)> GetIsEmriPageAsync(int page, int pageSize)
        {
            var response = await _httpClient.GetAsync($"/api/IsEmirleri?includeCompleted=true&page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            return (json, DeserializeIsEmriList(json));
        }

        private List<IsEmri> DeserializeIsEmriList(JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Array)
            {
                return json.Deserialize<List<IsEmri>>(_jsonOptions) ?? new List<IsEmri>();
            }

            if (json.ValueKind != JsonValueKind.Object)
            {
                return new List<IsEmri>();
            }

            if (json.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
            {
                return data.Deserialize<List<IsEmri>>(_jsonOptions) ?? new List<IsEmri>();
            }

            if (json.TryGetProperty("Data", out var capitalData) && capitalData.ValueKind == JsonValueKind.Array)
            {
                return capitalData.Deserialize<List<IsEmri>>(_jsonOptions) ?? new List<IsEmri>();
            }

            if (json.TryGetProperty("items", out var itemsNode) && itemsNode.ValueKind == JsonValueKind.Array)
            {
                return itemsNode.Deserialize<List<IsEmri>>(_jsonOptions) ?? new List<IsEmri>();
            }

            return new List<IsEmri>();
        }

        private static int GetTotalPages(JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Object &&
                json.TryGetProperty("totalPages", out var totalPages) &&
                totalPages.ValueKind == JsonValueKind.Number)
            {
                return Math.Max(1, totalPages.GetInt32());
            }

            return 1;
        }
    }
}
