using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Sisteme MVC Controller yapılarını ekliyoruz
builder.Services.AddControllersWithViews();

// 1. SİSTEME COOKIE (ÇEREZ) KİMLİK DOĞRULAMASINI TANITIYORUZ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(ayarlar =>
    {
        ayarlar.LoginPath = "/Auth/Login"; // Yetkisiz biri girerse buraya at
        ayarlar.LogoutPath = "/Auth/Logout"; // Çıkış yapınca buraya at
    });

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