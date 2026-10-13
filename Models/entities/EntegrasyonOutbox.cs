namespace MusteriYonetim.Models
{
    using System;
    using System.Text.Json.Serialization;

    public class EntegrasyonOutbox
    {
        [JsonPropertyName("outboxId")]
        public int outbox_id { get; set; }

        [JsonPropertyName("faturaId")]
        public int fatura_id { get; set; }

        [JsonPropertyName("hedefSistem")]
        public MusteriYonetim.Models.Enums.HedefSistem? hedef_sistem { get; set; }

        [JsonPropertyName("idempotencyKey")]
        public string? idempotency_key { get; set; }

        [JsonPropertyName("correlationId")]
        public string? corrolation_id { get; set; }

        [JsonPropertyName("payload")]
        public string? paload { get; set; }

        [JsonPropertyName("durum")]
        public MusteriYonetim.Models.Enums.OutboxDurumu? durum { get; set; }

        [JsonPropertyName("hataKodu")]
        public string? hata_kodu { get; set; }

        [JsonPropertyName("hataMesaji")]
        public string? hata_mesaji { get; set; }

        [JsonPropertyName("retryCount")]
        public int retry_count { get; set; }

        [JsonPropertyName("sonDenemeTarihi")]
        public DateTime? son_deneme_tarihi { get; set; }

        [JsonPropertyName("gonderimZamani")]
        public DateTime? gonderim_zamani { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime created_at { get; set; }

        [JsonPropertyName("fatura")]
        public Fatura? fatura { get; set; }
    }
}
