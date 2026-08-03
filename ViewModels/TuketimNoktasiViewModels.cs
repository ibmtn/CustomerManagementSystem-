using KcetasWeb.Models.entities;
namespace KcetasWeb.ViewModels
{
    using KcetasWeb.Models.Enums;
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

        public string BaglantiDurumuEtiketi => GetBaglantiDurumuAd(baglanti_durumu, status);
        public string BaglantiDurumuBadgeClass => GetBaglantiDurumuBadgeClass(baglanti_durumu, status);

        public static BaglantiDurumu? ResolveBaglantiDurumu(BaglantiDurumu? baglantiDurumu, string? status)
        {
            if (baglantiDurumu.HasValue)
                return baglantiDurumu.Value;

            return status?.Trim().ToUpperInvariant() switch
            {
                "AKTIF" or "AKTİF" or "ACTIVE" or "BAGLI" or "BAĞLI" => BaglantiDurumu.Aktif,
                "PASIF" or "PASİF" or "KESIK" or "KESİK" => BaglantiDurumu.Pasif,
                "KAPALI" or "SOKULU" or "SÖKÜLÜ" => BaglantiDurumu.Kapali,
                "TASLAK" => BaglantiDurumu.Taslak,
                "BAGLANTI BEKLIYOR" or "BAĞLANTI BEKLİYOR" => BaglantiDurumu.BaglantiBekliyor,
                "BAGLANABILIR" or "BAĞLANABİLİR" => BaglantiDurumu.Baglanabilir,
                _ => null
            };
        }

        public static string GetBaglantiDurumuAd(BaglantiDurumu? baglantiDurumu, string? status)
        {
            return ResolveBaglantiDurumu(baglantiDurumu, status) switch
            {
                BaglantiDurumu.Aktif => "Bağlı",
                BaglantiDurumu.Pasif => "Kesik",
                BaglantiDurumu.Kapali => "Sökülü",
                BaglantiDurumu.BaglantiBekliyor => "Bağlantı Bekliyor",
                BaglantiDurumu.Baglanabilir => "Bağlanabilir",
                BaglantiDurumu.Taslak => "Taslak",
                _ => string.IsNullOrWhiteSpace(status) ? "-" : status
            };
        }

        public static string GetBaglantiDurumuBadgeClass(BaglantiDurumu? baglantiDurumu, string? status)
        {
            return ResolveBaglantiDurumu(baglantiDurumu, status) switch
            {
                BaglantiDurumu.Aktif => "bg-success text-white",
                BaglantiDurumu.Pasif => "bg-danger text-white",
                BaglantiDurumu.Kapali => "bg-secondary text-white",
                BaglantiDurumu.BaglantiBekliyor => "bg-warning text-dark",
                BaglantiDurumu.Baglanabilir => "bg-info text-dark",
                BaglantiDurumu.Taslak => "bg-secondary text-white",
                _ => "bg-secondary text-white"
            };
        }
    }


    public class TuketimNoktasiListeViewModel : PaginationBaseViewModel
    {
        public string? FiltreTekilKod { get; set; }
        public int? FiltreIlId { get; set; }
        public int? FiltreIlceId { get; set; }
        public string? FiltreTuketiciGrubu { get; set; }
        public string? FiltreBaglantiDurumu { get; set; }

        public System.Collections.Generic.List<TuketimNoktasiViewModels> TuketimNoktalari { get; set; } = new();
    }
}
