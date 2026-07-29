using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using BCrypt.Net;

using System.Threading.Tasks;

namespace KcetasWeb.Controllers
{
    [Authorize]
    public class ProfilController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;
        private readonly PasswordHasher<Kullanici> _sifreHasher = new();

        public ProfilController(IKullaniciDeposu kullaniciDeposu)
        {
            _kullaniciDeposu = kullaniciDeposu;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Kullanıcıyı NameIdentifier claim'inden (kullanici_adi) buluyoruz
            var kullaniciAdi = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                TempData["HataMesaji"] = "Kullanıcı bilgileri alınamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Auth");
            }

            var kullanici = await _kullaniciDeposu.BulKullaniciAdiIleAsync(kullaniciAdi);
            
            if (kullanici == null)
            {
                // Kullanıcı veritabanında yoksa, büyük ihtimalle "admin" gibi hardcoded bir test hesabıdır.
                ViewBag.IsTestAccount = true;
                return View(new ProfilViewModel
                {
                    AdSoyad = User.Identity?.Name ?? kullaniciAdi,
                    KullaniciAdi = kullaniciAdi,
                    EPosta = "sistem-hesabi@kcetas.com.tr"
                });
            }

            var model = new ProfilViewModel
            {
                AdSoyad = kullanici.ad_soyad,
                KullaniciAdi = kullanici.kullanici_adi,
                EPosta = kullanici.e_posta
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Guncelle(ProfilViewModel model)
        {
            var kullaniciAdi = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciAdi)) return RedirectToAction("Login", "Auth");

            var kullanici = await _kullaniciDeposu.BulKullaniciAdiIleAsync(kullaniciAdi);
            if (kullanici == null)
            {
                TempData["HataMesaji"] = "Sistem/Test hesabı ile giriş yaptığınız için bu bilgileri güncelleyemezsiniz.";
                return RedirectToAction("Index");
            }

            kullanici.ad_soyad = model.AdSoyad;
            kullanici.e_posta = model.EPosta;
            
            // Not: Kullanıcı adını değiştirmelerine genellikle izin verilmez, ancak izin vermek isterseniz buraya eklenebilir.

            bool sonuc = await _kullaniciDeposu.GuncelleAsync(kullanici);

            if (sonuc)
            {
                TempData["BasariMesaji"] = "Profil bilgileriniz başarıyla güncellendi.";
            }
            else
            {
                TempData["HataMesaji"] = "Profil güncellenirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SifreDegistir(ProfilViewModel model)
        {
            var kullaniciAdi = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciAdi)) return RedirectToAction("Login", "Auth");

            var kullanici = await _kullaniciDeposu.BulKullaniciAdiIleAsync(kullaniciAdi);
            if (kullanici == null)
            {
                TempData["HataMesaji"] = "Sistem/Test hesabı ile giriş yaptığınız için şifre değiştiremezsiniz.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.EskiSifre) || string.IsNullOrEmpty(model.YeniSifre) || string.IsNullOrEmpty(model.YeniSifreTekrar))
            {
                TempData["HataMesaji"] = "Lütfen şifre alanlarını eksiksiz doldurun.";
                return RedirectToAction("Index");
            }

            if (model.YeniSifre != model.YeniSifreTekrar)
            {
                TempData["HataMesaji"] = "Yeni şifreler birbirleriyle eşleşmiyor.";
                return RedirectToAction("Index");
            }

            // Eski şifre doğrulaması (Güvenlik nedeniyle API üzerinden Login testi yaparak)
            bool sifreDogruMu = await _kullaniciDeposu.GirisKontrolAsync(kullanici.kullanici_adi, model.EskiSifre);
            
            // Eğer API üzerinden doğrulanamazsa, düz metin yedeğini kontrol edelim (Test hesapları için)
            if (!sifreDogruMu)
            {
                if (kullanici.Sifre == model.EskiSifre || (kullanici.sifre_hash != null && kullanici.sifre_hash == model.EskiSifre))
                {
                    sifreDogruMu = true;
                }
            }

            if (!sifreDogruMu)
            {
                TempData["HataMesaji"] = "Eski şifrenizi yanlış girdiniz.";
                return RedirectToAction("Index");
            }

            // Yeni şifreyi hashleyip kaydetme
            kullanici.sifre_hash = _sifreHasher.HashPassword(kullanici, model.YeniSifre);
            
            bool sonuc = await _kullaniciDeposu.GuncelleAsync(kullanici);

            if (sonuc)
            {
                TempData["BasariMesaji"] = "Şifreniz başarıyla değiştirildi.";
            }
            else
            {
                TempData["HataMesaji"] = "Şifreniz değiştirilirken sistemsel bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }
    }
}
