namespace KcetasWeb.Models
{
    using System;

    public class AuditLog
    {
        [System.Text.Json.Serialization.JsonPropertyName("auditId")]
        public int audit_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("varlikTipi")]
        public string varlik_tipi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("varlikId")]
        public int varlik_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("islemTipi")]
        public KcetasWeb.Models.Enums.AuditIslemTipi? islem_tipi { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("eskiDeger")]
        public string eski_deger { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("yeniDeger")]
        public string yeni_deger { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("kullaniciId")]
        public int kullanici_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("kullaniciAdi")]
        public string? kullanici_adi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("islemGerekcesi")]
        public string islem_gerekcesi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("islemZamani")]
        public DateTime islem_zamani { get; set; }
        
    }
}
