using KcetasWeb.Models.enums;

namespace KcetasWeb.ViewModels
{
    public class IsEmriDetayViewModel
    {
        // ── Temel Bilgiler ──
        public int IsEmriId { get; set; }
        public string IsEmriNo { get; set; } = null!;
        public IsEmriTipi Tip { get; set; }
        public string TipAdi { get; set; } = null!;
        public IsEmriDurumu Durum { get; set; }
        public string DurumAdi { get; set; } = null!;
        public string DurumRenk { get; set; } = null!;

        // ── Planlama & Atama ──
        public string? Oncelik { get; set; }
        public DateTime PlanlananTarih { get; set; }
        public string? AtananKullaniciAdi { get; set; }

        // ── Tüketim Noktası ──
        public string? TuketimNoktasiKodu { get; set; }
        public string? Adres { get; set; }
        public string? SayacSeriNo { get; set; }

        // ── Saha Sonuçları ──
        public string? SahaSonucu { get; set; }
        public string? Gerekce { get; set; }

        // ── Sökme / Takma Alanları ──
        public string? EskiSayacNo { get; set; }
        public decimal? EskiSonEndeksi { get; set; }
        public string? YeniSayacNo { get; set; }
        public decimal? YeniIlkEndeksi { get; set; }
        public string? MuhurNo { get; set; }

        // ── Açma / Kesme Alanları ──
        public string? TutanakNo { get; set; }
        public decimal? KesmeEndeksi { get; set; }
        public decimal? AcmaEndeksi { get; set; }

        // ── Tarihler ──
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // ── Hesaplanmış Özellikler ──
        public bool TutanakGirildiMi => !string.IsNullOrEmpty(TutanakNo);
        public bool IsSokmeTakma => Tip is IsEmriTipi.SayacSokme or IsEmriTipi.SayacTakma or IsEmriTipi.SayacDegisim;
        public bool IsAcmaKesme => Tip is IsEmriTipi.Acma or IsEmriTipi.Kesme;
    }
}
