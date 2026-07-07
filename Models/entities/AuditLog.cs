namespace KcetasWeb.Models
{
    using System;

    public class AuditLog
    {
        public long audit_id { get; set; }
        public string varlik_tipi { get; set; }
        public long vaklik_id { get; set; }
        public string islem_tipi { get; set; }
        public string eski_deger { get; set; }
        public string yeni_deger { get; set; }
        public int kullanici_id { get; set; }
        public long? islem_gerekcesi { get; set; }
        public DateTime islem_zamani { get; set; }
        
    }
}
