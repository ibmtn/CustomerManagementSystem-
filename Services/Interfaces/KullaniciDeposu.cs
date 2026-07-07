using KcetasWeb.Models.entities;

namespace KcetasWeb.Services.Interfaces
{
    // GEÇİCİ ÇÖZÜM: PostgreSQL bağlanana kadar kullanıcılar bellekte (RAM) tutulur.
    public class KullaniciDeposu : IKullaniciDeposu
    {
        private readonly List<Kullanici> _kullanicilar;
        private readonly object _kilit = new();
        private long _sonId;

        public KullaniciDeposu()
        {
            // Başlangıç (seed) verisi - yeni rol yapısına göre güncellendi
            _kullanicilar = new List<Kullanici>
            {
                new Kullanici {
                    KullaniciId = 1, AdSoyad = "Ahmet Yılmaz", KullaniciAdi = "ahmety",
                    EPosta = "ahmet@kcetas.com", Durum = "AKTIF", RolId = 5,
                    Rol = RolListesi.BulRolId(5), CreatedAt = DateTime.Now // Saha Operasyon Amiri
                },
                new Kullanici {
                    KullaniciId = 2, AdSoyad = "Ayşe Demir", KullaniciAdi = "aysed",
                    EPosta = "ayse@kcetas.com", Durum = "AKTIF", RolId = 2,
                    Rol = RolListesi.BulRolId(2), CreatedAt = DateTime.Now // Müşteri Temsilcisi
                },
                new Kullanici {
                    KullaniciId = 3, AdSoyad = "Sistem Yöneticisi", KullaniciAdi = "admin",
                    EPosta = "admin@kcetas.com", Durum = "AKTIF", RolId = 1,
                    Rol = RolListesi.BulRolId(1), CreatedAt = DateTime.Now // BT Yöneticisi
                }
            };
            _sonId = _kullanicilar.Max(k => k.KullaniciId);
        }

        public bool KullaniciAdiVarMi(string kullaniciAdi)
        {
            lock (_kilit)
            {
                return _kullanicilar.Any(k =>
                    k.KullaniciAdi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase));
            }
        }

        public Kullanici Ekle(Kullanici kullanici)
        {
            lock (_kilit)
            {
                _sonId++;
                kullanici.KullaniciId = _sonId;
                kullanici.Rol = RolListesi.BulRolId(kullanici.RolId);
                _kullanicilar.Add(kullanici);
                return kullanici;
            }
        }

        public Kullanici BulKullaniciAdiIle(string kullaniciAdi)
        {
            lock (_kilit)
            {
                return _kullanicilar.FirstOrDefault(k =>
                    k.KullaniciAdi.Equals(kullaniciAdi, StringComparison.OrdinalIgnoreCase));
            }
        }

        public List<Kullanici> Listele()
        {
            lock (_kilit)
            {
                return _kullanicilar.ToList();
            }
        }

        public Kullanici BulId(long id)
        {
            lock (_kilit)
            {
                return _kullanicilar.FirstOrDefault(k => k.KullaniciId == id);
            }
        }

        public bool Guncelle(Kullanici guncel)
        {
            lock (_kilit)
            {
                var mevcut = _kullanicilar.FirstOrDefault(k => k.KullaniciId == guncel.KullaniciId);
                if (mevcut == null) return false;

                mevcut.AdSoyad = guncel.AdSoyad;
                mevcut.KullaniciAdi = guncel.KullaniciAdi;
                mevcut.EPosta = guncel.EPosta;
                mevcut.Durum = guncel.Durum;
                mevcut.RolId = guncel.RolId;
                mevcut.Rol = RolListesi.BulRolId(guncel.RolId);
                mevcut.UpdatedAt = DateTime.Now;
                return true;
            }
        }

        public bool Sil(long id)
        {
            lock (_kilit)
            {
                var kullanici = _kullanicilar.FirstOrDefault(k => k.KullaniciId == id);
                if (kullanici == null) return false;
                _kullanicilar.Remove(kullanici);
                return true;
            }
        }
    }
}