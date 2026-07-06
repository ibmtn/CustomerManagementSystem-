namespace KcetasWeb.Models
{
    using System;

    public class EntegrasyonOutbox
    {
        public long Id { get; set; }
        public string AggregateType { get; set; }
        public string AggregateId { get; set; }
        public string EventType { get; set; }
        public string Payload { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
