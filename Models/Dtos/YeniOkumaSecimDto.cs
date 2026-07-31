using System.Text.Json.Serialization;

namespace KcetasWeb.Models.Dtos
{
    public class YeniOkumaSecimDto
    {
        [JsonPropertyName("sozlesmeId")]
        public long SozlesmeId { get; set; }

        [JsonPropertyName("sozlesmeNo")]
        public string? SozlesmeNo { get; set; }

        [JsonPropertyName("tuketimNoktasiId")]
        public long TuketimNoktasiId { get; set; }

        [JsonPropertyName("tekilKod")]
        public string? TekilKod { get; set; }

        [JsonPropertyName("adres")]
        public string? Adres { get; set; }

        [JsonPropertyName("sayacId")]
        public long SayacId { get; set; }

        [JsonPropertyName("sayacSeriNo")]
        public string? SayacSeriNo { get; set; }

        [JsonPropertyName("sonEndeks")]
        public decimal? SonEndeks { get; set; }

        [JsonPropertyName("donem")]
        public string? Donem { get; set; }
    }
}
