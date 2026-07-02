using KcetasWeb.Models.enums;

namespace KcetasWeb.ViewModels
{
    public class IsEmriListeViewModel
    {
        // ── Filtre Alanları ──
        public IsEmriTipi? FiltreTip { get; set; }
        public IsEmriDurumu? FiltreDurum { get; set; }
        public DateTime? BaslangicTarih { get; set; }
        public DateTime? BitisTarih { get; set; }
        public string? AramaMetni { get; set; }

        // ── Liste ──
        public List<IsEmriSatirViewModel> IsEmirleri { get; set; } = new();

        // ── Satır Modeli ──
        public class IsEmriSatirViewModel
        {
            public int IsEmriId { get; set; }
            public string IsEmriNo { get; set; } = null!;
            public IsEmriTipi Tip { get; set; }
            public string TipAdi { get; set; } = null!;
            public long TuketimNoktasiId { get; set; }
            public string TuketimNoktasiKodu { get; set; } = null!;
            public DateTime PlanlananTarih { get; set; }
            public string? AtananKullaniciAdi { get; set; }
            public IsEmriDurumu Durum { get; set; }
            public string DurumAdi { get; set; } = null!;
            public string DurumRenk { get; set; } = null!;
            public string? Adres { get; set; }
        }

        // ── Yardımcı Metotlar ──
        public static string GetTipAdi(IsEmriTipi tip) => tip switch
        {
            IsEmriTipi.SayacSokme => "Sayaç Sökme",
            IsEmriTipi.SayacTakma => "Sayaç Takma",
            IsEmriTipi.SayacDegisim => "Sayaç Değişim",
            IsEmriTipi.Acma => "Açma",
            IsEmriTipi.Kesme => "Kesme",
            IsEmriTipi.KontrolMuayene => "Kontrol / Muayene",
            _ => "Bilinmiyor"
        };

        public static string GetDurumAdi(IsEmriDurumu durum) => durum switch
        {
            IsEmriDurumu.Olusturuldu => "Oluşturuldu",
            IsEmriDurumu.EkibeAtandi => "Ekibe Atandı",
            IsEmriDurumu.DevamEdiyor => "Devam Ediyor",
            IsEmriDurumu.Tamamlandi => "Tamamlandı",
            IsEmriDurumu.IptalEdildi => "İptal Edildi",
            IsEmriDurumu.Durduruldu => "Durduruldu",
            _ => "Bilinmiyor"
        };

        public static string GetDurumRenk(IsEmriDurumu durum) => durum switch
        {
            IsEmriDurumu.Olusturuldu => "secondary",
            IsEmriDurumu.EkibeAtandi => "info",
            IsEmriDurumu.DevamEdiyor => "primary",
            IsEmriDurumu.Tamamlandi => "success",
            IsEmriDurumu.IptalEdildi => "danger",
            IsEmriDurumu.Durduruldu => "warning",
            _ => "dark"
        };
    }
}
