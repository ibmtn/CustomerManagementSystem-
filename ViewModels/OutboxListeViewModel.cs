using KcetasWeb.Models.enums;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
namespace KcetasWeb.ViewModels
{
    public class OutboxListeViewModel
    {
        // ── Filtre Alanları ──
        public OutboxDurumu? FiltreDurum { get; set; }
        public string? FiltreIslemTipi { get; set; }
        public DateTime? BaslangicTarih { get; set; }
        public DateTime? BitisTarih { get; set; }

        // ── Liste ──
        public List<OutboxSatirViewModel> Kayitlar { get; set; } = new();

        // ── İstatistikler ──
        public int ToplamKayit { get; set; }
        public int BekleyenSayisi { get; set; }
        public int GonderilmisSayisi { get; set; }
        public int BasarisizSayisi { get; set; }

        // ── Satır Modeli ──
        public class OutboxSatirViewModel
        {
            public int OutboxId { get; set; }
            public string IslemTipi { get; set; } = null!;
            public string ReferansNo { get; set; } = null!;
            public string HedefSistem { get; set; } = null!;
            public OutboxDurumu Durum { get; set; }
            public string DurumAdi { get; set; } = null!;
            public string DurumRenk { get; set; } = null!;
            public int DenemeSayisi { get; set; }
            public string? SonHataMesaji { get; set; }
            public DateTime OlusturulmaZamani { get; set; }
            public DateTime? GonderimZamani { get; set; }
            public string PayloadOnizleme { get; set; } = null!;
        }

        // ── Yardımcı Metotlar ──
        public static string GetOutboxDurumAdi(OutboxDurumu durum) => durum switch
        {
            OutboxDurumu.Bekliyor => "Bekliyor",
            OutboxDurumu.Gonderildi => "Gönderildi",
            OutboxDurumu.Basarisiz => "Başarısız",
            OutboxDurumu.IptalEdildi => "İptal Edildi",
            _ => "Bilinmiyor"
        };

        public static string GetOutboxDurumRenk(OutboxDurumu durum) => durum switch
        {
            OutboxDurumu.Bekliyor => "warning",
            OutboxDurumu.Gonderildi => "success",
            OutboxDurumu.Basarisiz => "danger",
            OutboxDurumu.IptalEdildi => "secondary",
            _ => "dark"
        };
    }
}
