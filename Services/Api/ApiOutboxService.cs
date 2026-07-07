using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
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

        public List<EntegrasyonOutbox> GetAll()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<EntegrasyonOutbox>>("/api/EntegrasyonOutbox", _jsonOptions).GetAwaiter().GetResult();
                return result ?? new List<EntegrasyonOutbox>();
            }
            catch
            {
                return new List<EntegrasyonOutbox>();
            }
        }

        public EntegrasyonOutbox? GetById(long id)
        {
            try
            {
                return _httpClient.GetFromJsonAsync<EntegrasyonOutbox>($"/api/EntegrasyonOutbox/{id}", _jsonOptions).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public List<EntegrasyonOutbox> Filtrele(string? durum, string? eventType, DateTime? baslangic, DateTime? bitis)
        {
            var query = GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(durum))
                query = query.Where(x => x.durum == durum);

            if (!string.IsNullOrEmpty(eventType))
                query = query.Where(x => x.hedef_sistem == eventType); // outbox'ta eventType yok, hedef_sistem veya islem_tipi olabilir.

            if (baslangic.HasValue)
                query = query.Where(x => x.created_at >= baslangic.Value);

            if (bitis.HasValue)
                query = query.Where(x => x.created_at <= bitis.Value);

            return query.ToList();
        }

        public (int Toplam, int Bekleyen, int Gonderilmis, int Basarisiz) GetIstatistikler()
        {
            var all = GetAll();
            var toplam = all.Count;
            var bekleyen = all.Count(x => x.durum == "Bekliyor");
            var gonderilmis = all.Count(x => x.durum == "Gönderildi");
            var basarisiz = all.Count(x => x.durum == "Başarısız");

            return (toplam, bekleyen, gonderilmis, basarisiz);
        }

        public bool YenidenGonder(long id)
        {
            var kayit = GetById(id);
            if (kayit == null) return false;

            kayit.durum = "Bekliyor";
            kayit.hata_mesaji = null;
            
            var response = _httpClient.PutAsJsonAsync($"/api/EntegrasyonOutbox/{id}", kayit, _jsonOptions).GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
    }
}
