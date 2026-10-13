using System;

namespace MusteriYonetim.ViewModels
{
    public class BelgeGoruntuleViewModel
    {
        public string BelgeTipi { get; set; } // Fatura, Sozlesme, Tutanak
        public string BelgeNo { get; set; }
        public DateTime Tarih { get; set; }
        
        // Ortak Alanlar
        public string AboneBilgisi { get; set; }
        public string TuketimNoktasiKod { get; set; }

        // Fatura Özel Alanlar
        public decimal? FaturaTutar { get; set; }
        public decimal? FaturaTuketim { get; set; }
        public string FaturaDurum { get; set; }

        // Sozlesme Özel Alanlar
        public string SozlesmeTipi { get; set; }
        public decimal? GuvenceBedeli { get; set; }
        public string SozlesmeDurum { get; set; }

        // Tutanak Özel Alanlar
        public string TutanakIslemTipi { get; set; }
        public string TutanakSahaSonucu { get; set; }
        public string TutanakGerekce { get; set; }
        public string TutanakDurum { get; set; }
    }
}
