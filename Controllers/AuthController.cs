using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using KcetasWeb.Models;                 // RegisterViewModel için
using KcetasWeb.Models.entities;        // Kullanici, Rol için
using KcetasWeb.Services.Interfaces;    // IKullaniciDeposu, RolListesi için
using KcetasWeb.Models.entities;              // AppRoles sınıfı için

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
            // --- 1. GELİŞTİRİCİ TEST KULLANICILARI ---
            // Şifre 123 ise girilen kullanıcı adına göre ilgili rol atanır
            if (sifre == "123")
            {
                switch (kullaniciAdi.ToLower())
                {
                    case "admin":
                    case "bt":
                        await GirisYap("BT Yöneticisi", AppRoles.BTYoneticisi);
                        return RedirectToAction("Index", "Home");
                    
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

            // --- 2. VERİTABANI KULLANICILARI ---
            // Test kullanıcısı değilse veritabanından (Mock servisinden) doğrula
            var kayitliKullanici = _kullaniciDeposu.BulKullaniciAdiIle(kullaniciAdi);
            if (kayitliKullanici != null)
            {
                var sonuc = _sifreHasher.VerifyHashedPassword(kayitliKullanici, kayitliKullanici.SifreHash, sifre);
                if (sonuc == PasswordVerificationResult.Success)
                {
                    // RolListesi'nden gelen rolü alıyoruz. Bulunamazsa varsayılan rol atanır.
                    var rol = RolListesi.BulRolId(kayitliKullanici.RolId);
                    var rolAdi = rol?.RolAdi ?? AppRoles.MusteriTemsilcisi;

                    await GirisYap(kayitliKullanici.AdSoyad, rolAdi);
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["HataMesaji"] = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        private async Task GirisYap(string ad, string rol)
        {
            var kartBilgileri = new List<Claim>
            {
                new Claim(ClaimTypes.Name, ad),
                new Claim(ClaimTypes.Role, rol)
            };

            var kullaniciKimligi = new ClaimsIdentity(kartBilgileri, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(kullaniciKimligi));
        }

        public IActionResult Register()
        {
            // ID'si 1 olanı (BT Yoneticisi) kayıt ekranında gizliyoruz
            ViewBag.Roller = RolListesi.Roller.Where(r => r.RolId != 1).ToList();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            ViewBag.Roller = RolListesi.Roller.Where(r => r.RolId != 1).ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_kullaniciDeposu.KullaniciAdiVarMi(model.KullaniciAdi))
            {
                ModelState.AddModelError(nameof(model.KullaniciAdi), "Bu kullanıcı adı zaten alınmış.");
                return View(model);
            }

            var yeniKullanici = new Kullanici
            {
                AdSoyad = model.AdSoyad,
                EPosta = model.EPosta,
                KullaniciAdi = model.KullaniciAdi,
                RolId = model.RolId,
                AboneTuru = null, // Artık abone olmadığı için bu alan hep boş kalacak
                Durum = "Aktif",
                CreatedAt = DateTime.Now
            };

            yeniKullanici.SifreHash = _sifreHasher.HashPassword(yeniKullanici, model.Sifre);

            _kullaniciDeposu.Ekle(yeniKullanici);

            TempData["BasariMesaji"] = "Hesabınız oluşturuldu! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Yetkisi olmayan bir sayfaya girmeye çalışınca gösterilecek sayfa
        [HttpGet]
        public IActionResult Yetkisiz()
        {
            return View(); 
        }
    }
}