namespace KcetasWeb.Models
{
    using System;

    public class Sayac
    {
        [System.Text.Json.Serialization.JsonPropertyName("sayacId")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public int sayac_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("seriNo")]
        public string seri_no { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tuketimNoktasiId")]
        public int? tuketim_noktasi_id { get; set; }
        
        public string marka { get; set; }
        public string model { get; set; }
        
        public KcetasWeb.Models.Enums.SayacFaz? faz { get; set; }
        public decimal carpan { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("muhurNo")]
        public string? muhur_no { get; set; }
        
        
        [System.Text.Json.Serialization.JsonPropertyName("durum")]
        
        public KcetasWeb.Models.Enums.SayacDurumu? durum { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string status { get; set; }

        
        [System.Text.Json.Serialization.JsonPropertyName("createdBy")]
        public int? created_by { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedBy")]
        public int? updated_by { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime created_at { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedAt")]
        public DateTime? updated_at { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("uretimYili")]
        public int uretim_yili { get; set; }
    }
}
