using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace KcetasWeb.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        // Metodu async (asenkron) yapıyoruz çünkü çerez oluşturma işlemi zaman alır
        [HttpPost]
public async Task<IActionResult> Login(string kullaniciAdi, string sifre)
{
    // 1. KULLANICI: YÖNETİCİ (Admin)
    if (kullaniciAdi == "admin" && sifre == "123")
    {
        var kartBilgileri = new List<Claim>
        {
            new Claim(ClaimTypes.Name, kullaniciAdi),
            new Claim(ClaimTypes.Role, "Yonetici") // Rolü: Yönetici
        };

        var kullaniciKimligi = new ClaimsIdentity(kartBilgileri, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(kullaniciKimligi));

        return RedirectToAction("Index", "Home");
    }
    // 2. KULLANICI: NORMAL PERSONEL (Gişe/Saha)
    else if (kullaniciAdi == "personel" && sifre == "123")
    {
        var kartBilgileri = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Ahmet (Gişe)"), // Ekranda görünecek isim
            new Claim(ClaimTypes.Role, "Kullanici") // Rolü: Normal Kullanıcı
        };

        var kullaniciKimligi = new ClaimsIdentity(kartBilgileri, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(kullaniciKimligi));

        return RedirectToAction("Index", "Home");
    }
    // 3. HATALI GİRİŞ DURUMU
    else
    {
        TempData["HataMesaji"] = "Kullanıcı adı veya şifre hatalı!";
        return View();
    }
}

        // Çıkış Yapma Metodu
        public async Task<IActionResult> Logout()
        {
            // Sisteme basılan çerezi yırtıp atıyoruz
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}