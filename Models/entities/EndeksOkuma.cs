namespace KcetasWeb.Models
{
    using System;

    public class EndeksOkuma
    {
        public long okuma_id { get; set; }
        public long sayac_id { get; set; }
        public long? is_emri_id { get; set; }
        public long? sozlesme_id { get; set; }
        public string? okuma_tipi { get; set; }
        public string? okuma_kaynagi { get; set; }
        public decimal? onceki_endeks { get; set; }
        public decimal? yeni_endeks { get; set; }
        public DateTime? okuma_zamani { get; set; }
        public int? kullanici_id { get; set; }
        public decimal? okunamam_nedeni { get; set; }
        public decimal? dogrulama_durumu { get; set; }
        public bool? anomali_mi { get; set; }
        // MVP Eksikleri:
        public string? status { get; set; }
        public string? AnomaliAciklamasi { get; set; }
        public DateTime CreatedAt { get; set; }
       
    }
}