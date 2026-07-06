using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = AppRoles.FaturalamaUzmani + "," + AppRoles.BTYoneticisi)]
    public class AdminController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;

        public AdminController(IKullaniciDeposu kullaniciDeposu)
        {
            _kullaniciDeposu = kullaniciDeposu;
        }

        public IActionResult Index()
        {
            var liste = _kullaniciDeposu.Listele()
                .OrderByDescending(k => k.CreatedAt)
                .ToList();
            return View(liste);
        }

        public IActionResult Yeni()
        {
            ViewBag.Roller = RolListesi.Roller;
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(string AdSoyad, string KullaniciAdi, string EPosta, string Sifre, short RolId)
        {
            if (_kullaniciDeposu.KullaniciAdiVarMi(KullaniciAdi))
            {
                TempData["PersonelMesaji"] = "Bu kullanıcı adı zaten kullanılıyor.";
                return RedirectToAction("Yeni");
            }

            var yeniKullanici = new Kullanici
            {
                AdSoyad = AdSoyad,
                KullaniciAdi = KullaniciAdi,
                EPosta = EPosta,
                RolId = RolId,
                Durum = "AKTIF",
                CreatedAt = DateTime.Now
            };

            var hasher = new PasswordHasher<Kullanici>();
            yeniKullanici.SifreHash = hasher.HashPassword(yeniKullanici, Sifre);

            _kullaniciDeposu.Ekle(yeniKullanici);

            TempData["PersonelMesaji"] = AdSoyad + " isimli kullanıcı sisteme başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(long id)
        {
            var kullanici = _kullaniciDeposu.BulId(id);
            if (kullanici == null) return NotFound();
            return View(kullanici);
        }

        public IActionResult Duzenle(long id)
        {
            var kullanici = _kullaniciDeposu.BulId(id);
            if (kullanici == null) return NotFound();
            ViewBag.Roller = RolListesi.Roller;
            return View(kullanici);
        }

        [HttpPost]
        public IActionResult Duzenle(long KullaniciId, string AdSoyad, string KullaniciAdi, string EPosta, string Durum, short RolId)
        {
            var guncellenecek = new Kullanici
            {
                KullaniciId = KullaniciId,
                AdSoyad = AdSoyad,
                KullaniciAdi = KullaniciAdi,
                EPosta = EPosta,
                Durum = Durum,
                RolId = RolId
            };

            _kullaniciDeposu.Guncelle(guncellenecek);

            TempData["PersonelMesaji"] = AdSoyad + " isimli kullanıcı başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = KullaniciId });
        }

        public IActionResult Sil(long id)
        {
            var kullanici = _kullaniciDeposu.BulId(id);
            if (kullanici != null)
            {
                _kullaniciDeposu.Sil(id);
                TempData["PersonelMesaji"] = kullanici.AdSoyad + " isimli kullanıcı başarıyla silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}