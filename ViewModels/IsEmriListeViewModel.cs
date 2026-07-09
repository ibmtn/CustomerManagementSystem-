using System;
using System.Collections.Generic;

namespace KcetasWeb.ViewModels
{
    public class IsEmriListeViewModel
    {
        public string? FiltreTip { get; set; }
        public string? FiltreDurum { get; set; }
        public DateTime? BaslangicTarih { get; set; }
        public DateTime? BitisTarih { get; set; }
        public string? AramaMetni { get; set; }
        
        // Yeni Filtre Alanları
        public string? FiltreIsEmriNo { get; set; }
        public string? FiltreTekilKod { get; set; }
        public string? FiltreAboneAdi { get; set; }
        public string? FiltrePersonel { get; set; }
        public string? FiltreOncelik { get; set; }
        

        

        public List<IsEmriSatirViewModel> IsEmirleri { get; set; } = new();

        public static string GetDurumRenk(string durum) => durum switch
        {
            "Oluşturuldu" => "secondary",
            "Ekibe Atandı" => "info",
            "Devam Ediyor" => "primary",
            "Tamamlandı" => "success",
            "İptal Edildi" => "danger",
            "Durduruldu" => "warning",
            _ => "dark"
        };
    }

    public class IsEmriSatirViewModel
    {
        public long IsEmriId { get; set; }
        public string IsEmriNo { get; set; } = null!;
        public string Tip { get; set; } = null!;
        public long TuketimNoktasiId { get; set; }
        public string tekil_kod { get; set; }
        public string TuketimNoktasiKodu { get; set; } = null!;
        public DateTime? PlanlananTarih { get; set; }
        public DateTime olusturulma_tarihi {get; set;}
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string AtananKullaniciAdi { get; set; } = null!;
        public string Durum { get; set; } = null!;
        public string DurumRenk { get; set; } = null!;
        public string Adres { get; set; } = null!;
        public string oncelik { get; set; }
        public string? musteri_ad { get; set; }
        public string? musteri_soyad { get; set; }
        public string? musteri_unvan { get; set; }
        

        public string musteriDurum => $"{musteri_ad} {musteri_soyad} {musteri_unvan}";
        
    }
}