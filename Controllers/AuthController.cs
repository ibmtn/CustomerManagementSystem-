using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;
        private readonly PasswordHasher<Kullanici> _sifreHasher = new();

        public AuthController(IKullaniciDeposu kullaniciDeposu)
        {
            _kullaniciDeposu = kullaniciDeposu;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string kullaniciAdi, string sifre)
        {
            if (sifre == "123")
            {
                switch (kullaniciAdi.ToLower())
                {
                    case "admin":
                    case "bt":
                        await GirisYap("BT Yöneticisi", AppRoles.BTYoneticisi);
                        return RedirectToAction("Index", "Dashboard");

                    case "musteri":
                        await GirisYap("Müşteri Temsilcisi", AppRoles.MusteriTemsilcisi);
                        return RedirectToAction("Index", "Home");

                    case "sozlesme":
                        await GirisYap("Sözleşme Yetkilisi", AppRoles.SozlesmeYetkilisi);
                        return RedirectToAction("Index", "Home");

                    case "sayac":
                        await GirisYap("Sayaç Okuma Personeli", AppRoles.SayacOkumaPersoneli);
                        return RedirectToAction("Index", "Home");

                    case "saha":
                        await GirisYap("Saha Operasyon Amiri", AppRoles.SahaOperasyonAmiri);
                        return RedirectToAction("Index", "Home");

                    case "fatura":
                        await GirisYap("Faturalama Uzmanı", AppRoles.FaturalamaUzmani);
                        return RedirectToAction("Index", "Home");

                    case "denetci":
                        await GirisYap("Denetçi Personel", AppRoles.Denetci);
                        return RedirectToAction("Index", "Home");
                }
            }

            var kayitliKullanici = _kullaniciDeposu.BulKullaniciAdiIle(kullaniciAdi);

            if (kayitliKullanici != null)
            {
                var sonuc = _sifreHasher.VerifyHashedPassword(
                    kayitliKullanici,
                    kayitliKullanici.sifre_hash,
                    sifre
                );

                if (sonuc == PasswordVerificationResult.Success)
                {
                    var rol = RolListesi.BulRolId(kayitliKullanici.rol_id);
                    var rolAdi = rol?.rol_adi ?? AppRoles.MusteriTemsilcisi;

                    await GirisYap(kayitliKullanici.ad_soyad, rolAdi);

                    if (rolAdi == AppRoles.BTYoneticisi || rolAdi == "Yonetici")
                        return RedirectToAction("Index", "Dashboard");

                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["HataMesaji"] = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        private async Task GirisYap(string ad, string rol)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, ad),
                new Claim(ClaimTypes.Role, rol)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );
        }

        public IActionResult Register()
        {
            ViewBag.Roller = RolListesi.Roller
                .Where(r => r.rol_id != 1)
                .ToList();

            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            ViewBag.Roller = RolListesi.Roller
                .Where(r => r.rol_id != 1)
                .ToList();

            if (!ModelState.IsValid)
                return View(model);

            if (_kullaniciDeposu.KullaniciAdiVarMi(model.KullaniciAdi))
            {
                ModelState.AddModelError(nameof(model.KullaniciAdi), "Bu kullanıcı adı zaten alınmış.");
                return View(model);
            }

            var yeniKullanici = new Kullanici
            {
                ad_soyad = model.AdSoyad,
                e_posta = model.EPosta,
                kullanici_adi = model.KullaniciAdi,
                rol_id = model.RolId,
                durum = "Aktif",
                created_at = DateTime.Now
            };

            yeniKullanici.sifre_hash = _sifreHasher.HashPassword(yeniKullanici, model.Sifre);

            _kullaniciDeposu.Ekle(yeniKullanici);

            TempData["BasariMesaji"] = "Hesabınız oluşturuldu! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Yetkisiz()
        {
            return View();
        }
    }
}