using System;

namespace KcetasWeb.ViewModels
{
    public class IsEmriDetayViewModel
    {
        public long IsEmriId { get; set; }
        public string IsEmriNo { get; set; } = null!;
        public string tekil_kod { get; set; }
        public string Tip { get; set; } = null!;
        public string Durum { get; set; } = null!;
        public string DurumRenk { get; set; } = null!;
        public string model { get; set; }
        public string? Oncelik { get; set; }
        public DateTime? PlanlananTarih { get; set; }
        public string? AtananKullaniciAdi { get; set; }
        public string marka { get; set; }
        public decimal son_endeks {get; set;}
        public string? musteri_ad { get; set; }
        public string? musteri_soyad { get; set; }
        public string? musteri_unvan { get; set; }
        public string? telefon { get; set; }
        public string musteriDurum => string.IsNullOrWhiteSpace(musteri_unvan) ? $"{musteri_ad} {musteri_soyad}" : $"{musteri_unvan}";
        public string aciklama {get; set;}
        public string? TuketimNoktasiKodu { get; set; }
        public string? Adres { get; set; }
        public string? SayacSeriNo { get; set; }
        public string sökme_nedeni {get; set;}
        public string? SahaSonucu { get; set; }
        public string? Gerekce { get; set; }
        public string? MuhurNo { get; set; }
        public string? TutanakNo { get; set; }
        public decimal carpan { get; set; }
        public string? EskiSayacNo { get; set; }
        public string? YeniSayacNo { get; set; }
        public decimal? EskiSonEndeksi { get; set; }
        public decimal? YeniIlkEndeksi { get; set; }
        public decimal? onceki_endeks { get; set; }
        public decimal? yeni_endeks { get; set; }
        public decimal? KesmeEndeksi { get; set; }
        public decimal? AcmaEndeksi { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public bool TutanakGirildiMi => !string.IsNullOrEmpty(TutanakNo);
        public bool IsSokmeTakma => Tip == "Sayaç Sökme" || Tip == "Sayaç Takma" || Tip == "Sayaç Değişim";
        public bool IsAcmaKesme => Tip == "Açma" || Tip == "Kesme";
    }
}
