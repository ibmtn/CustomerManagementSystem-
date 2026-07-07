namespace KcetasWeb.Models.entities
{
    public class Kullanici
    {
        public long kullanici_id { get; set; }
        public string ad_soyad { get; set; }
        public string kullanici_adi { get; set; }
        public string e_posta { get; set; }
        public string sifre_hash { get; set; }
        
        // Foreign Key (Rol tablosuna bağlantı)
        public short rol_id { get; set; }
        
        public string durum { get; set; }
        public DateTime created_at { get; set; }
        
        // UpdatedAt null (boş) olabileceği için DateTime? (soru işareti) kullandık
        public DateTime? updated_at { get; set; }

        // İlişki: Her kullanıcının veritabanında bağlı olduğu bir Rol nesnesi vardır
        public Rol Rol { get; set; }
        // YENİ: Sadece "Abone" rolündeki kullanıcılar için doldurulur (Mesken / İş Yeri)
        public string? AboneTuru { get; set; }
    }
}