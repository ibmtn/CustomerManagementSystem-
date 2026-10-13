using System.Collections.Generic;

namespace MusteriYonetim.ViewModels
{
    public class FaturaListeViewModel : PaginationBaseViewModel
    {
        public string? FiltreFaturaNo { get; set; }
        public string? FiltreTekilKod { get; set; }
        public string? FiltreDonem { get; set; }
        public string? FiltreDurum { get; set; }
        public string? FiltreSozlesmeNo { get; set; }
        public string sozlesme_no { get; set; }

        public List<FaturaSimulasyonViewModel> Faturalar { get; set; } = new();
    }
}
