namespace MusteriYonetim.Models.Dtos
{
    public class AboneListDto
    {
        public int AboneId { get; set; }
        public string AboneNo { get; set; }
        public string AdSoyadUnvan { get; set; }
        public string TcknVkn { get; set; }
        public string Telefon { get; set; }
        public MusteriYonetim.Models.Enums.AboneTipi AboneTipi { get; set; }
        public string Durum { get; set; }
    }
}
