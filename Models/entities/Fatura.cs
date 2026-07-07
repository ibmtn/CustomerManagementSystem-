namespace KcetasWeb.Models
{
    using System;

    public class Fatura
    {
        public long fatura_id { get; set; }
        public string fatura_no { get; set; }
        public long sozlesme_id { get; set; }
        public string tekil_kod { get; set; }
        public string fatura_tipi { get; set; }
        public string donem { get; set; }
        public DateTime fatura_tarihi { get; set; }
        public DateTime son_odeme_tarihi { get; set; }
        public int okuma_id { get; set; }
        public decimal ilk_endeks { get; set; }
        public decimal son_endeks { get; set; }
        public decimal tuketim_kwh { get; set; }
        public decimal reaktif_enduktif { get; set; }
        public decimal reaktif_kapasitif { get; set; }
        public decimal carpan { get; set; }
        public decimal enerji_bedeli { get; set; }
        public decimal dagatim_bedeli { get; set; }
        public decimal hizmet_bedeli { get; set; }
        public decimal kesme_baglama_bedeli { get; set; }
        public decimal vergi_fon_toplama { get; set; }
        public decimal toplam_tutar { get; set; }
        public string durum { get; set; }
        public string status { get; set; }

        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}