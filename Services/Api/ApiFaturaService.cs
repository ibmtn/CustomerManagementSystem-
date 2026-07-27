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

                public async Task<PaginatedResponse<Fatura>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                var allData = await GetAllAsync();
                var count = allData.Count;
                var pagedData = allData.OrderByDescending(x => x.fatura_id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new PaginatedResponse<Fatura>
                {
                    Data = pagedData,
                    TotalCount = count,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize)
                };
            }
            catch
            {
                return new PaginatedResponse<Fatura> { CurrentPage = page, PageSize = pageSize };
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
                var response = await _httpClient.GetAsync("/api/Fatura?page=1&pageSize=1000");
                response.EnsureSuccessStatusCode();
                var jsonStr = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonStr);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<Fatura>>(jsonStr, _jsonOptions) ?? new List<Fatura>();
                }
                else if (doc.RootElement.TryGetProperty("data", out var dataProp))
                {
                    System.IO.File.WriteAllText("fatura_debug.txt", "Found data property. Length: " + dataProp.GetRawText().Length);
                    var result = JsonSerializer.Deserialize<List<Fatura>>(dataProp.GetRawText(), _jsonOptions);
                    System.IO.File.AppendAllText("fatura_debug.txt", "\nDeserialized count: " + (result?.Count ?? -1));
                    return result ?? new List<Fatura>();
                }
                else if (doc.RootElement.TryGetProperty("Data", out var capitalDataProp))
                {
                    return JsonSerializer.Deserialize<List<Fatura>>(capitalDataProp.GetRawText(), _jsonOptions) ?? new List<Fatura>();
                }
                return new List<Fatura>();
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("fatura_err.txt", ex.ToString());
                System.Diagnostics.Debug.WriteLine($"GetAllAsync Hata: {ex.Message}");
                throw; // Do not swallow!
            }
        }

        public async Task<Fatura?> GetByIdAsync(int id)
        {
            try
            {
                var all = await GetAllAsync();
                return all.FirstOrDefault(x => x.fatura_id == id);
            }
            catch
            {
                throw;
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

