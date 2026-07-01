using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;

namespace KcetasWeb.Controllers
{
    // Sadece "Yonetici" rolüne sahip olanlar bu Controller'a girebilir!
    [Authorize(Roles = "Yonetici")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            // Veritabanı bağlanana kadar, arkadaşının tablolarını simüle eden geçici veriler:
            var kullanicilar = new List<Kullanici>
            {
                new Kullanici { 
                    KullaniciId = 1, AdSoyad = "Ahmet Yılmaz", KullaniciAdi = "ahmety", 
                    EPosta = "ahmet@kcetas.com", Durum = "AKTIF", 
                    Rol = new Rol { RolAdi = "Kullanici" } 
                },
                new Kullanici { 
                    KullaniciId = 2, AdSoyad = "Ayşe Demir", KullaniciAdi = "aysed", 
                    EPosta = "ayse@kcetas.com", Durum = "AKTIF", 
                    Rol = new Rol { RolAdi = "Gişe Memuru" } 
                },
                new Kullanici { 
                    KullaniciId = 3, AdSoyad = "Sistem Yöneticisi", KullaniciAdi = "admin", 
                    EPosta = "admin@kcetas.com", Durum = "AKTIF", 
                    Rol = new Rol { RolAdi = "Yonetici" } 
                }
            };
            

            // Veriyi ekrana (View'a) gönderiyoruz
            return View(kullanicilar);
        }
        // Yeni Kayıt Formunu Açan Metot
        public IActionResult Yeni()
        {
            return View();
        }

        // Formdan Gelen Veriyi Yakalayan Metot
        [HttpPost]
        public IActionResult Yeni(string AdSoyad, string KullaniciAdi, string EPosta, string Sifre, short RolId)
        {
            // İbrahimler veritabanını bağladığında burada Hash işlemi ve DB kayıt kodu olacak.
            // Şimdilik başarılı sayıp yeşil mesajla listeye dönüyoruz:
            TempData["PersonelMesaji"] = AdSoyad + " isimli personel sisteme başarıyla eklendi.";
            return RedirectToAction("Index");
        }
    }
}