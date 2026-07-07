using KcetasWeb.Models.entities;

namespace KcetasWeb.Services.Interfaces
{
    // GEÇİCİ: PostgreSQL bağlanana kadar roller bellekte sabit tutulur.
    public static class RolListesi
    {
        public static readonly List<Rol> Roller = new()
        {
            new Rol { rol_id = 1, rol_adi = AppRoles.BTYoneticisi, aciklama = "Entegrasyon/BT Yöneticisi", created_at = DateTime.Now },
            new Rol { rol_id = 2, rol_adi = AppRoles.MusteriTemsilcisi, aciklama = "Müşteri Temsilcisi", created_at = DateTime.Now },
            new Rol { rol_id = 3, rol_adi = AppRoles.SozlesmeYetkilisi, aciklama = "Sözleşme Yetkilisi", created_at = DateTime.Now },
            new Rol { rol_id = 4, rol_adi = AppRoles.SayacOkumaPersoneli, aciklama = "Sayaç Okuma Personeli", created_at = DateTime.Now },
            new Rol { rol_id = 5, rol_adi = AppRoles.SahaOperasyonAmiri, aciklama = "Saha Operasyon Amiri", created_at = DateTime.Now },
            new Rol { rol_id = 6, rol_adi = AppRoles.FaturalamaUzmani, aciklama = "Faturalama Uzmanı", created_at = DateTime.Now },
            new Rol { rol_id = 7, rol_adi = AppRoles.Denetci, aciklama = "Denetçi/Rapor Kullanıcısı", created_at = DateTime.Now }
        };

        public static Rol? BulRolId(short rolId)
        {
            return Roller.FirstOrDefault(r => r.rol_id == rolId);
        }
    }
}