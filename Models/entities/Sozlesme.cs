namespace KcetasWeb.Models
{
    using System;

    public class Sozlesme
    {
        [System.Text.Json.Serialization.JsonPropertyName("sozlesmeId")]
        public int sozlesme_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sozlesmeNo")]
        public string sozlesme_no { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tuketimNoktasiId")]
        public int tuketim_noktasi_id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("aboneId")]
        public int? abone_id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tarifeId")]
        public int? tarife_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sozlesmeTipi")]
        public string? sozlesme_tipi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("baslangicTarihi")]
        [System.Text.Json.Serialization.JsonConverter(typeof(KcetasWeb.Helpers.DateOnlyJsonConverter))]
        public DateTime? baslangic_tarihi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("bitisTarihi")]
        [System.Text.Json.Serialization.JsonConverter(typeof(KcetasWeb.Helpers.DateOnlyJsonConverter))]
        public DateTime? bitis_tarihi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("guvenceBedeli")]
        public decimal? guvence_bedeli { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("durum")]
        public KcetasWeb.Models.Enums.SozlesmeDurumu? durum { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime created_at { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedAt")]
        public DateTime? updated_at { get; set; }
    }
}
