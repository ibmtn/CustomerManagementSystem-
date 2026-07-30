namespace KcetasWeb.Models
{
    using System;

    public class TuketimNoktasi
    {
        [System.Text.Json.Serialization.JsonPropertyName("tuketimNoktasiId")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public int tuketim_noktasi_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tekilKod")]
        public string tekil_kod { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("ilceId")]
        public int ilce_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("mahalle")]
        public string? mahalle { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("binaNo")]
        public string? bina_no { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("bagimsizBolumNo")]
        public string? bagimsiz_bolum_no { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("acikAdres")]
        public string? acik_adres { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("koordinatLat")]
        public decimal? koordinat_lat { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("koordinatLon")]
        public decimal? koordinat_lot { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("baglantiGucuKw")]
        public decimal baglanti_gucu_kw { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tuketiciGrubu")]
        public string tuketici_grubu { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("baglantiDurumu")]
        
        public KcetasWeb.Models.Enums.BaglantiDurumu? baglanti_durumu { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string? status { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime created_at { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedAt")]
        public DateTime? updated_at { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("createdBy")]
        public int? created_by { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedBy")]
        public int? updated_by { get; set; }
   
        // MVP Eksikleri:
        [System.Text.Json.Serialization.JsonIgnore]
        public decimal BaglantiGucuKw { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Enlem { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Boylam { get; set; }
    }


    
        

        
}
