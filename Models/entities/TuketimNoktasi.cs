namespace KcetasWeb.Models
{
    using System;

    public class TuketimNoktasi
    {
        public long TuketimNoktasiId { get; set; }
        public string tekil_kod { get; set; }
        public string? musteri_ad { get; set; }
        public string? musteri_soyad { get; set; }
        public string? musteri_unvan { get; set; }
        public string? tckn { get; set; }
        public string? vkn { get; set; }
        public string? telefon { get; set; }
        public string? e_posta { get; set; }
        public string? iletisim_tercihi { get; set; }
        public int ilce_id { get; set; }
        public string? il_adi { get; set; }
        public string? ilce_adi { get; set; }
        public string? mahalle { get; set; }
        public string? bina_no { get; set; }
        public string? bagimsiz_bolum_no{ get; set; }
        public string? acik_adres { get; set; }
        public string? koordinat_lat { get; set; }
        public string? koordinat_lot { get; set; }
        public string? baglanti_gucu_kw { get; set; }
        public string? tuketici_grubu { get; set; }
        public string? baglanti_grubu { get; set; }
        public string? status { get; set; }

        
        // MVP Eksikleri:
        public decimal BaglantiGucuKw { get; set; }
        public string? Enlem { get; set; }
        public string? Boylam { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // TuketimNoktasi.cs içinde olması gereken örnek tanım
        public virtual TuketiciGrubu TuketiciGrubu { get; set; }

    }


    
        

        
}