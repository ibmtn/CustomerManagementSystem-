using System;
using System.ComponentModel.DataAnnotations;

namespace KcetasWeb.ViewModels
{
    public class TutanakGirisViewModel
    {
        public long IsEmriId { get; set; }
        public string IsEmriNo { get; set; } = null!;
        public string Tip { get; set; } = null!;
        
        [Required(ErrorMessage="Tutanak numarası zorunludur")]
        public string TutanakNo { get; set; } = null!;
        
        [Required(ErrorMessage="Saha sonucu zorunludur")]
        public string SahaSonucu { get; set; } = null!;
        
        public string? Gerekce { get; set; }
        
        [Required]
        public DateTime IslemTarihi { get; set; } = DateTime.Now;
        
        public string? MuhurNo { get; set; }
        
        // Sökme/Takma Endeksleri
        public string? EskiSayacNo { get; set; }
        public string? YeniSayacNo { get; set; }
        public decimal? EskiSonEndeksi { get; set; }
        public decimal? YeniIlkEndeksi { get; set; }
        
        // Açma/Kesme Endeksleri
        public decimal? KesmeEndeksi { get; set; }
        public decimal? AcmaEndeksi { get; set; }

        public bool IsSokmeTakma => Tip == "Sayaç Sökme" || Tip == "Sayaç Takma" || Tip == "Sayaç Değişim";
        public bool IsAcmaKesme => Tip == "Açma" || Tip == "Kesme";
    }
}
