using KcetasWeb.Models.enums;

namespace KcetasWeb.ViewModels
{
    public class EndeksOkumaListeViewModel
    {
        // ── Filtre Alanları ──
        public OkumaKaynagi? FiltreKaynak { get; set; }
        public OkumaDurumu? FiltreDurum { get; set; }
        public DateTime? BaslangicTarih { get; set; }
        public DateTime? BitisTarih { get; set; }
        public string? AramaMetni { get; set; }

        // ── Liste ──
        public List<OkumaSatirViewModel> Okumalar { get; set; } = new();

        // ── İstatistikler ──
        public int ToplamOkuma { get; set; }
        public int ManuelOkuma { get; set; }
        public int OSOSOkuma { get; set; }
        public int AnomaliSayisi { get; set; }
        public decimal OrtalamaTuketim { get; set; }

        // ── Satır Modeli ──
        public class OkumaSatirViewModel
        {
            public int OkumaId { get; set; }
            public string TuketimNoktasiKodu { get; set; } = null!;
            public string SayacSeriNo { get; set; } = null!;
            public DateTime OkumaTarihi { get; set; }
            public decimal OncekiEndeks { get; set; }
            public decimal GuncelEndeks { get; set; }
            public decimal TuketimMiktari { get; set; }
            public OkumaKaynagi Kaynak { get; set; }
            public string KaynakAdi { get; set; } = null!;
            public OkumaDurumu Durum { get; set; }
            public string DurumAdi { get; set; } = null!;
            public string DurumRenk { get; set; } = null!;
            public bool DogrulamaDurumu { get; set; }
            public string? AnomaliAciklamasi { get; set; }
            public string? TarifeGrubu { get; set; }
        }

        // ── Yardımcı Metotlar ──
        public static string GetKaynakAdi(OkumaKaynagi kaynak) => kaynak switch
        {
            OkumaKaynagi.Manuel => "Manuel",
            OkumaKaynagi.OSOS => "OSOS",
            OkumaKaynagi.Tahmin => "Tahmin",
            _ => "Bilinmiyor"
        };

        public static string GetOkumaDurumAdi(OkumaDurumu durum) => durum switch
        {
            OkumaDurumu.Basarili => "Başarılı",
            OkumaDurumu.Hatali => "Hatalı",
            OkumaDurumu.SifirTuketim => "Sıfır Tüketim",
            OkumaDurumu.Anormal => "Anormal",
            _ => "Bilinmiyor"
        };

        public static string GetOkumaDurumRenk(OkumaDurumu durum) => durum switch
        {
            OkumaDurumu.Basarili => "success",
            OkumaDurumu.Hatali => "danger",
            OkumaDurumu.SifirTuketim => "warning",
            OkumaDurumu.Anormal => "danger",
            _ => "dark"
        };
    }
}
