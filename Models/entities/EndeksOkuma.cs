namespace KcetasWeb.Models
{
    using KcetasWeb.Models.enums;
    using System;
    
    public class EndeksOkuma
    {
        public long OkumaId { get; set; }
        public long TuketimNoktasiId { get; set; }
        public long SayacId { get; set; }
        public long SozlesmeId { get; set; }
        public string SayacSeriNo { get; set; }
        public string TuketimNoktasiKodu { get; set; }
        
        public DateTime OkumaTarihi { get; set; }
        public decimal OncekiEndeks { get; set; }
        public decimal GuncelEndeks { get; set; }
        public decimal TuketimMiktari { get; set; }
        
        public OkumaKaynagi OkumaKaynagi { get; set; }
        public OkumaDurumu OkumaDurumu { get; set; }
        
        public long OkuyanKullaniciId { get; set; }
        public string OkuyanKullaniciAdi { get; set; }
        public bool DogrulamaDurumu { get; set; }
        public string AnomaliAciklamasi { get; set; }
        public string TarifeGrubu { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}