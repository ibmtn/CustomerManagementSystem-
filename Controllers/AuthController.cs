using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;
using BCrypt.Net;

namespace KcetasWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;
        private readonly IAuditLogService _auditLogService;
        private readonly PasswordHasher<Kullanici> _sifreHasher = new();

        public AuthController(IKullaniciDeposu kullaniciDeposu, IAuditLogService auditLogService)
        {
            _kullaniciDeposu = kullaniciDeposu;
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string kullaniciAdi, string sifre)
        {
            kullaniciAdi = kullaniciAdi?.Trim() ?? "";

            var kayitliKullanici = await _kullaniciDeposu.BulKullaniciAdiIleAsync(kullaniciAdi);

            if (kayitliKullanici != null)
            {
                PasswordVerificationResult sonuc = PasswordVerificationResult.Failed;
                
                // API tarafı Case-Sensitive olduğu için, kullanıcının girdiği (küçük harfli vs) adı değil,
                // Veritabanından (liste üzerinden) gelen birebir (orijinal büyük/küçük harfli) kullanıcı adını gönderiyoruz.
                string orjinalKullaniciAdi = kayitliKullanici.kullanici_adi ?? kullaniciAdi;
                bool apiGirisBasarili = await _kullaniciDeposu.GirisKontrolAsync(orjinalKullaniciAdi, sifre);
                
                if (apiGirisBasarili)
                {
                    sonuc = PasswordVerificationResult.Success;
                }

                if (sonuc == PasswordVerificationResult.Success || sonuc == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    var rol = RolListesi.BulRolId(kayitliKullanici.rol_id ?? 0);
                    var rolAdi = rol?.rol_adi ?? AppRoles.MusteriTemsilcisi;

                    await GirisYap(kayitliKullanici.ad_soyad, rolAdi, kayitliKullanici.kullanici_adi);
                    
                    await _auditLogService.EkleAsync("Kullanici", kayitliKullanici.kullanici_id, "LOGIN", "", "Sisteme giriş yapıldı.", kayitliKullanici.kullanici_id, "Başarılı Kullanıcı Girişi");

                    if (rolAdi == AppRoles.BTYoneticisi || rolAdi == "Yonetici")
                        return RedirectToAction("Index", "Dashboard");

                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["HataMesaji"] = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        private async Task GirisYap(string ad, string rol, string kullaniciAdi)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, ad),
                new Claim(ClaimTypes.Role, rol),
                new Claim(ClaimTypes.NameIdentifier, kullaniciAdi)
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

        public async Task<IActionResult> Register()
        {
            ViewBag.Roller = RolListesi.Roller
                .Where(r => r.rol_id != 1)
                .ToList();

            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewBag.Roller = RolListesi.Roller
                .Where(r => r.rol_id != 1)
                .ToList();

            if (!ModelState.IsValid)
                return View(model);

            if (await _kullaniciDeposu.KullaniciAdiVarMiAsync(model.KullaniciAdi))
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
                durum = KcetasWeb.Models.Enums.KullaniciDurumu.Aktif,
                created_at = DateTime.Now,
                Sifre = model.Sifre
            };

            await _kullaniciDeposu.EkleAsync(yeniKullanici);

            // Log registration (using newly generated ID or 0 if not auto-assigned)
            await _auditLogService.EkleAsync("Kullanici", yeniKullanici.kullanici_id, "REGISTER", "", "Yeni kullanıcı hesabı açıldı.", yeniKullanici.kullanici_id, "Sistem Kayıt");

            TempData["BasariMesaji"] = "Hesabınız oluşturuldu! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await _kullaniciDeposu.BulKullaniciAdiIleAsync(userName);
                if (user != null)
                {
                    await _auditLogService.EkleAsync("Kullanici", user.kullanici_id, "LOGOUT", "", "Sistemden güvenli çıkış yapıldı.", user.kullanici_id, "Kullanıcı Çıkışı");
                }
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Yetkisiz()
        {
            return View();
        }
    }
}