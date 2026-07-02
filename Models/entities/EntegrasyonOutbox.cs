using KcetasWeb.Models.enums;
using System.ComponentModel.DataAnnotations;

namespace KcetasWeb.Models.entities;

public class EntegrasyonOutbox
{
    [Key]
    public int outbox_id { get; set; }

    [Required]
    [MaxLength(50)]
    public string islem_tipi { get; set; } = string.Empty; // Fatura, Okuma, IsEmri

    [Required]
    public long referans_id { get; set; }

    [Required]
    [MaxLength(50)]
    public string referans_no { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string hedef_sistem { get; set; } = string.Empty; // SAP, MDMS, GIS vb.

    [Required]
    public string payload { get; set; } = "{}"; // JSON string

    [Required]
    public OutboxDurumu durum { get; set; } = OutboxDurumu.Bekliyor;

    public int deneme_sayisi { get; set; } = 0;

    [MaxLength(1000)]
    public string? son_hata_mesaji { get; set; }

    [Required]
    public DateTime olusturulma_zamani { get; set; } = DateTime.Now;

    public DateTime? gonderim_zamani { get; set; }

    public DateTime? sonraki_deneme_zamani { get; set; }
}
