using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace KcetasWeb.Services.Api
{
    public class ApiAuditLogService : IAuditLogService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;

        public ApiAuditLogService(HttpClient httpClient, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };

        }

        public async System.Threading.Tasks.Task EkleAsync(string varlikTipi, int varlikId, string islemTipi, string eskiDeger, string yeniDeger, int kullaniciId, string islemGerekcesi = null)
        {
            var log = new AuditLog
            {
                varlik_tipi = varlikTipi,
                varlik_id = varlikId,
                islem_tipi = Enum.TryParse<KcetasWeb.Models.Enums.AuditIslemTipi>(islemTipi, true, out var pType) ? pType : null,
                eski_deger = eskiDeger,
                yeni_deger = yeniDeger,
                kullanici_id = kullaniciId,
                islem_gerekcesi = islemGerekcesi,
                islem_zamani = DateTime.Now
            };

            var response = await _httpClient.PostAsJsonAsync("/api/AuditLog", log, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                // Hata durumunda loglanabilir, şimdilik sessizce yutalım veya fırlatalım
                // throw new Exception($"API Hatası: {response.StatusCode} - AuditLog oluşturulamadı.");
            }
        }

        public async System.Threading.Tasks.Task<List<AuditLog>> GetirByVarlikAsync(string varlikTipi, int varlikId)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<PaginatedResponse<AuditLog>>($"/api/AuditLog?varlikTipi={varlikTipi}&varlikId={varlikId}", _jsonOptions);
                return result?.Data ?? new List<AuditLog>();
            }
            catch
            {
                return new List<AuditLog>();
            }
        }

        public async System.Threading.Tasks.Task<PaginatedResponse<AuditLog>> GetAllAsync(int page = 1, int pageSize = 100, string? q = null, string? kullanici = null, int? kayitId = null, string? islemTipi = null, string? tarih = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrWhiteSpace(q)) queryParams.Add($"q={Uri.EscapeDataString(q.Trim())}");
                if (!string.IsNullOrWhiteSpace(kullanici)) queryParams.Add($"kullanici={Uri.EscapeDataString(kullanici.Trim())}");
                if (kayitId.HasValue) queryParams.Add($"kayitId={kayitId.Value}");
                if (!string.IsNullOrWhiteSpace(islemTipi)) queryParams.Add($"islemTipi={Uri.EscapeDataString(islemTipi.Trim())}");
                if (!string.IsNullOrWhiteSpace(tarih)) queryParams.Add($"tarih={Uri.EscapeDataString(tarih.Trim())}");

                var url = $"/api/AuditLog?{string.Join("&", queryParams)}";
                
                var pagedResult = await _httpClient.GetFromJsonAsync<PaginatedResponse<AuditLog>>(url, _jsonOptions);
                return pagedResult ?? new PaginatedResponse<AuditLog>();
            }
            catch
            {
                return new PaginatedResponse<AuditLog>();
            }
        }
    }
}
