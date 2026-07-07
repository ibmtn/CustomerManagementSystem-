namespace KcetasWeb.Models
{
    using System;

    public class EntegrasyonOutbox
    {
        public long outbox_id { get; set; }
        public int fatura_id { get; set; }
        public string hedef_sistem { get; set; }
        public string idempotency_key { get; set; }
        public string corrolation_id { get; set; }
        public string paload { get; set; }
        public string durum { get; set; }
        public string hata_kodu { get; set; }
        public string hata_mesaji { get; set; }
        public int retry_count { get; set; }
        public DateTime son_deneme_tarihi { get; set; }
        public DateTime gonderim_zamani { get; set; }
        public DateTime created_at { get; set; }
        
    }
}
