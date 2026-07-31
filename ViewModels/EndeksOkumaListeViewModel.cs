namespace KcetasWeb.ViewModels
{
    using System;
    using System.Collections.Generic;

    public class EndeksOkumaListeViewModel : PaginationBaseViewModel
    {
        public string? FiltreKaynak { get; set; }
        public string? FiltreDurum { get; set; }
        public DateTime? BaslangicTarih { get; set; }
        public DateTime? BitisTarih { get; set; }
        public string? AramaMetni { get; set; }
        
        public string? FiltreSayacId { get; set; }
        public string? FiltreTuketimNoktasi { get; set; }
        public string? FiltreDonem { get; set; }
        public string? FiltreDogrulamaDurumu { get; set; }
        public DateTime? FiltreOkumaTarihi { get; set; }
        public string? FiltreAbone { get; set; }
        public long? FiltreOkumaNo { get; set; }

        public List<OkumaSatirViewModel> Okumalar { get; set; } = new();

        public int ToplamOkuma { get; set; }
        public int ManuelOkuma { get; set; }
        public int OSOSOkuma { get; set; }
        public int AnomaliSayisi { get; set; }
        public decimal OrtalamaTuketim { get; set; }

        public class OkumaSatirViewModel
        {
            public long OkumaId { get; set; }
            public string TuketimNoktasiKodu { get; set; } = null!;
            public string SayacSeriNo { get; set; } = null!;
            public DateTime OkumaTarihi { get; set; }
            public decimal OncekiEndeks { get; set; }
            public decimal GuncelEndeks { get; set; }
            public decimal TuketimMiktari { get; set; }
            public KcetasWeb.Models.Enums.OkumaKaynagi? Kaynak { get; set; }
            public string Durum { get; set; } = null!;
            public string DurumRenk { get; set; } = null!;
            public bool DogrulamaDurumu { get; set; }
            public string? AnomaliAciklamasi { get; set; }
            public string? TarifeGrubu { get; set; }
            public string AboneBilgisi { get; set; } = null!;
            public KcetasWeb.Models.Enums.OkumaTipi? OkumaTipi { get; set; }
        }

        public static string GetOkumaDurumRenk(string durum) => durum switch
        {
            "Basarili" => "success",
            "Hatali" => "danger",
            "Sifir Tuketim" => "warning",
            "Anormal" => "danger",
            _ => "dark"
        };
    }
}
