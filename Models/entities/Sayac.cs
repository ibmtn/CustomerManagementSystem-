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
       public string? muhur_no { get; set; } // Added for Bagla feature
        public string sökme_nedeni {get; set;}
        public string MuhurDurumu {get; set;}
        public string durum { get; set; }
        public string status { get; set; }
        public decimal? son_endeks { get; set; }
        public string? aciklama { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

          public int uretim_yili { get; set; }
    }
}