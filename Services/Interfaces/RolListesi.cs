using System.Collections.Generic;
using System.Linq;
using KcetasWeb.Models.entities; // Rol sınıfının bulunduğu yer
using KcetasWeb.Models.entities;       // AppRoles sınıfının bulunduğu yer

namespace KcetasWeb.Services.Interfaces // Senin projeneki namespace'e göre ayarla
{
    public static class RolListesi
    {
        // Yeni 7 rolümüzü ID'leri ile birlikte sisteme tanıtıyoruz
        public static List<Rol> Roller = new List<Rol>
        {
            new Rol { RolId = 1, RolAdi = AppRoles.BTYoneticisi },          // Admin yetkisi
            new Rol { RolId = 2, RolAdi = AppRoles.MusteriTemsilcisi },
            new Rol { RolId = 3, RolAdi = AppRoles.SozlesmeYetkilisi },
            new Rol { RolId = 4, RolAdi = AppRoles.SayacOkumaPersoneli },
            new Rol { RolId = 5, RolAdi = AppRoles.SahaOperasyonAmiri },
            new Rol { RolId = 6, RolAdi = AppRoles.FaturalamaUzmani },
            new Rol { RolId = 7, RolAdi = AppRoles.Denetci }
        };

        // Verilen ID'ye göre rolü döndüren metot
        public static Rol? BulRolId(int id)
        {
            return Roller.FirstOrDefault(r => r.RolId == id);
        }
    }
}