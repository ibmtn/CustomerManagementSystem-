using System.ComponentModel.DataAnnotations;

namespace KcetasWeb.ViewModels
{
    public class FaturaSimulasyonViewModel
    {
        // ── Giriş Alanları ──
        [Required(ErrorMessage = "Tarife grubu zorunludur")]
        public string TarifeGrubu { get; set; } = null!;

        [Required(ErrorMessage = "Tüketim miktarı zorunludur")]
        [Range(0.01, 999999, ErrorMessage = "Tüketim miktarı 0.01 ile 999999 arasında olmalıdır")]
        public decimal TuketimMiktari { get; set; }

        public DateTime DonemBaslangic { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime DonemBitis { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        // ── Çıktı Alanları (Hesaplama Sonrası) ──
        public decimal? BirimFiyat { get; set; }
        public decimal? EnerjiBedeli { get; set; }
        public decimal? DagitimBedeli { get; set; }
        public decimal? TrtPayi { get; set; }
        public decimal? EnerjiFonu { get; set; }
        public decimal? KdvTutari { get; set; }
        public decimal? ToplamTutar { get; set; }

        // ── Kalem Detayları ──
        public List<SimulasyonKalemViewModel>? Kalemler { get; set; }

        // ── Hesaplanmış Özellik ──
        public bool HesaplamaTamamlandiMi => ToplamTutar.HasValue;

        // ── Kalem Modeli ──
        public class SimulasyonKalemViewModel
        {
            public string KalemAdi { get; set; } = null!;
            public decimal Miktar { get; set; }
            public decimal BirimFiyat { get; set; }
            public decimal Tutar { get; set; }
        }
    }
}
