using System.ComponentModel.DataAnnotations;

namespace MusteriYonetim.Models.Enums
{
    public enum SozlesmeDurumu
    {
        [Display(Name = "Aktif")]
        Aktif = 1,
        [Display(Name = "Güvence Bekliyor")]
        GuvenceBekliyor = 2,
        [Display(Name = "Feshedildi")]
        Feshedildi = 3,
        [Display(Name = "Pasif")]
        Pasif = 4
    }

    public enum SozlesmeTipi
    {
        [Display(Name = "Bireysel")]
        Bireysel = 1,
        [Display(Name = "Kurumsal")]
        Kurumsal = 2
    }

    public enum SayacFaz
    {
        [Display(Name = "Monofaze")]
        Monofaze = 1,
        [Display(Name = "Trifaze")]
        Trifaze = 2
    }

    public enum SayacDurumu
    {
        [Display(Name = "Depoda")]
        Depoda = 1,
        [Display(Name = "Bağlı")]
        Bagli = 2,
        [Display(Name = "Arızalı")]
        Arizali = 3,
        [Display(Name = "TAKILI")]
        Takili = 4
    }

    public enum AuditIslemTipi
    {
        [Display(Name = "Ekleme")]
        Ekleme = 1,
        [Display(Name = "Güncelleme")]
        Guncelleme = 2,
        [Display(Name = "Silme")]
        Silme = 3,
        [Display(Name = "Giriş")]
        Giris = 4
    }
}
