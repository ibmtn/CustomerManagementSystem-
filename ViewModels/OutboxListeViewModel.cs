using System;
using System.Collections.Generic;

namespace KcetasWeb.ViewModels
{
    public class OutboxListeViewModel : PaginationBaseViewModel
    {
        public string? FiltreDurum { get; set; }
        public string? FiltreHedefSistem { get; set; }
        public string? FiltreFaturaNo { get; set; }
        public long? FiltreKayitNo { get; set; }
        public DateTime? OlusturmaTarihi { get; set; }
        public DateTime? SonDenemeTarihi { get; set; }

        public List<OutboxSatirViewModel> Kayitlar { get; set; } = new();

        public int ToplamKayit { get; set; }
        public int BekleyenSayisi { get; set; }
        public int GonderilmisSayisi { get; set; }
        public int BasarisizSayisi { get; set; }

        public class OutboxSatirViewModel
        {
            public long OutboxId { get; set; }
            public int FaturaId { get; set; }
            public string? FaturaNo { get; set; }
            public string? ReferansNo { get; set; }
            public string? HedefSistem { get; set; }
            public string? IdempotencyKey { get; set; }
            public string? Durum { get; set; }
            public string DurumEtiketi { get; set; } = null!;
            public string DurumRenk { get; set; } = null!;
            public int DenemeSayisi { get; set; }
            public string? HataKodu { get; set; }
            public string? SonHataMesaji { get; set; }
            public DateTime OlusturulmaZamani { get; set; }
            public DateTime? SonDenemeTarihi { get; set; }
            public DateTime? GonderimZamani { get; set; }
            public string? PayloadOnizleme { get; set; }

            public bool YenidenGonderilebilir =>
                string.Equals(Durum, "BASARISIZ", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Durum, "HATALI", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Durum, "HATA", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Durum, "Basarisiz", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetOutboxDurumRenk(string? durum) => NormalizeDurum(durum) switch
        {
            "BEKLIYOR" => "warning",
            "GONDERILDI" => "success",
            "BASARISIZ" => "danger",
            "HATALI" => "danger",
            "HATA" => "danger",
            "MANUEL_MUDAHALE" => "primary",
            "MANUELMUDAHALE" => "primary",
            "IPTAL" => "secondary",
            _ => "dark"
        };

        public static string GetOutboxDurumEtiketi(string? durum) => NormalizeDurum(durum) switch
        {
            "BEKLIYOR" => "Bekliyor",
            "GONDERILDI" => "Gönderildi",
            "BASARISIZ" => "Başarısız",
            "HATALI" => "Hatalı",
            "HATA" => "Hata",
            "MANUEL_MUDAHALE" => "Manuel Müdahale",
            "MANUELMUDAHALE" => "Manuel Müdahale",
            "IPTAL" => "İptal Edildi",
            "" => "-",
            _ => durum ?? "-"
        };

        public static string NormalizeDurum(string? durum)
        {
            if (string.IsNullOrWhiteSpace(durum))
                return "";

            return durum.Trim().ToUpperInvariant()
                .Replace("ı", "I")
                .Replace("İ", "I")
                .Replace("Ğ", "G")
                .Replace("Ü", "U")
                .Replace("Ş", "S")
                .Replace("Ö", "O")
                .Replace("Ç", "C")
                .Replace(" ", "_")
                .Replace("BASARILI", "GONDERILDI")
                .Replace("GONDERILMIS", "GONDERILDI")
                .Replace("IPTAL_EDILDI", "IPTAL");
        }
    }
}
