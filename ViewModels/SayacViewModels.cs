using KcetasWeb.Models;
using System.Collections.Generic;

namespace KcetasWeb.ViewModels
{
    public class SayacListeViewModel : PaginationBaseViewModel
    {
        public string? FiltreSeriNo { get; set; }
        public string? FiltreMarka { get; set; }
        public string? FiltreDurum { get; set; }
        public string? FiltreMuhurNo { get; set; }
        public string? FiltreTuketimNoktasi { get; set; }
        public string? FiltreFaz { get; set; }

        public List<Sayac> Sayaclar { get; set; } = new();
    }
}
