namespace KcetasWeb.Models
{
    using System;

    public class EndeksOkuma
    {
        [System.Text.Json.Serialization.JsonPropertyName("okumaId")]
        public long okuma_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sayacId")]
        public int? sayac_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("isEmriId")]
        public int? is_emri_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sozlesmeId")]
        public int? sozlesme_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("okumaTipi")]
        public string? okuma_tipi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("okumaKaynagi")]
        public string? okuma_kaynagi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("oncekiEndeks")]
        public decimal? onceki_endeks { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("yeniEndeks")]
        public decimal? yeni_endeks { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("okumaZamani")]
        public DateTime? okuma_zamani { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("kullaniciId")]
        public int? kullanici_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("okunamamaNedeni")]
        public string? okunamam_nedeni { get; set; } // Düzeltildi: decimal -> string
        
        [System.Text.Json.Serialization.JsonPropertyName("dogrulamaDurumu")]
        public string? dogrulama_durumu { get; set; } // Düzeltildi: decimal -> string
        
        [System.Text.Json.Serialization.JsonPropertyName("anomaliMi")]
        public bool? anomali_mi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string? status { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("anomaliAciklamasi")]
        public string? AnomaliAciklamasi { get; set; }
        public string sökme_nedeni {get; set;}
        public string aciklama {get; set;}
        public decimal son_endeks {get; set;}
        
        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
       
    }
}