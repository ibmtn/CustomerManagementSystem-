namespace KcetasWeb.Models
{
    using System;

    public class IsEmri
    {
        public long IsEmriId { get; set; }
        public string IsEmriNo { get; set; }
        public long TuketimNoktasiId { get; set; }
        public long SayacId { get; set; }
        public string Tip { get; set; }
        public string Oncelik { get; set; }
        public DateTime PlanlananTarih { get; set; }
        public long? AtananKullaniciId { get; set; }
        public string Durum { get; set; }
        public string SahaSonucu { get; set; }
        public string Gerekce { get; set; }
        public string MuhurNo { get; set; }
        public string TutanakNo { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}