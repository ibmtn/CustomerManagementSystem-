using System.ComponentModel.DataAnnotations;

namespace MusteriYonetim.Models.Enums
{
    public enum BaglantiDurumu
    {
        [Display(Name = "Taslak")]
        Taslak = 1,
        [Display(Name = "Bağlantı Bekliyor")]
        BaglantiBekliyor = 2,
        [Display(Name = "Bağlanabilir")]
        Baglanabilir = 3,
        [Display(Name = "Aktif")]
        Aktif = 4,
        [Display(Name = "Pasif (Kesik)")]
        Pasif = 5,
        [Display(Name = "Kapalı (Sökülü)")]
        Kapali = 6
    }

    public enum AboneTipi
    {
        [Display(Name = "Bireysel")]
        Bireysel = 1,
        [Display(Name = "Kurumsal")]
        Kurumsal = 2
    }

    public enum FazTipi
    {
        [Display(Name = "Tek Faz (Monofaze)")]
        TekFaz = 1,
        [Display(Name = "Üç Faz (Trifaze)")]
        UcFaz = 2
    }
}
