using System.ComponentModel.DataAnnotations;

namespace KcetasWeb.Models.Enums
{
    public enum IsEmriDurumu
    {
        [Display(Name = "Açık")]
        Acik = 1,
        [Display(Name = "Atandı")]
        Atandi = 2,
        [Display(Name = "Yolda")]
        Yolda = 3,
        [Display(Name = "Sahada")]
        Sahada = 4,
        [Display(Name = "Tamamlandı")]
        Tamamlandi = 5,
        [Display(Name = "İptal")]
        Iptal = 6,
        [Display(Name = "Başarısız")]
        Basarisiz = 7
    }

    public enum IsEmriTipi
    {

        [Display(Name = "Sayaç Değiştirme")]
        Degistirme = 2,
        [Display(Name = "Sayaç Sökme")]
        Sokme = 3,
        [Display(Name = "Enerji Açma")]
        Acma = 4,
        [Display(Name = "Enerji Kesme")]
        Kesme = 5,
        [Display(Name = "Endeks Okuma")]
        EndeksOkuma = 6,
        [Display(Name = "Sayaç Arıza")]
        SayacAriza = 7,
        [Display(Name = "Mühürleme")]
        Muhurleme = 8,
        [Display(Name = "Keşif/İnceleme")]
        KesifInceleme = 9,
        [Display(Name = "Yeni Bağlantı")]
        YeniBaglanti = 10
    }
}
