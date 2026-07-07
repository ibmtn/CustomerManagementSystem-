using Microsoft.AspNetCore.Authentication.Cookies;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Services.Mock;

var builder = WebApplication.CreateBuilder(args);

// Sisteme MVC Controller yapılarını ekliyoruz
builder.Services.AddControllersWithViews();

// 1. SİSTEME COOKIE (ÇEREZ) KİMLİK DOĞRULAMASINI TANITIYORUZ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(ayarlar =>
    {
        ayarlar.LoginPath = "/Auth/Login";       // Yetkisiz biri girerse buraya at
        ayarlar.LogoutPath = "/Auth/Logout";     // Çıkış yapınca buraya at
        ayarlar.AccessDeniedPath = "/Auth/Yetkisiz";
    });

// 2. API SERVİSLERİNİN DI CONTAINER'A KAYDI
var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://172.10.38.23:5050";

builder.Services.AddHttpClient<IIsEmriService, KcetasWeb.Services.Api.ApiIsEmriService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IEndeksOkumaService, KcetasWeb.Services.Api.ApiEndeksOkumaService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IFaturaService, KcetasWeb.Services.Api.ApiFaturaService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IOutboxService, KcetasWeb.Services.Api.ApiOutboxService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IKullaniciDeposu, KcetasWeb.Services.Api.ApiKullaniciDeposu>(client => client.BaseAddress = new Uri(baseUrl));

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// 3. KİMLİK SORGULAMA VE YETKİLENDİRME (Bu ikisinin sırası çok önemlidir!)
app.UseAuthentication(); // Kimlik sor
app.UseAuthorization();  // Yetkisi var mı bak

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();