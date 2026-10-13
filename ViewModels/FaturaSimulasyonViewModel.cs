using System.ComponentModel.DataAnnotations;

namespace MusteriYonetim.ViewModels
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
        public string tekil_kod { get; set; }
        public long fatura_id { get; set; }
        public string? fatura_no { get; set; }
        public long? sozlesme_id { get; set; }
        public MusteriYonetim.Models.Enums.FaturaTipi? fatura_tipi { get; set; }
        public string? donem { get; set; }
        public DateOnly? fatura_tarihi { get; set; }
        public DateOnly? son_odeme_tarihi { get; set; }
        public int? okuma_id { get; set; }
        public decimal? ilk_endeks { get; set; }
        public decimal? son_endeks { get; set; }
        public decimal? tuketim_kwh { get; set; }
        public decimal? reaktif_enduktif { get; set; }
        public decimal? reaktif_kapasitif { get; set; }
        public decimal? carpan { get; set; }
        public decimal? enerji_bedeli { get; set; }
        public decimal? dagatim_bedeli { get; set; }
        public decimal? hizmet_bedeli { get; set; }
        public decimal? kesme_baglama_bedeli { get; set; }
        public decimal? vergi_fon_toplama { get; set; }
        public decimal? toplam_tutar { get; set; }
        public string? durum { get; set; }
        public string? status { get; set; }
        public string sozlesme_no { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        // ── Kalem Detayları ──
        public List<SimulasyonKalemViewModel>? Kalemler { get; set; }

        // ── Hesaplanmış Özellik ──
        public bool HesaplamaTamamlandiMi => ToplamTutar.HasValue;
        public long fatura_kalemi_id { get; set; }
        
        public string? kalem_tipi { get; set; } // KDV, Enerji Fonu, TRT Payı vb.
        public string? aciklama { get; set; }
        public decimal? miktar { get; set; }
        public decimal? birim_fiyati { get; set; }
        public decimal? tutar { get; set; }
        
       

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
