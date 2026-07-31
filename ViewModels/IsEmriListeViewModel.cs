using System;
using System.Collections.Generic;

namespace KcetasWeb.ViewModels
{
    public class IsEmriListeViewModel
    {
        public string? FiltreTip { get; set; }
        public string? FiltreDurum { get; set; }
        public DateTime? FiltreOlusturmaTarihi { get; set; }
        public DateTime? FiltrePlanlananTarih { get; set; }
        public string? AramaMetni { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);// Yeni Filtre Alanları
        public string? FiltreIsEmriNo { get; set; }
        public string? FiltreTekilKod { get; set; }
        public string? FiltreAboneAdi { get; set; }
        public string? FiltrePersonel { get; set; }
        public string? FiltreOncelik { get; set; }
        

        

        public List<IsEmriSatirViewModel> IsEmirleri { get; set; } = new();

        public static string GetDurumRenk(KcetasWeb.Models.Enums.IsEmriDurumu durum) => durum switch
        {
            KcetasWeb.Models.Enums.IsEmriDurumu.Acik => "bg-light-primary text-primary border border-primary",
            KcetasWeb.Models.Enums.IsEmriDurumu.Atandi => "bg-light-info text-info border border-info",
            KcetasWeb.Models.Enums.IsEmriDurumu.Yolda => "bg-light-dark text-dark border border-dark",
            KcetasWeb.Models.Enums.IsEmriDurumu.Sahada => "bg-light-warning text-warning border border-warning",
            KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi => "bg-light-success text-success border border-success",
            KcetasWeb.Models.Enums.IsEmriDurumu.Iptal => "bg-danger text-white border border-danger",
            KcetasWeb.Models.Enums.IsEmriDurumu.Basarisiz => "bg-light-danger text-danger border border-danger",
            _ => "bg-light-secondary text-secondary border border-secondary"
        };

        public static string GetDurumAd(KcetasWeb.Models.Enums.IsEmriDurumu durum) => durum switch
        {
            KcetasWeb.Models.Enums.IsEmriDurumu.Acik => "Açık",
            KcetasWeb.Models.Enums.IsEmriDurumu.Atandi => "Atandı",
            KcetasWeb.Models.Enums.IsEmriDurumu.Yolda => "Yolda",
            KcetasWeb.Models.Enums.IsEmriDurumu.Sahada => "Sahada",
            KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi => "Tamamlandı",
            KcetasWeb.Models.Enums.IsEmriDurumu.Iptal => "İptal",
            KcetasWeb.Models.Enums.IsEmriDurumu.Basarisiz => "Başarısız",
            _ => durum.ToString()
        };

        public static string GetTipAd(KcetasWeb.Models.Enums.IsEmriTipi tip) => tip switch
        {
            KcetasWeb.Models.Enums.IsEmriTipi.Degistirme => "Sayaç Değiştirme",
            KcetasWeb.Models.Enums.IsEmriTipi.Sokme => "Sayaç Sökme",
            KcetasWeb.Models.Enums.IsEmriTipi.Kesme => "Enerji Kesme",
            KcetasWeb.Models.Enums.IsEmriTipi.Acma => "Enerji Açma",
            KcetasWeb.Models.Enums.IsEmriTipi.EndeksOkuma => "Endeks Okuma",
            KcetasWeb.Models.Enums.IsEmriTipi.SayacAriza => "Sayaç Arıza",
            KcetasWeb.Models.Enums.IsEmriTipi.Muhurleme => "Mühürleme",
            KcetasWeb.Models.Enums.IsEmriTipi.KesifInceleme => "Keşif İnceleme",
            KcetasWeb.Models.Enums.IsEmriTipi.YeniBaglanti => "Yeni Bağlantı",
            (KcetasWeb.Models.Enums.IsEmriTipi)1 => "Yeni Bağlama", // Handle legacy/missing 1 value
            _ => tip.ToString()
        };

        public static string NormalizeOncelikKey(string? oncelik)
        {
            if (string.IsNullOrWhiteSpace(oncelik)) return "NORMAL";

            return oncelik.Trim().ToUpperInvariant() switch
            {
                "ACİL" or "ACIL" => "ACIL",
                "YÜKSEK" or "YUKSEK" => "YUKSEK",
                "DÜŞÜK" or "DUSUK" => "DUSUK",
                _ => "NORMAL"
            };
        }

        public static string GetOncelikAd(string? oncelik) => NormalizeOncelikKey(oncelik) switch
        {
            "ACIL" => "Acil",
            "YUKSEK" => "Yüksek",
            "DUSUK" => "Düşük",
            _ => "Normal"
        };
    }

    public class IsEmriSatirViewModel
    {
        public long IsEmriId { get; set; }
        public string IsEmriNo { get; set; } = null!;
        public KcetasWeb.Models.Enums.IsEmriTipi Tip { get; set; }
        public long TuketimNoktasiId { get; set; }
        public string tekil_kod { get; set; }
        public string TuketimNoktasiKodu { get; set; } = null!;
        public DateTime? PlanlananTarih { get; set; }
        public DateTime olusturulma_tarihi {get; set;}
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string AtananKullaniciAdi { get; set; } = null!;
        public KcetasWeb.Models.Enums.IsEmriDurumu Durum { get; set; }
        public string DurumRenk { get; set; } = null!;
        public string Adres { get; set; } = null!;
        public string oncelik { get; set; }
        public string? musteri_ad { get; set; }
        public string? musteri_soyad { get; set; }
        public string? musteri_unvan { get; set; }
        

        public string musteriDurum => $"{musteri_ad} {musteri_soyad} {musteri_unvan}";
        
    }
}
