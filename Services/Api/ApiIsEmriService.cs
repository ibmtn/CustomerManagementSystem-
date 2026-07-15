using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiIsEmriService : IIsEmriService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiIsEmriService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public List<IsEmri> GetAll()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<IsEmri>>("/api/IsEmirleri/all", _jsonOptions).GetAwaiter().GetResult();
                return result ?? new List<IsEmri>();
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("is_emri_err.txt", ex.ToString());
                return new List<IsEmri>();
            }
        }

        public IsEmri? GetById(long id)
        {
            try
            {
                return _httpClient.GetFromJsonAsync<IsEmri>($"/api/IsEmirleri/{id}", _jsonOptions).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public List<IsEmri> Filtrele(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var query = GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(tip))
                query = query.Where(x => x.tip == tip);

            if (!string.IsNullOrEmpty(durum))
                query = query.Where(x => x.durum == durum);

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

        public void TutanakKaydet(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks)
        {
            var isEmri = GetById(isEmriId);
            if (isEmri == null) return;

            isEmri.tutanak_no = tutanakNo;
            isEmri.saha_sonucu = sahaSonucu;
            isEmri.gerekce = gerekce;
            isEmri.muhur_no = muhurNo;
            isEmri.durum = "Tamamlandı";
            isEmri.updated_at = DateTime.Now;

            _httpClient.PutAsJsonAsync($"/api/IsEmirleri/{isEmriId}", isEmri, _jsonOptions).GetAwaiter().GetResult();
        }

        public void Ekle(IsEmri isEmri)
        {
            var response = _httpClient.PostAsJsonAsync("/api/IsEmirleri", isEmri, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new Exception($"API Hatası: {response.StatusCode} - İş emri oluşturulamadı. Detay: {errorContent}");
            }
        }

        public void DurumGuncelle(long id, string yeniDurum)
        {
            var isEmri = GetById(id);
            if (isEmri == null) return;

            isEmri.durum = yeniDurum;
            isEmri.updated_at = DateTime.Now;

            _httpClient.PutAsJsonAsync($"/api/IsEmirleri/{id}", isEmri, _jsonOptions).GetAwaiter().GetResult();
        }
    }
}
