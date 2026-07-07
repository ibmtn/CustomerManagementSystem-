namespace KcetasWeb.Models.entities
{
    public class Rol
    {
        public short rol_id { get; set; }
        public string rol_adi { get; set; }
        public string aciklama { get; set; }
        public DateTime created_at { get; set; }

        // İlişki: Bir rolün (Örn: Yönetici) birden fazla kullanıcısı olabilir
        public List<Kullanici> Kullanicilar { get; set; }
    }
}