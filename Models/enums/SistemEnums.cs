using System.ComponentModel.DataAnnotations;

namespace MusteriYonetim.Models.Enums
{
    public enum KullaniciDurumu
    {
        [Display(Name = "Aktif")]
        Aktif = 1,
        [Display(Name = "Pasif")]
        Pasif = 2
    }

    public enum OutboxDurumu
    {
        [Display(Name = "Bekliyor")]
        Bekliyor = 1,
        [Display(Name = "Gönderildi")]
        Gonderildi = 2,
        [Display(Name = "Hata Alındı")]
        Hata = 3,
        [Display(Name = "Manuel Müdahale Gerekiyor")]
        ManuelMudahale = 4
    }

    public enum HedefSistem
    {
        [Display(Name = "GİB e-Fatura")]
        GibEFatura = 1,
        [Display(Name = "GİB e-Arşiv")]
        GibEArsiv = 2,
        [Display(Name = "ERP Sistemi")]
        Erp = 3,
        [Display(Name = "CRM Bildirimi")]
        CrmNotification = 4,
        [Display(Name = "Saha Yazıcı (Field Print)")]
        FieldPrint = 5
    }
}
