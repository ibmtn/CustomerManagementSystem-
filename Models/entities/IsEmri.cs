namespace KcetasWeb.Models
{
    using System;

    public class IsEmri
    {
        public long is_emri_id { get; set; }
        public string is_emri_no { get; set; }
        public long tuketim_noktasi_id { get; set; }
        public long sayac_id { get; set; }
        public string tip { get; set; }
        public string oncelik { get; set; }
        public DateTime planlanan_tarih { get; set; }
        public long? atanan_kullanici_id { get; set; }
        public string durum { get; set; }
        public string saha_sonucu { get; set; }
        public string gerekce { get; set; }
        public string muhur_no { get; set; }
        public string tutanak_no { get; set; }
        public string status { get; set; }
        
        // Açma-Kesme Endeksleri
        public decimal? kesme_endeksi { get; set; }
        public decimal? acma_endeksi { get; set; }
        
        // Sökme-Takma Endeksleri
        public string? eski_sayac_no { get; set; }
        public string? yeni_sayac_no { get; set; }
        public decimal? eski_son_endeksi { get; set; }
        public decimal? yeni_ilk_endeksi { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}