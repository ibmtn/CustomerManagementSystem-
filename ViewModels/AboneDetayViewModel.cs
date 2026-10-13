using System.Collections.Generic;
using MusteriYonetim.Models;

namespace MusteriYonetim.ViewModels
{
    public class AboneDetayViewModel
    {
        public Abone Abone { get; set; } = null!;
        public string KimlikNoMaskeli { get; set; } = null!;
        
        // Mock sekmeler için listeler
        public List<Sozlesme> Sozlesmeler { get; set; } = new();
        public List<TuketimNoktasi> TuketimNoktalari { get; set; } = new();
        public List<Fatura> Faturalar { get; set; } = new();
        public List<IsEmri> IsEmirleri { get; set; } = new();
        public List<object> Bildirimler { get; set; } = new(); 
    }
}