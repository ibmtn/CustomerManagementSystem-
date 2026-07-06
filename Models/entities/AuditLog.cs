namespace KcetasWeb.Models
{
    using System;

    public class AuditLog
    {
        public long AuditId { get; set; }
        public string IslemTipi { get; set; }
        public string TabloAdi { get; set; }
        public long KayitId { get; set; }
        public string EskiDeger { get; set; }
        public string YeniDeger { get; set; }
        public long? KullaniciId { get; set; }
        public string KullaniciAdi { get; set; }
        public DateTime IslemZamani { get; set; }
        public string IpAdresi { get; set; }
    }
}
