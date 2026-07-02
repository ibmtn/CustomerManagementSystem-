namespace KcetasWeb.Models
{
    public class Rol
    {
        public short RolId { get; set; }
        public string RolAdi { get; set; }
        public string Aciklama { get; set; }
        public DateTime CreatedAt { get; set; }

        // İlişki: Bir rolün (Örn: Yönetici) birden fazla kullanıcısı olabilir
        public List<Kullanici> Kullanicilar { get; set; }
    }
}