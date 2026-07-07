using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiKullaniciDeposu : IKullaniciDeposu
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiKullaniciDeposu(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public Kullanici? BulId(long id)
        {
            try
            {
                var url = $"/api/Kullanici/{id}";
                return _httpClient.GetFromJsonAsync<Kullanici>(url, _jsonOptions).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public Kullanici? BulKullaniciAdiIle(string kullaniciAdi)
        {
            // Kullanicilar listesini çekip filtreliyoruz (API'de özel bir endpoint yoksa)
            var liste = Listele();
            return liste.FirstOrDefault(k => string.Equals(k.kullanici_adi, kullaniciAdi, StringComparison.OrdinalIgnoreCase));
        }

        public Kullanici Ekle(Kullanici kullanici)
        {
            var response = _httpClient.PostAsJsonAsync("/api/Kullanici", kullanici, _jsonOptions).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            return kullanici; // API'den dönen model de alınabilir ama şimdilik request nesnesini döndürüyoruz.
        }

        public bool Guncelle(Kullanici kullanici)
        {
            var response = _httpClient.PutAsJsonAsync($"/api/Kullanici/{kullanici.kullanici_id}", kullanici, _jsonOptions).GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }

        public bool KullaniciAdiVarMi(string kullaniciAdi)
        {
            return BulKullaniciAdiIle(kullaniciAdi) != null;
        }

        public List<Kullanici> Listele()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<Kullanici>>("/api/Kullanici", _jsonOptions).GetAwaiter().GetResult();
                return result ?? new List<Kullanici>();
            }
            catch
            {
                return new List<Kullanici>();
            }
        }

        public bool Sil(long id)
        {
            var response = _httpClient.DeleteAsync($"/api/Kullanici/{id}").GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
    }
}
