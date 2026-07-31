using System.Collections.Generic;

namespace KcetasWeb.ViewModels
{
    public class AboneListeViewModel : PaginationBaseViewModel
    {
        public string? FiltreAboneNo { get; set; }
        public string? FiltreAdSoyadUnvan { get; set; }
        public string? FiltreAboneTipi { get; set; }
        public string? FiltreDurum { get; set; }

        public List<AboneSatirViewModel> Aboneler { get; set; } = new();
    }

    public class AboneSatirViewModel
    {
        public int AboneId { get; set; }
        public string AboneNo { get; set; } = null!;
        public string AdSoyad { get; set; } = null!;
        public string KimlikNoMaskeli { get; set; } = null!;
        public string Telefon { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public KcetasWeb.Models.Enums.AboneTipi? AboneTipi { get; set; }
        public string Durum { get; set; } = null!;
        
        [System.Text.Json.Serialization.JsonPropertyName("ePosta")]
        public string EPostaApi { get; set; } = null!;
    }
}
