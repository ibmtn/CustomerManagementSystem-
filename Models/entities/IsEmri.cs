namespace KcetasWeb.Models
{
    using System;

    public class IsEmri
    {
        [System.Text.Json.Serialization.JsonPropertyName("isEmriId")]
        public long is_emri_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public long id { get => is_emri_id; set => is_emri_id = value; }

        [System.Text.Json.Serialization.JsonPropertyName("isEmriNo")]
        public string is_emri_no { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tuketimNoktasiId")]
        public long tuketim_noktasi_id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("sayacId")]
        public long? sayac_id { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("tip")]
        public string tip { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("oncelik")]
        public string oncelik { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("planlananTarih")]
        public DateTime? planlanan_tarih { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("atananKullaniciId")]
        public long? atanan_kullanici_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("durum")]
        public string durum { get; set; }
        
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
        
        [System.Text.Json.Serialization.JsonPropertyName("kesmeEndeksi")]
        public decimal? kesme_endeksi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("acmaEndeksi")]
        public decimal? acma_endeksi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("eskiSayacNo")]
        public string? eski_sayac_no { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("yeniSayacNo")]
        public string? yeni_sayac_no { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("eskiSonEndeksi")]
        public decimal? eski_son_endeksi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("yeniIlkEndeksi")]
        public decimal? yeni_ilk_endeksi { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}