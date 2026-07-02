namespace KcetasWeb.Models
{
    public class TuketimNoktasi
    {
        public long TuketimNoktasiId { get; set; }
        public string TekilKod { get; set; }
        public string TesisatNo { get; set; }
        
        // Veritabanındaki il/ilçe bağlantıları
        public int IlId { get; set; }
        public int IlceId { get; set; }
        
        public string Mahalle { get; set; }
        public string BinaNo { get; set; }
        public string BagimsizBolumNo { get; set; }
        public string AcikAdres { get; set; }
        
        // Saha ekipleri için harita koordinatları
        public string KoordinatLat { get; set; }
        public string KoordinatLon { get; set; }
        
        // Kw cinsinden sayısal değer olduğu için decimal kullanıyoruz
        public decimal BaglantiGucuKw { get; set; } 
        public string TuketiciGrubu { get; set; }
        public string BaglantiDurumu { get; set; }
        
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}