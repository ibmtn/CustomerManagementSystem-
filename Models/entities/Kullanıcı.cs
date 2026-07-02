namespace KcetasWeb.Models
{
    public class Kullanici
    {
        public long KullaniciId { get; set; }
        public string AdSoyad { get; set; }
        public string KullaniciAdi { get; set; }
        public string EPosta { get; set; }
        public string SifreHash { get; set; }
        
        // Foreign Key (Rol tablosuna bağlantı)
        public short RolId { get; set; }
        
        public string Durum { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // UpdatedAt null (boş) olabileceği için DateTime? (soru işareti) kullandık
        public DateTime? UpdatedAt { get; set; }

        // İlişki: Her kullanıcının veritabanında bağlı olduğu bir Rol nesnesi vardır
        public Rol Rol { get; set; }
    }
}