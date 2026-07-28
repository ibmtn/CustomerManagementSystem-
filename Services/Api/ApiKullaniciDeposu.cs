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

        public async Task<Kullanici?> BulIdAsync(long id)
        {
            var liste = await ListeleAsync();
            return liste.FirstOrDefault(k => k.kullanici_id == id);
        }

        public async Task<bool> GirisKontrolAsync(string kullaniciAdi, string sifre)
        {
            var body = new { kullaniciAdi = kullaniciAdi, sifre = sifre };
            var response = await _httpClient.PostAsJsonAsync("/api/Auth/Login", body);
            return response.IsSuccessStatusCode;
        }

        public async Task<Kullanici?> BulKullaniciAdiIleAsync(string kullaniciAdi)
        {
            // Kullanicilar listesini çekip filtreliyoruz (API'de özel bir endpoint yoksa)
            var liste = await ListeleAsync();
            return liste.FirstOrDefault(k => string.Equals(k.kullanici_adi, kullaniciAdi, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Kullanici> EkleAsync(Kullanici kullanici)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Kullanici", kullanici, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}). Details: {error}");
            }
            return kullanici; // API'den dönen model de alınabilir ama şimdilik request nesnesini döndürüyoruz.
        }

        public async Task<bool> GuncelleAsync(Kullanici kullanici)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Kullanici/{kullanici.kullanici_id}", kullanici, _jsonOptions);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> KullaniciAdiVarMiAsync(string kullaniciAdi)
        {
            var result = await BulKullaniciAdiIleAsync(kullaniciAdi);
            return result != null;
        }

        public async Task<List<Kullanici>> ListeleAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Kullanici>>("/api/Kullanici", _jsonOptions);
                return result ?? new List<Kullanici>();
            }
            catch
            {
                return new List<Kullanici>();
            }
        }

        public async Task<bool> SilAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Kullanici/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
