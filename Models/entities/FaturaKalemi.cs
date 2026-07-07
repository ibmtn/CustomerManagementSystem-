namespace KcetasWeb.Models
{
    public class FaturaKalemi
    {
        public long fatura_kalemi_id { get; set; }
        public int fatura_id { get; set; } // Hangi faturaya ait?
        
        public string kalem_tipi { get; set; } // KDV, Enerji Fonu, TRT Payı vb.
        public string aciklama { get; set; }
        public decimal miktar { get; set; }
        public decimal birim_fiyati { get; set; }
        public decimal tutar { get; set; }
        
        public DateTime created_at { get; set; }
    }
}