using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiFaturaService : IFaturaService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiFaturaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<(decimal BirimFiyat, decimal EnerjiBedeli, decimal DagitimBedeli, decimal TrtPayi, decimal EnerjiFonu, decimal KdvTutari, decimal ToplamTutar, List<SimulasyonKalemDto> Kalemler)> SimulasyonHesaplaAsync(string tarifeGrubu, decimal tuketimMiktari)
        {
            // Eğer Swagger/API tarafında simülasyon endpointi yoksa (şimdilik mock hesabı API içinde çalışıyormuş gibi localde simüle edebiliriz
            // ya da doğrudan API'den "/api/Fatura/Simulasyon" gibi bir endpoint kullanılabilir. 
            // Biz şimdilik mock mantığına yakın local hesap yapıp API servis imzasını sağlıyoruz.
            
            decimal birimFiyat = tarifeGrubu switch
            {
                "Ticarethane" => 3.45m,
                "Sanayi" => 2.65m,
                _ => 2.85m // Mesken
            };

            decimal dagitimBirimFiyat = 0.65m;

            decimal enerjiBedeli = tuketimMiktari * birimFiyat;
            decimal dagitimBedeli = tuketimMiktari * dagitimBirimFiyat;
            decimal trtPayi = enerjiBedeli * 0.02m;
            decimal enerjiFonu = enerjiBedeli * 0.01m;

            decimal matrah = enerjiBedeli + dagitimBedeli + trtPayi + enerjiFonu;
            decimal kdvTutari = matrah * 0.20m;
            decimal toplamTutar = matrah + kdvTutari;

            var kalemler = new List<SimulasyonKalemDto>
            {
                new SimulasyonKalemDto("Aktif Enerji Bedeli", tuketimMiktari, birimFiyat, enerjiBedeli),
                new SimulasyonKalemDto("Dağıtım Bedeli", tuketimMiktari, dagitimBirimFiyat, dagitimBedeli),
                new SimulasyonKalemDto("TRT Payı", 1, 0, trtPayi),
                new SimulasyonKalemDto("Enerji Fonu", 1, 0, enerjiFonu),
                new SimulasyonKalemDto("KDV (%20)", 1, 0, kdvTutari)
            };

            return (birimFiyat, enerjiBedeli, dagitimBedeli, trtPayi, enerjiFonu, kdvTutari, toplamTutar, kalemler);
        }

        public async Task<PagedResponse<Fatura>> GetPagedAsync(
            int page, 
            int pageSize, 
            string? faturaNo = null, 
            int? durum = null, 
            long? sozlesmeId = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrEmpty(faturaNo)) queryParams.Add($"faturaNo={Uri.EscapeDataString(faturaNo)}");
                if (durum.HasValue) queryParams.Add($"durum={durum.Value}");
                if (sozlesmeId.HasValue) queryParams.Add($"sozlesmeId={sozlesmeId.Value}");

                string url = $"/api/Fatura/Paged?{string.Join("&", queryParams)}";
                
                var result = await _httpClient.GetFromJsonAsync<PagedResponse<Fatura>>(url, _jsonOptions);
                return result ?? new PagedResponse<Fatura>();
            }
            catch
            {
                return new PagedResponse<Fatura>();
            }
        }

        public async Task<KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.FaturaListDto>> GetPagedCursorAsync(long? lastId, int limit)
        {
            try
            {
                var qs = lastId.HasValue ? $"?lastId={lastId}&limit={limit}" : $"?limit={limit}";
                var response = await _httpClient.GetFromJsonAsync<KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.FaturaListDto>>($"/api/Fatura/Cursor{qs}", _jsonOptions);
                return response ?? new KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.FaturaListDto> { PageSize = limit };
            }
            catch
            {
                return new KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.FaturaListDto> { PageSize = limit };
            }
        }

        public async Task<List<Fatura>> GetAllAsync()
        {
            try
            {
                var pagedResponse = await GetPagedAsync(1, 50000);
                return pagedResponse.Data ?? new List<Fatura>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllAsync Hata: {ex.Message}");
                return new List<Fatura>();
            }
        }

        public async Task<Fatura?> GetByIdAsync(int id)
        {
            try
            {
                // API takımının muhtemelen bir GetById endpoint'i vardır.
                var response = await _httpClient.GetFromJsonAsync<Fatura>($"/api/Fatura/{id}", _jsonOptions);
                return response;
            }
            catch
            {
                // Fallback olarak Paged üzerinden arayalım
                var paged = await GetPagedAsync(1, 100, faturaNo: null, durum: null, sozlesmeId: null);
                return paged.Data?.FirstOrDefault(x => x.fatura_id == id);
            }
        }

        public async Task<Fatura> EkleAsync(Fatura fatura)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Fatura", fatura, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return fatura;
        }

        public async Task GuncelleAsync(Fatura fatura)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Fatura/{fatura.fatura_id}", fatura, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task SilAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Fatura/{id}");
            response.EnsureSuccessStatusCode();
        }


    }
}

