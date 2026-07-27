using System;
using System.Collections.Generic;

namespace KcetasWeb.ViewModels
{
    public class BelgeSatirViewModel
    {
        public string BelgeTipi { get; set; } 
        public string BelgeNo { get; set; } 
        public string TuketimNoktasiKodu { get; set; }
        public DateTime Tarih { get; set; }
        public decimal? Tutar { get; set; } 
        public string Aciklama { get; set; } 
        public string Url { get; set; }
    }

    public class BelgelerListeViewModel : PaginationBaseViewModel
    {
        public DateTime? FiltreBelgeTarihi { get; set; }
        public string? FiltreBelgeNo { get; set; }
        public string? FiltreTekilKod { get; set; }
        public string AktifSekme { get; set; } = "Fatura";

        public List<BelgeSatirViewModel> Belgeler { get; set; } = new List<BelgeSatirViewModel>();
    }
}
