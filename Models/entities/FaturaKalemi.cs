namespace KcetasWeb.Models
{
    public class FaturaKalemi
    {
        public long FaturaKalemId { get; set; }
        public long FaturaId { get; set; } // Hangi faturaya ait?
        
        public string KalemTipi { get; set; } // KDV, Enerji Fonu, TRT Payı vb.
        public string Aciklama { get; set; }
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal Tutar { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}