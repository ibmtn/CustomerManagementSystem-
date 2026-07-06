namespace KcetasWeb.Models
{
    using System;

    public class Sozlesme
    {
        public long SozlesmeId { get; set; }
        public string SozlesmeNo { get; set; }
        public long TuketimNoktasiId { get; set; }
        public long AboneId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string Durum { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}