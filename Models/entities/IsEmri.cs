namespace KcetasWeb.Models
{
    using KcetasWeb.Models.enums;
    using System;
    
    public class IsEmri
    {
        public long IsEmriId { get; set; }
        public string IsEmriNo { get; set; }
        
        public long TuketimNoktasiId { get; set; }
        public long SayacId { get; set; }
        
        public IsEmriTipi Tip { get; set; }
        public string Oncelik { get; set; }
        public DateTime PlanlananTarih { get; set; }
        
        public long AtananKullaniciId { get; set; }
        public string AtananKullaniciAdi { get; set; }
        
        public IsEmriDurumu Durum { get; set; }
        public string SahaSonucu { get; set; }
        public string Gerekce { get; set; }
        public string Aciklama { get; set; }
        
        public string EskiSayacNo { get; set; }
        public decimal? EskiSonEndeksi { get; set; }
        public string YeniSayacNo { get; set; }
        public decimal? YeniIlkEndeksi { get; set; }
        public string MuhurNo { get; set; }
        public string TutanakNo { get; set; }
        public string TuketimNoktasiKodu { get; set; }
        public string Adres { get; set; }
        public string SayacSeriNo { get; set; }
        
        public decimal? KesmeEndeksi { get; set; }
        public decimal? AcmaEndeksi { get; set; }
        
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}