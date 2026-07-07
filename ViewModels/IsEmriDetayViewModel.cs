using System;

namespace KcetasWeb.ViewModels
{
    public class IsEmriDetayViewModel
    {
        public long IsEmriId { get; set; }
        public string IsEmriNo { get; set; } = null!;
        
        public string Tip { get; set; } = null!;
        public string Durum { get; set; } = null!;
        public string DurumRenk { get; set; } = null!;
        
        public string? Oncelik { get; set; }
        public DateTime PlanlananTarih { get; set; }
        public string? AtananKullaniciAdi { get; set; }
        
        public string? TuketimNoktasiKodu { get; set; }
        public string? Adres { get; set; }
        public string? SayacSeriNo { get; set; }
        
        public string? SahaSonucu { get; set; }
        public string? Gerekce { get; set; }
        public string? MuhurNo { get; set; }
        public string? TutanakNo { get; set; }

        public string? EskiSayacNo { get; set; }
        public string? YeniSayacNo { get; set; }
        public decimal? EskiSonEndeksi { get; set; }
        public decimal? YeniIlkEndeksi { get; set; }
        
        public decimal? KesmeEndeksi { get; set; }
        public decimal? AcmaEndeksi { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public bool TutanakGirildiMi => !string.IsNullOrEmpty(TutanakNo);
        public bool IsSokmeTakma => Tip == "Sayaç Sökme" || Tip == "Sayaç Takma" || Tip == "Sayaç Değişim";
        public bool IsAcmaKesme => Tip == "Açma" || Tip == "Kesme";
    }
}
