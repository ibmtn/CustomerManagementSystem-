using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
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

        public (decimal BirimFiyat, decimal EnerjiBedeli, decimal DagitimBedeli, decimal TrtPayi, decimal EnerjiFonu, decimal KdvTutari, decimal ToplamTutar, List<SimulasyonKalemDto> Kalemler) SimulasyonHesapla(string tarifeGrubu, decimal tuketimMiktari)
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
    }
}
