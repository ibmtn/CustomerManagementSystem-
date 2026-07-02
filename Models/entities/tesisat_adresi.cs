using KcetasWeb.Models.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace KcetasWeb.Models.entities;

public class tesisat_adresi
{

    [Key]
    public int is_emri_id { get; set; }


    [Required]
    [MaxLength(40)]
    public string is_emri_no { get; set; }

    [Required]
    [MaxLength(40)]
    public long tuketim_noktasi_id { get; set; }

    [Required]
    [MaxLength(40)]
    public long sayac_id { get; set; }

    [Required]
    [MaxLength(40)]
    public long okuma_id { get; set; }

    [Required]
    [MaxLength(40)]
    public long fatura_id { get; set; }

    [Required]
    [MaxLength(30)]
    public string tip {get; set;}

    [Required]
    [MaxLength(10)]
    public string oncelik {get; set;}

    [Required]
    [MaxLength(40)]
    public DateTime planlanan_tarih { get; set; }

    [Required]
    [MaxLength(40)]
    public long atanan_kullanici_id { get; set; }

    [Required]
    [MaxLength(20)]
    public string durum {get; set;}

    [Required]
    [MaxLength(255)]
    public string saha_sonucu {get; set;}

    [Required]
    [MaxLength(255)]
    public string gerekce {get; set;}

    [Required]
    [MaxLength(255)]
    public string eski_sayac_no  {get; set;}

    [Required]
    [MaxLength(255)]
    public decimal eski_son_endeksi {get; set;}

    [Required]
    [MaxLength(255)]
    public string yeni_sayac_no {get; set;}

    [Required]
    [MaxLength(255)]
    public string yeni_ilk_endeksi {get; set;}

    [Required]
    [MaxLength(255)]
    public string muhur_no {get; set;}

    [Required]
    [MaxLength(255)]
    public string tutanak_no {get; set;}

    [Required]
    [MaxLength(255)]
    public string kesme_endeksi {get; set;}

    [Required]
    [MaxLength(255)]
    public string status {get; set;}

    [Required]
    [MaxLength(255)]
    public DateTime created_at {get; set;}

    [Required]
    [MaxLength(255)]
    public DateTime updated_at {get; set;}

}

