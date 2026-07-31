using KcetasWeb.Models.entities;
namespace KcetasWeb.ViewModels
{
    using System;

    public class TuketimNoktasiViewModels
    {
        public int TuketimNoktasiId { get; set; }
        public string tekil_kod { get; set; }
        
        // Abone'den gelen veriler (Join edilecek)
        public int AboneId { get; set; }
        public string? musteri_ad { get; set; }
        public string? musteri_soyad { get; set; }
        public string? musteri_unvan { get; set; }
        public string? tckn { get; set; }
        public string? vkn { get; set; }
        public string? telefon { get; set; }
        public string? e_posta { get; set; }
        public string? iletisim_tercihi { get; set; }
        
        // Tüketim Noktasından gelen veriler
        public int ilce_id { get; set; }
        public string? mahalle { get; set; }
        public string? bina_no { get; set; }
        public string? bagimsiz_bolum_no{ get; set; }
        public string? acik_adres { get; set; }
        public decimal koordinat_lat { get; set; }
        public decimal koordinat_lot { get; set; }
        public decimal baglanti_gucu_kw { get; set; }
        public string? tuketici_grubu { get; set; }
        public KcetasWeb.Models.Enums.BaglantiDurumu? baglanti_durumu { get; set; }
        public string? status { get; set; }
        
        public int sayac_id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string? Unvan { get; set; }

        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string is_emri_no { get; set; }
        public int sozlesme_id { get; set; }
        public int il_id { get; set; }
        public string il_adi { get; set; }
        public string ilce_adi { get; set; }
        public int okuma_id { get; set; }
    }


    public class TuketimNoktasiListeViewModel : PaginationBaseViewModel
    {
        public string? FiltreTekilKod { get; set; }
        public int? FiltreIlId { get; set; }
        public int? FiltreIlceId { get; set; }
        public string? FiltreTuketiciGrubu { get; set; }
        public string? FiltreDurum { get; set; }

        public System.Collections.Generic.List<TuketimNoktasiViewModels> TuketimNoktalari { get; set; } = new();
    }
}