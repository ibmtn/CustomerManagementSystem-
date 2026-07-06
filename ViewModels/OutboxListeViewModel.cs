using System;
using System.Collections.Generic;

namespace KcetasWeb.ViewModels
{
    public class OutboxListeViewModel
    {
        public string? FiltreDurum { get; set; }
        public string? FiltreIslemTipi { get; set; }
        public DateTime? BaslangicTarih { get; set; }
        public DateTime? BitisTarih { get; set; }

        public List<OutboxSatirViewModel> Kayitlar { get; set; } = new();

        public int ToplamKayit { get; set; }
        public int BekleyenSayisi { get; set; }
        public int GonderilmisSayisi { get; set; }
        public int BasarisizSayisi { get; set; }

        public class OutboxSatirViewModel
        {
            public long OutboxId { get; set; }
            public string IslemTipi { get; set; } = null!;
            public string ReferansNo { get; set; } = null!;
            public string HedefSistem { get; set; } = null!;
            public string Durum { get; set; } = null!;
            public string DurumRenk { get; set; } = null!;
            public int DenemeSayisi { get; set; }
            public string? SonHataMesaji { get; set; }
            public DateTime OlusturulmaZamani { get; set; }
            public DateTime? GonderimZamani { get; set; }
            public string PayloadOnizleme { get; set; } = null!;
        }

        public static string GetOutboxDurumRenk(string durum) => durum switch
        {
            "Bekliyor" => "warning",
            "Gönderildi" => "success",
            "Başarısız" => "danger",
            "İptal Edildi" => "secondary",
            _ => "dark"
        };
    }
}
