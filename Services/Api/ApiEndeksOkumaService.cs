using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiEndeksOkumaService : IEndeksOkumaService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiEndeksOkumaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };

        }

        public async System.Threading.Tasks.Task<List<EndeksOkuma>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<EndeksOkuma>>("/api/EndeksOkuma", _jsonOptions);
                return result ?? new List<EndeksOkuma>();
            }
            catch
            {
                return new List<EndeksOkuma>();
            }
        }

        public async System.Threading.Tasks.Task<EndeksOkuma?> GetByIdAsync(long id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<EndeksOkuma>($"/api/EndeksOkuma/{id}", _jsonOptions);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async System.Threading.Tasks.Task<List<EndeksOkuma>> FiltreleAsync(string? okumaTipi, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var data = await GetAllAsync();
            var query = data.AsQueryable();

            if (!string.IsNullOrEmpty(okumaTipi))
                query = query.Where(x => ((int?)x.okuma_tipi).ToString() == okumaTipi || x.okuma_tipi.ToString() == okumaTipi);

            if (!string.IsNullOrEmpty(durum))
                query = query.Where(x => x.status == durum);

            if (baslangic.HasValue)
                query = query.Where(x => x.okuma_zamani >= baslangic.Value);

            if (bitis.HasValue)
                query = query.Where(x => x.okuma_zamani <= bitis.Value);

            // Arama sayac_id veya benzeri bir field üzerinden yapılabilir (modelde sayac_id var string değil)
            
            return query.ToList();
        }

        public async System.Threading.Tasks.Task<PagedResponse<EndeksOkuma>> GetPagedAsync(
            int page, 
            int pageSize,
            string? okumaTipi,
            string? durum,
            DateTime? baslangic,
            DateTime? bitis,
            string? aramaMetni,
            string? sayacId,
            string? donem,
            string? dogrulamaDurumu)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrEmpty(okumaTipi)) queryParams.Add($"okumaTipi={Uri.EscapeDataString(okumaTipi)}");
                if (!string.IsNullOrEmpty(durum)) queryParams.Add($"durum={Uri.EscapeDataString(durum)}");
                if (baslangic.HasValue) queryParams.Add($"baslangic={baslangic.Value.ToString("yyyy-MM-ddTHH:mm:ss")}");
                if (bitis.HasValue) queryParams.Add($"bitis={bitis.Value.ToString("yyyy-MM-ddTHH:mm:ss")}");
                if (!string.IsNullOrEmpty(aramaMetni)) queryParams.Add($"arama={Uri.EscapeDataString(aramaMetni)}");
                if (!string.IsNullOrEmpty(sayacId)) queryParams.Add($"sayacId={Uri.EscapeDataString(sayacId)}");
                if (!string.IsNullOrEmpty(donem)) queryParams.Add($"donem={Uri.EscapeDataString(donem)}");
                if (!string.IsNullOrEmpty(dogrulamaDurumu)) queryParams.Add($"dogrulamaDurumu={Uri.EscapeDataString(dogrulamaDurumu)}");

                string url = $"/api/EndeksOkuma/Paged?{string.Join("&", queryParams)}";
                
                var result = await _httpClient.GetFromJsonAsync<PagedResponse<EndeksOkuma>>(url, _jsonOptions);
                return result ?? new PagedResponse<EndeksOkuma>();
            }
            catch
            {
                return new PagedResponse<EndeksOkuma>();
            }
        }

        private class EndeksOkumaStatsResponse
        {
            public int ToplamOkuma { get; set; }
            public int OsosOkuma { get; set; }
            public int ManuelOkuma { get; set; }
            public int Duzeltme { get; set; }
        }

        public async System.Threading.Tasks.Task<(int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim)> GetIstatistiklerAsync(string? donem = null)
        {
            try
            {
                string url = "/api/EndeksOkuma/Stats";
                if (!string.IsNullOrEmpty(donem))
                {
                    url += $"?donem={Uri.EscapeDataString(donem)}";
                }

                var stats = await _httpClient.GetFromJsonAsync<EndeksOkumaStatsResponse>(url, _jsonOptions);
                if (stats != null)
                {
                    return (stats.ToplamOkuma, stats.ManuelOkuma, stats.OsosOkuma, stats.Duzeltme, 0m);
                }
            }
            catch
            {
                // Fallback on error
            }
            return (0, 0, 0, 0, 0m);
        }

        public async System.Threading.Tasks.Task CreateAsync(EndeksOkuma model)
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(model, _jsonOptions);
            System.IO.File.WriteAllText("debug_json.txt", jsonString);

            var response = await _httpClient.PostAsJsonAsync("/api/EndeksOkuma", model, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Endeks okuması oluşturulamadı. Detay: {errorContent}");
            }
        }
        public async System.Threading.Tasks.Task UpdateAsync(EndeksOkuma model)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/EndeksOkuma/{model.okuma_id}", model, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Endeks okuması güncellenemedi. Detay: {errorContent}");
            }
        }
        
        public async System.Threading.Tasks.Task DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"/api/EndeksOkuma/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Endeks okuması silinemedi. Detay: {errorContent}");
            }
        }
    }
}

