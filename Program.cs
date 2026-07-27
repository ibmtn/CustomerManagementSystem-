using Microsoft.AspNetCore.Authentication.Cookies;
using KcetasWeb.Services.Interfaces;

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
var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://172.10.38.28:5050";
Action<HttpClient> configureClient = client => 
{
    client.BaseAddress = new Uri(baseUrl);
};

builder.Services.AddHttpClient<IIsEmriService, KcetasWeb.Services.Api.ApiIsEmriService>(configureClient);
builder.Services.AddHttpClient<IEndeksOkumaService, KcetasWeb.Services.Api.ApiEndeksOkumaService>(configureClient);
builder.Services.AddHttpClient<IFaturaService, KcetasWeb.Services.Api.ApiFaturaService>(configureClient);
builder.Services.AddHttpClient<IOutboxService, KcetasWeb.Services.Api.ApiOutboxService>(configureClient);
builder.Services.AddHttpClient<IKullaniciDeposu, KcetasWeb.Services.Api.ApiKullaniciDeposu>(configureClient);
builder.Services.AddHttpClient<ITuketimNoktasiService, KcetasWeb.Services.Api.ApiTuketimNoktasiService>(configureClient);
builder.Services.AddHttpClient<ISozlesmeService, KcetasWeb.Services.Api.ApiSozlesmeService>(configureClient);
builder.Services.AddHttpClient<ISayacService, KcetasWeb.Services.Api.ApiSayacService>(configureClient);
builder.Services.AddHttpClient<IAboneService, KcetasWeb.Services.Api.ApiAboneService>(configureClient);
builder.Services.AddHttpClient<IAuditLogService, KcetasWeb.Services.Api.ApiAuditLogService>(configureClient);

// 3. ARKA PLAN SERVİSLERİ (BACKGROUND SERVICES)
builder.Services.AddHostedService<KcetasWeb.Services.Background.IsEmriOlusturucuBackgroundService>();

var app = builder.Build();

// 2.5 HATA YÖNETİMİ (EXCEPTION HANDLING)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}
else 
{
    // Geliştirme ortamında bile şık hata sayfasını görmek için zorunlu yönlendirme eklenebilir, 
    // ama şimdilik production'da /Error sayfasına gitsin, Development'ta DeveloperExceptionPage çıksın.
    // Eğer isterseniz her ortamda /Error için yorum satırından çıkarın:
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

// 3. KİMLİK SORGULAMA VE YETKİLENDİRME (Bu ikisinin sırası çok önemlidir!)
app.UseAuthentication(); // Kimlik sor
app.UseAuthorization();  // Yetkisi var mı bak

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();