using System.ComponentModel.DataAnnotations;

namespace MusteriYonetim.Models.Enums
{
    public enum DogrulamaDurumu
    {
        [Display(Name = "Planlandı")]
        Planlandi = 1,
        [Display(Name = "Okundu")]
        Okundu = 2,
        [Display(Name = "Doğrulama Bekliyor")]
        DogrulamaBekliyor = 3,
        [Display(Name = "Onaylandı")]
        Onaylandi = 4,
        [Display(Name = "Reddedildi")]
        Reddedildi = 5,
        [Display(Name = "Tahakkuka Aktarıldı")]
        TahakkukaAktarildi = 6
    }

    public enum OkumaTipi
    {
        [Display(Name = "Rutin Dönem")]
        RutinDonem = 1,
        [Display(Name = "İlk Okuma (Açılış)")]
        IlkOkuma = 2,
        [Display(Name = "Son Okuma (Kapanış)")]
        SonOkuma = 3,
        [Display(Name = "Kesme Endeksi")]
        KesmeEndeksi = 4,
        [Display(Name = "Sayaç Değişim Okuması")]
        SayacDegisimOkumasi = 5,
        [Display(Name = "Sayaç Arıza Okuması")]
        SayacArizaOkumasi = 6,
        [Display(Name = "Mühürleme Endeksi")]
        MuhurlemeEndeksi = 7,
        [Display(Name = "Kontrol Okuması")]
        KontrolOkumasi = 8
    }

    public enum OkumaKaynagi
    {
        [Display(Name = "Manuel (Saha)")]
        Manuel = 1,
        [Display(Name = "OSOS")]
        Osos = 2,
        [Display(Name = "Düzeltme")]
        Duzeltme = 3,
        [Display(Name = "Müşteri Bildirimi")]
        MusteriBildirimi = 4
    }
}
