using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiAuditLogService : IAuditLogService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiAuditLogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

        public async System.Threading.Tasks.Task<PaginatedResponse<AuditLog>> GetAllAsync(int page = 1, int pageSize = 100)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<PaginatedResponse<AuditLog>>($"/api/AuditLog?page={page}&pageSize={pageSize}", _jsonOptions);
                return result ?? new PaginatedResponse<AuditLog>();
            }
            catch
            {
                return new PaginatedResponse<AuditLog>();
            }
        }
    }
}
