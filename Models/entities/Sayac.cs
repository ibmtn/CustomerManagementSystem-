namespace KcetasWeb.Models
{
    public class Sayac
    {
        public long SayacId { get; set; }
        public string SeriNo { get; set; }
        
        // Hangi tüketim noktasında takılı olduğunu gösteren bağlantı
        public long TuketimNoktasiId { get; set; }
        
        public string Marka { get; set; }
        public string Model { get; set; }
        public string Faz { get; set; }
        
        // Çarpan ve endeks değerleri küsuratlı olabileceği için decimal
        public decimal Carpan { get; set; }
        public string MuhurNo { get; set; }
        public decimal AktifEndeks { get; set; }
        public decimal ReaktifEndeks { get; set; }
        
        public string Durum { get; set; }
        public string Status { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}