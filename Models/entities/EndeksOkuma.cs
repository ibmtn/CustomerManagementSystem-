namespace KcetasWeb.Models
{
    using System;

    public class EndeksOkuma
    {
        [System.Text.Json.Serialization.JsonPropertyName("okumaId")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public int okuma_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sayacId")]
        public int? sayac_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("donem")]
        public string? donem { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("isEmriId")]
        public int? is_emri_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sozlesmeId")]
        public int? sozlesme_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("okumaTipi")]
        
        public KcetasWeb.Models.Enums.OkumaTipi? okuma_tipi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("okumaKaynagi")]
        
        public KcetasWeb.Models.Enums.OkumaKaynagi? okuma_kaynagi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("oncekiEndeks")]
        public decimal? onceki_endeks { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("yeniEndeks")]
        public decimal? yeni_endeks { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("okumaZamani")]
        [System.Text.Json.Serialization.JsonConverter(typeof(KcetasWeb.Helpers.DateOnlyJsonConverter))]
        public DateTime? okuma_zamani { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("kullaniciId")]
        public int? kullanici_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("okunamamaNedeni")]
        public string? okunamama_nedeni { get; set; } 
        
        [System.Text.Json.Serialization.JsonPropertyName("dogrulamaDurumu")]
        
        public KcetasWeb.Models.Enums.DogrulamaDurumu? dogrulama_durumu { get; set; } 
        
        [System.Text.Json.Serialization.JsonPropertyName("anomaliMi")]
        public Boolean anomali_mi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string? status { get; set; }

        
        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime created_at { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedAt")]
        public DateTime? updated_at { get; set; }
       
    }
}
