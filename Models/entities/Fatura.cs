namespace KcetasWeb.Models
{
    public class Fatura
    {
        public long FaturaId { get; set; }
        public string FaturaNo { get; set; }
        public long SozlesmeId { get; set; }
        public string TekilKod { get; set; }
        public long AboneId { get; set; }
        public string AboneTipi { get; set; }
        
        public string Donem { get; set; }
        public DateTime FaturaTarihi { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public long OkumaId { get; set; } // Hangi okumadan üretildi?
        
        public decimal IlkEndeks { get; set; }
        public decimal SonEndeks { get; set; }
        public decimal TuketimKwh { get; set; }
        
        public decimal ReaktifEnduktif { get; set; }
        public decimal ReaktifKapasitif { get; set; }
        public decimal Carpan { get; set; }
        
        // Ücretlendirme Alanları
        public decimal EnerjiBedeli { get; set; }
        public decimal DagitimBedeli { get; set; }
        public decimal HizmetBedeli { get; set; }
        public decimal KesmeBaglamaBedeli { get; set; }
        public decimal VergiFonToplam { get; set; }
        public decimal ToplamTutar { get; set; }
        
        public string Durum { get; set; } // Ödendi, Bekliyor, İptal
        public string GibUuid { get; set; } // e-Fatura entegrasyonu için
        public int RetryCount { get; set; }
        public string PdfUrl { get; set; }
        public string TeslimKanali { get; set; } // SMS, E-Posta, Matbu
        public DateTime? TeslimTarihi { get; set; }
        
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Fatura Kalemleri (Bire-Çok İlişki)
        public List<FaturaKalemi> FaturaKalemleri { get; set; }
    }
}