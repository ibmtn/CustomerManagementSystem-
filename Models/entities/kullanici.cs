using System;

namespace KcetasWeb.Models.entities
{
    public class Kullanici
    {
        [System.Text.Json.Serialization.JsonPropertyName("kullaniciId")]
        public long kullanici_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("adSoyad")]
        public string? ad_soyad { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("kullaniciAdi")]
        public string? kullanici_adi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("ePosta")]
        public string? e_posta { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sifreHash")]
        public string? sifre_hash { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("rolId")]
        public short? rol_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("durum")]
        public string? durum { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime? created_at { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedAt")]
        public DateTime? updated_at { get; set; }

        public Rol? Rol { get; set; }
        public string? AboneTuru { get; set; }
    }
}