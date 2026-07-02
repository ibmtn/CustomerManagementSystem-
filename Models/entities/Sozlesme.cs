namespace KcetasWeb.Models
{
    public class Sozlesme
    {
        public long SozlesmeId { get; set; }
        public string SozlesmeNo { get; set; }
        
        // Bağlantılar (Abone ve Tüketim Noktası)
        public long AboneId { get; set; }
        public string AboneAdSoyad { get; set; }
        public long TuketimNoktasiId { get; set; }
        public string TuketimNoktasiKod { get; set; }
        
        public string SozlesmeTipi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; } // Sözleşme devam ediyorsa bitiş tarihi boştur
        
        public string Statu { get; set; }
        public string TarifeGrubu { get; set; }
        public string Tedarikci { get; set; }
        
        // Parasal değerler olduğu için decimal kullanıyoruz
        public decimal GuvenceBedeli { get; set; } 
        public string OdemeSekli { get; set; }
        public decimal BaslangicEndeksi { get; set; }
        
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}