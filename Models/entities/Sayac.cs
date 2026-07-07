namespace KcetasWeb.Models
{
    using System;

    public class Sayac
    {
        public long sayac_id { get; set; }
        public string seri_no { get; set; }
        public int tuketim_noktasi_id { get; set; }
        public string marka { get; set; }
        public string model { get; set; }
        public string faz { get; set; }
        public decimal carpan { get; set; }
        public long muhur_no { get; set; } // Added for Bagla feature
        public string durum { get; set; }
        public string status { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}