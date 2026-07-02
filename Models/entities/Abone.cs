namespace KcetasWeb.Models
{
    public class Abone
    {
        public long AboneId { get; set; }
        public string AboneNo { get; set; }
        public string AdSoyadUnvan { get; set; }
        public string AboneTipi { get; set; }
        public string TcKimlikVergiNo { get; set; }
        public string Telefon { get; set; }
        public string EPosta { get; set; }
        public string IletisimTercihi { get; set; }
        public string Status { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } // Boş olabileceği için soru işareti var
    }
}