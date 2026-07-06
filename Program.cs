using Microsoft.AspNetCore.Authentication.Cookies;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Services.Mock;
using KcetasWeb.Services.Interfaces;




var builder = WebApplication.CreateBuilder(args);

// Sisteme MVC Controller yapılarını ekliyoruz
builder.Services.AddControllersWithViews();

// 1. SİSTEME COOKIE (ÇEREZ) KİMLİK DOĞRULAMASINI TANITIYORUZ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(ayarlar =>
    {
        ayarlar.LoginPath = "/Auth/Login"; // Yetkisiz biri girerse buraya at
        ayarlar.LogoutPath = "/Auth/Logout"; // Çıkış yapınca buraya at
        ayarlar.AccessDeniedPath = "/Auth/Yetkisiz";
    });




// 2. MOCK SERVİSLERİN DI CONTAINER'A KAYDI
// İleride gerçek veritabanı servisleriyle değiştirilecek
builder.Services.AddSingleton<IIsEmriService, MockIsEmriService>();
builder.Services.AddSingleton<IEndeksOkumaService, MockEndeksOkumaService>();
builder.Services.AddSingleton<IFaturaService, MockFaturaService>();
builder.Services.AddSingleton<IOutboxService, MockOutboxService>();
builder.Services.AddSingleton<KcetasWeb.Services.Interfaces.IKullaniciDeposu, KcetasWeb.Services.Interfaces.KullaniciDeposu>();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// 2. KİMLİK SORGULAMA VE YETKİLENDİRME (Bu ikisinin sırası çok önemlidir!)
app.UseAuthentication(); // Kimlik sor
app.UseAuthorization();  // Yetkisi var mı bak

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();




