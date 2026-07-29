using KcetasWeb.Models.entities;

namespace KcetasWeb.Services.Interfaces
{
    // GEÇİCİ ÇÖZÜM: PostgreSQL bağlanana kadar kullanıcılar bellekte (RAM) tutulur.
    public class KullaniciDeposu : IKullaniciDeposu
    {
        private readonly List<Kullanici> _kullanicilar;
        private readonly object _kilit = new();
        private int _sonId;

        public KullaniciDeposu()
        {
            // Başlangıç (seed) verisi - yeni rol yapısına göre güncellendi
            _kullanicilar = new List<Kullanici>
            {
                new Kullanici {
                    kullanici_id = 1, ad_soyad = "Ahmet Yılmaz", kullanici_adi = "ahmety",
                    e_posta = "ahmet@kcetas.com", durum = KcetasWeb.Models.Enums.KullaniciDurumu.Aktif, rol_id = 5,
                    Rol = RolListesi.BulRolId(5), created_at = DateTime.Now // Saha Operasyon Amiri
                },
                new Kullanici {
                    kullanici_id = 2, ad_soyad = "Ayşe Demir", kullanici_adi = "aysed",
                    e_posta = "ayse@kcetas.com", durum = KcetasWeb.Models.Enums.KullaniciDurumu.Aktif, rol_id = 2,
                    Rol = RolListesi.BulRolId(2), created_at = DateTime.Now // Müşteri Temsilcisi
                },
                new Kullanici {
                    kullanici_id = 3, ad_soyad = "Sistem Yöneticisi", kullanici_adi = "admin",
                    e_posta = "admin@kcetas.com", durum = KcetasWeb.Models.Enums.KullaniciDurumu.Aktif, rol_id = 1,
                    Rol = RolListesi.BulRolId(1), created_at = DateTime.Now // BT Yöneticisi
                }
            };
            _sonId = _kullanicilar.Max(k => k.kullanici_id);
        }

        public Task<bool> KullaniciAdiVarMiAsync(string kullaniciAdi)
        {
            lock (_kilit)
            {
                return Task.FromResult(_kullanicilar.Any(k =>
                    k.kullanici_adi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase)));
            }
        }

        public Task<bool> EPostaVarMiAsync(string ePosta)
        {
            lock (_kilit)
            {
                return Task.FromResult(_kullanicilar.Any(k =>
                    k.e_posta != null && k.e_posta.Equals(ePosta, StringComparison.OrdinalIgnoreCase)));
            }
        }

        public Task<bool> GirisKontrolAsync(string kullaniciAdi, string sifre)
        {
            var kullanici = _kullanicilar.FirstOrDefault(k => string.Equals(k.kullanici_adi, kullaniciAdi, StringComparison.OrdinalIgnoreCase));
            if (kullanici == null) return Task.FromResult(false);
            
            return Task.FromResult(kullanici.Sifre == sifre || kullanici.sifre_hash == sifre);
        }

        public Task<Kullanici> EkleAsync(Kullanici kullanici)
        {
            lock (_kilit)
            {
                _sonId++;
                kullanici.kullanici_id = _sonId;
                kullanici.Rol = RolListesi.BulRolId(kullanici.rol_id ?? 0);
                _kullanicilar.Add(kullanici);
                return Task.FromResult(kullanici);
            }
        }

        public Task<Kullanici> BulKullaniciAdiIleAsync(string kullaniciAdi)
        {
            lock (_kilit)
            {
                return Task.FromResult(_kullanicilar.FirstOrDefault(k =>
                    k.kullanici_adi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase)));
            }
        }

        public Task<List<Kullanici>> ListeleAsync()
        {
            lock (_kilit)
            {
                return Task.FromResult(_kullanicilar.ToList());
            }
        }

        public Task<Kullanici> BulIdAsync(long id)
        {
            lock (_kilit)
            {
                return Task.FromResult(_kullanicilar.FirstOrDefault(k => k.kullanici_id == id));
            }
        }

        public Task<bool> GuncelleAsync(Kullanici guncel)
        {
            lock (_kilit)
            {
                var mevcut = _kullanicilar.FirstOrDefault(k => k.kullanici_id == guncel.kullanici_id);
                if (mevcut == null) return Task.FromResult(false);

                mevcut.ad_soyad = guncel.ad_soyad;
                mevcut.kullanici_adi = guncel.kullanici_adi;
                mevcut.e_posta = guncel.e_posta;
                mevcut.durum = guncel.durum;
                mevcut.rol_id = guncel.rol_id;
                mevcut.Rol = RolListesi.BulRolId(guncel.rol_id ?? 0);
                mevcut.updated_at = DateTime.Now;
                return Task.FromResult(true);
            }
        }

        public Task<bool> SilAsync(long id)
        {
            lock (_kilit)
            {
                var kullanici = _kullanicilar.FirstOrDefault(k => k.kullanici_id == id);
                if (kullanici == null) return Task.FromResult(false);
                _kullanicilar.Remove(kullanici);
                return Task.FromResult(true);
            }
        }
    }
}