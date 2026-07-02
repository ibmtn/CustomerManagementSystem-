using System.ComponentModel.DataAnnotations;
using KcetasWeb.Models.enums;

namespace KcetasWeb.ViewModels
{
    public class TutanakGirisViewModel
    {
        // ── Görüntüleme Alanları (Read-Only) ──
        public int IsEmriId { get; set; }
        public string IsEmriNo { get; set; } = null!;
        public IsEmriTipi Tip { get; set; }
        public string TipAdi { get; set; } = null!;

        // ── Zorunlu Giriş Alanları ──
        [Required(ErrorMessage = "Tutanak numarası zorunludur")]
        public string TutanakNo { get; set; } = null!;

        [Required(ErrorMessage = "Saha sonucu zorunludur")]
        public string SahaSonucu { get; set; } = null!;

        public string? Gerekce { get; set; }

        [Required]
        public DateTime IslemTarihi { get; set; } = DateTime.Now;

        // ── Sökme / Takma Alanları ──
        public string? EskiSayacNo { get; set; }
        public decimal? EskiSonEndeksi { get; set; }
        public string? YeniSayacNo { get; set; }
        public decimal? YeniIlkEndeksi { get; set; }
        public string? MuhurNo { get; set; }

        // ── Açma / Kesme Alanları ──
        public decimal? KesmeEndeksi { get; set; }
        public decimal? AcmaEndeksi { get; set; }

        // ── Hesaplanmış Özellikler ──
        public bool IsSokmeTakma => Tip is IsEmriTipi.SayacSokme or IsEmriTipi.SayacTakma or IsEmriTipi.SayacDegisim;
        public bool IsAcmaKesme => Tip is IsEmriTipi.Acma or IsEmriTipi.Kesme;
    }
}
