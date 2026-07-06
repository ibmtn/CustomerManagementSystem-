namespace KcetasWeb.Models
{
    using System;

    public class Fatura
    {
        public long FaturaId { get; set; }
        public string FaturaNo { get; set; }
        public long SozlesmeId { get; set; }
        public long EndeksOkumaId { get; set; }
        public DateTime FaturaTarihi { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public decimal KdvTutari { get; set; }
        public string OdemeDurumu { get; set; }
        public DateTime? OdemeTarihi { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}