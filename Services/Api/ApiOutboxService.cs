using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.ViewModels;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiOutboxService : IOutboxService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiOutboxService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };

        }

        public async Task<List<EntegrasyonOutbox>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<EntegrasyonOutbox>>("/api/EntegrasyonOutbox", _jsonOptions);
                return (result ?? new List<EntegrasyonOutbox>())
                    .OrderByDescending(x => x.created_at)
                    .ToList();
            }
            catch
            {
                return new List<EntegrasyonOutbox>();
            }
        }

        public async Task<EntegrasyonOutbox?> GetByIdAsync(long id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<EntegrasyonOutbox>($"/api/EntegrasyonOutbox/{id}", _jsonOptions);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<PaginatedResponse<EntegrasyonOutbox>> FiltreleAsync(string? durum, string? hedefSistem, DateTime? baslangic, DateTime? bitis, string? faturaNo = null, int page = 1, int pageSize = 50)
        {
            try
            {
                var queryParams = new List<string> { $"page={page}", $"pageSize={pageSize}" };
                
                var normalizedDurum = OutboxListeViewModel.NormalizeDurum(durum);
                if (!string.IsNullOrEmpty(normalizedDurum))
                {
                    string durumInt = normalizedDurum switch
                    {
                        "BEKLIYOR" => "1",
                        "GONDERILDI" => "2",
                        "BASARISIZ" => "3",
                        "HATALI" => "3",
                        "IPTAL" => "4",
                        _ => ""
                    };
                    if (!string.IsNullOrEmpty(durumInt)) queryParams.Add($"durum={durumInt}");
                }

                if (!string.IsNullOrEmpty(hedefSistem)) queryParams.Add($"hedefSistem={Uri.EscapeDataString(hedefSistem)}");
                if (!string.IsNullOrEmpty(faturaNo)) queryParams.Add($"faturaNo={Uri.EscapeDataString(faturaNo)}");
                
                if (baslangic.HasValue) queryParams.Add($"baslangic={baslangic.Value:yyyy-MM-dd}");
                if (bitis.HasValue) queryParams.Add($"bitis={bitis.Value:yyyy-MM-dd}");

                var queryString = string.Join("&", queryParams);
                var result = await _httpClient.GetFromJsonAsync<PaginatedResponse<EntegrasyonOutbox>>($"/api/EntegrasyonOutbox?{queryString}", _jsonOptions);
                return result ?? new PaginatedResponse<EntegrasyonOutbox>();
            }
            catch
            {
                return new PaginatedResponse<EntegrasyonOutbox>();
            }
        }

        public async Task<(int Toplam, int Bekleyen, int Gonderilmis, int Basarisiz)> GetIstatistiklerAsync()
        {
            try
            {
                var tTotal = _httpClient.GetFromJsonAsync<PaginatedResponse<EntegrasyonOutbox>>("/api/EntegrasyonOutbox?page=1&pageSize=1", _jsonOptions);
                var tBekleyen = _httpClient.GetFromJsonAsync<PaginatedResponse<EntegrasyonOutbox>>("/api/EntegrasyonOutbox?durum=1&page=1&pageSize=1", _jsonOptions);
                var tGonderildi = _httpClient.GetFromJsonAsync<PaginatedResponse<EntegrasyonOutbox>>("/api/EntegrasyonOutbox?durum=2&page=1&pageSize=1", _jsonOptions);
                var tBasarisiz = _httpClient.GetFromJsonAsync<PaginatedResponse<EntegrasyonOutbox>>("/api/EntegrasyonOutbox?durum=3&page=1&pageSize=1", _jsonOptions);

                await Task.WhenAll(tTotal, tBekleyen, tGonderildi, tBasarisiz);

                return (
                    tTotal.Result?.TotalCount ?? 0,
                    tBekleyen.Result?.TotalCount ?? 0,
                    tGonderildi.Result?.TotalCount ?? 0,
                    tBasarisiz.Result?.TotalCount ?? 0
                );
            }
            catch
            {
                return (0, 0, 0, 0);
            }
        }

        public async Task<bool> YenidenGonderAsync(long id)
        {
            var kayit = await GetByIdAsync(id);
            if (kayit == null) return false;

            var updateDto = new
            {
                outboxId = kayit.outbox_id,
                durum = "BEKLIYOR",
                retryCount = kayit.retry_count,
                hataMesaji = (string?)null
            };

            var response = await _httpClient.PutAsJsonAsync($"/api/EntegrasyonOutbox/{id}", updateDto, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
    }
}

