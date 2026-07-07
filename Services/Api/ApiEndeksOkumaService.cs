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

        public List<EndeksOkuma> GetAll()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<EndeksOkuma>>("/api/EndeksOkuma", _jsonOptions).GetAwaiter().GetResult();
                return result ?? new List<EndeksOkuma>();
            }
            catch
            {
                return new List<EndeksOkuma>();
            }
        }

        public EndeksOkuma? GetById(long id)
        {
            try
            {
                return _httpClient.GetFromJsonAsync<EndeksOkuma>($"/api/EndeksOkuma/{id}", _jsonOptions).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public List<EndeksOkuma> Filtrele(string? okumaTipi, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var query = GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(okumaTipi))
                query = query.Where(x => x.okuma_tipi == okumaTipi);

            if (!string.IsNullOrEmpty(durum))
                query = query.Where(x => x.status == durum);

            if (baslangic.HasValue)
                query = query.Where(x => x.okuma_zamani >= baslangic.Value);

            if (bitis.HasValue)
                query = query.Where(x => x.okuma_zamani <= bitis.Value);

            // Arama sayac_id veya benzeri bir field üzerinden yapılabilir (modelde sayac_id var string değil)
            
            return query.ToList();
        }

        public (int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim) GetIstatistikler()
        {
            var all = GetAll();
            var toplam = all.Count;
            var manuel = all.Count(x => x.okuma_kaynagi == "Manuel");
            var osos = all.Count(x => x.okuma_kaynagi == "OSOS");
            var anomali = all.Count(x => x.anomali_mi == true || !string.IsNullOrEmpty(x.AnomaliAciklamasi));
            var ortalama = toplam > 0 ? all.Average(x => ((x.yeni_endeks ?? 0) - (x.onceki_endeks ?? 0))) : 0;

            return (toplam, manuel, osos, anomali, ortalama);
        }
    }
}
