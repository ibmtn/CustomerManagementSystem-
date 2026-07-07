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
                .OrderByDescending(k => k.created_at)
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
                ad_soyad = AdSoyad,
                kullanici_adi = KullaniciAdi,
                e_posta = EPosta,
                rol_id = RolId,
                durum = "AKTIF",
                created_at = DateTime.Now
            };

            var hasher = new PasswordHasher<Kullanici>();
            yeniKullanici.sifre_hash = hasher.HashPassword(yeniKullanici, Sifre);

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
        public IActionResult Duzenle(Kullanici model)
        {
            var mevcut = _kullaniciDeposu.BulId(model.kullanici_id);
            if (mevcut == null) return NotFound();

            mevcut.ad_soyad = model.ad_soyad;
            mevcut.kullanici_adi = model.kullanici_adi;
            mevcut.e_posta = model.e_posta;
            mevcut.durum = model.durum;
            mevcut.rol_id = model.rol_id;

            _kullaniciDeposu.Guncelle(mevcut);

            TempData["PersonelMesaji"] = mevcut.ad_soyad + " isimli kullanıcı başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.kullanici_id });
        }

        public IActionResult Sil(long id)
        {
            var kullanici = _kullaniciDeposu.BulId(id);
            if (kullanici != null)
            {
                _kullaniciDeposu.Sil(id);
                TempData["PersonelMesaji"] = kullanici.ad_soyad + " isimli kullanıcı başarıyla silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}