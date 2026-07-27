namespace KcetasWeb.Models
{
    using System;

    public class IsEmri
    {
        [System.Text.Json.Serialization.JsonPropertyName("isEmriId")]
        public int is_emri_id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("isEmriNo")]
        public string is_emri_no { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tuketimNoktasiId")]
        public int tuketim_noktasi_id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tuketimNoktasi")]
        public KcetasWeb.Models.TuketimNoktasi? TuketimNoktasi { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("sayacId")]
        public int? sayac_id { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("tip")]
        public KcetasWeb.Models.Enums.IsEmriTipi tip { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("oncelik")]
        public string oncelik { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("planlananTarih")]
        public DateTime? planlanan_tarih { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("atananKullaniciId")]
        public long? atanan_kullanici_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("durum")]
        public KcetasWeb.Models.Enums.IsEmriDurumu durum { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sahaSonucu")]
        public string saha_sonucu { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("gerekce")]
        public string gerekce { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("muhurNo")]
        public string muhur_no { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tutanakNo")]
        public string tutanak_no { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string status { get; set; }


        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime created_at { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedAt")]
        public DateTime? updated_at { get; set; }

    }
}