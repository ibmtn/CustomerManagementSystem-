using System;

namespace MusteriYonetim.Models.Dtos
{
    public class FaturaListDto
    {
        public int FaturaId { get; set; }
        public string FaturaNo { get; set; }
        public string Donem { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; }
        public DateTime FaturaTarihi { get; set; }
        public string SozlesmeNo { get; set; }
        public string TekilKod { get; set; }
    }
}
