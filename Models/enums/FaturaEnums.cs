using System.ComponentModel.DataAnnotations;

namespace MusteriYonetim.Models.Enums
{
    public enum FaturaTipi
    {
        [Display(Name = "Dönem (Rutin)")]
        Donem = 1,
        [Display(Name = "Kapanış")]
        Kapanis = 2,
        [Display(Name = "Ara Fatura")]
        Ara = 3,
        [Display(Name = "Düzeltme")]
        Duzeltme = 4,
        [Display(Name = "İptal")]
        Iptal = 5
    }

    public enum FaturaKalemTipi
    {
        [Display(Name = "Aktif Enerji Bedeli")]
        EnerjiBedeli = 1,
        [Display(Name = "Dağıtım Bedeli")]
        DagitimBedeli = 2,
        [Display(Name = "Hizmet Bedeli")]
        Hizmet = 3,
        [Display(Name = "Kesme/Bağlama Bedeli")]
        KesmeBaglama = 4,
        [Display(Name = "Gecikme Zammı")]
        Gecikme = 5,
        [Display(Name = "Yuvarlama Farkı")]
        Yuvarlama = 6,
        [Display(Name = "Vergi/Fon (KDV vb.)")]
        VergiFon = 7
    }
}
