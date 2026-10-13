using System;
using System.ComponentModel.DataAnnotations;

namespace MusteriYonetim.ViewModels
{
    public class YeniIsEmriViewModel
    {
        [Required(ErrorMessage = "Tüketim noktası seçimi zorunludur.")]
        public long TuketimNoktasiId { get; set; }

        [Required(ErrorMessage = "İş emri tipi zorunludur.")]
        public MusteriYonetim.Models.Enums.IsEmriTipi Tip { get; set; }

        [Required(ErrorMessage = "Öncelik zorunludur.")]
        public string Oncelik { get; set; } = "Normal";

        [Required(ErrorMessage = "Planlanan tarih zorunludur.")]
        public DateTime PlanlananTarih { get; set; } = DateTime.Now;

        public long? AtananKullaniciId { get; set; }

        public string? Aciklama { get; set; }
        
        public long? SozlesmeId { get; set; }
        
        public long? SayacId { get; set; }
        
        public string? IsEmriNo { get; set; }
    }
}
