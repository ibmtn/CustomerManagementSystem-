using Microsoft.AspNetCore.Authentication.Cookies;
using MusteriYonetim.Services.Interfaces;
using Serilog;
using MusteriYonetim.Filters;

var builder = WebApplication.CreateBuilder(args);

// Serilog Yapılandırması
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/web-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Sisteme MemoryCache ekliyoruz (Tüketim noktaları gibi büyük tabloları önbelleğe almak için)
builder.Services.AddMemoryCache();
builder.Services.AddResponseCompression(options => { options.EnableForHttps = true; });

// Sisteme MVC Controller yapılarını ve Global Exception Filter'ı ekliyoruz
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// 1. SİSTEME COOKIE (ÇEREZ) KİMLİK DOĞRULAMASINI TANITIYORUZ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(ayarlar =>
    {
        ayarlar.LoginPath = "/Auth/Login";       // Yetkisiz biri girerse buraya at
        ayarlar.LogoutPath = "/Auth/Logout";     // Çıkış yapınca buraya at
        ayarlar.AccessDeniedPath = "/Auth/Yetkisiz";
    });

// 2. API SERVİSLERİNİN DI CONTAINER'A KAYDI
// API adresi koda veya depoya yazılmaz; user-secrets ya da ApiSettings__BaseUrl ortam değişkeniyle verilir.
var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var apiBaseUri))
{
    throw new InvalidOperationException(
        "ApiSettings:BaseUrl ayarı eksik veya geçersiz. API adresini 'dotnet user-secrets set ApiSettings:BaseUrl <adres>' veya ApiSettings__BaseUrl ortam değişkeniyle verin.");
}
Action<HttpClient> configureClient = client =>
{
    client.BaseAddress = apiBaseUri;
};

builder.Services.AddHttpClient<IIsEmriService, MusteriYonetim.Services.Api.ApiIsEmriService>(configureClient);
builder.Services.AddHttpClient<IEndeksOkumaService, MusteriYonetim.Services.Api.ApiEndeksOkumaService>(configureClient);
builder.Services.AddHttpClient<IFaturaService, MusteriYonetim.Services.Api.ApiFaturaService>(configureClient);
builder.Services.AddHttpClient<IOutboxService, MusteriYonetim.Services.Api.ApiOutboxService>(configureClient);
builder.Services.AddHttpClient<IKullaniciDeposu, MusteriYonetim.Services.Api.ApiKullaniciDeposu>(configureClient);
builder.Services.AddHttpClient<ITuketimNoktasiService, MusteriYonetim.Services.Api.ApiTuketimNoktasiService>(configureClient);
builder.Services.AddHttpClient<ISozlesmeService, MusteriYonetim.Services.Api.ApiSozlesmeService>(configureClient);
builder.Services.AddHttpClient<ISayacService, MusteriYonetim.Services.Api.ApiSayacService>(configureClient);
builder.Services.AddHttpClient<IAboneService, MusteriYonetim.Services.Api.ApiAboneService>(configureClient);
builder.Services.AddHttpClient<IAuditLogService, MusteriYonetim.Services.Api.ApiAuditLogService>(configureClient);

// 3. ARKA PLAN SERVİSLERİ (BACKGROUND SERVICES)
builder.Services.AddHostedService<MusteriYonetim.Services.Background.IsEmriOlusturucuBackgroundService>();

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
    app.UseExceptionHandler("/Error");
}

app.UseResponseCompression();
app.UseStaticFiles();
app.UseRouting();

// 3. KİMLİK SORGULAMA VE YETKİLENDİRME (Bu ikisinin sırası çok önemlidir!)
app.UseAuthentication(); // Kimlik sor
app.UseAuthorization();  // Yetkisi var mı bak

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
