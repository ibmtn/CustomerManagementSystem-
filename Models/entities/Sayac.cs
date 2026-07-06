namespace KcetasWeb.Models
{
    using System;

    public class Sayac
    {
        public long SayacId { get; set; }
        public string SayacNo { get; set; }
        public int MarkaId { get; set; }
        public int ModelId { get; set; }
        public string Tip { get; set; }
        public int ImalYili { get; set; }
        public string Durum { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}