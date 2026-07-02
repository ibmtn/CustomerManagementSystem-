using System.ComponentModel.DataAnnotations;

namespace KcetasWeb.Models.entities;

public class AuditLog
{
    [Key]
    public int log_id { get; set; }

    [Required]
    [MaxLength(50)]
    public string islem_tipi { get; set; } = string.Empty; // Ekleme, Guncelleme, Silme

    [Required]
    [MaxLength(100)]
    public string tablo_adi { get; set; } = string.Empty;

    [Required]
    public long kayit_id { get; set; }

    public string? eski_deger { get; set; } // JSON

    public string? yeni_deger { get; set; } // JSON

    public long? kullanici_id { get; set; }

    [MaxLength(100)]
    public string? kullanici_adi { get; set; }

    [Required]
    public DateTime islem_zamani { get; set; } = DateTime.Now;

    [MaxLength(45)]
    public string? ip_adresi { get; set; }
}
