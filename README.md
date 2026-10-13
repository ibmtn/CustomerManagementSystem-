# Müşteri Yönetim Sistemi — Web Arayüzü

Bir elektrik dağıtım şirketi için geliştirilen müşteri yönetim sisteminin **web arayüzü**.
Aboneler, tüketim noktaları, sözleşmeler, sayaçlar, endeks okumaları, faturalar ve saha iş emirleri tek bir portaldan yönetilir.

> **Bu depo hakkında:** Staj projesinin portfolyo amaçlı kopyasıdır. Şirkete ve projeye özel her şey
> (şirket adı ve logoları, iç ağ adresleri, test verileri, lisanslı tema dosyaları ve commit geçmişi) kaldırılmış,
> şirket adı yerine nötr **MYS** (Müşteri Yönetim Sistemi) adı kullanılmıştır.

## Ekip ve rolüm

- Staj döneminde (2026) **9 kişilik bir ekiple** geliştirildi.
- **Web arayüzünü** bir ekip arkadaşımla birlikte ben geliştirdim — bu depodaki kod.
- **Arka uç API'si** ve **mobil uygulama** ekibin diğer üyelerinin sorumluluğundaydı; bu depoda yer almaz.

## Modüller

| Alan | Ekranlar |
|---|---|
| Müşteri | Abone kayıt/düzenleme/sorgulama, tüketim noktaları, sözleşmeler |
| Ölçüm | Sayaçlar, endeks okuma, okuma tutanağı yazdırma |
| Faturalama | Fatura listesi ve detayı, fatura simülasyonu |
| Saha | İş emirleri, iş emri tamamlama ve tutanak |
| Yönetim | Panel (dashboard), kullanıcı yönetimi, belgeler, denetim kaydı (audit log), entegrasyon kuyruğu (outbox) |

## Öne çıkanlar

- **Rol bazlı yetkilendirme:** Yedi kullanıcı rolü (BT yöneticisi, müşteri temsilcisi, sözleşme yetkilisi, sayaç okuma personeli, saha operasyon amiri, faturalama uzmanı, denetçi). Rol adları `AppRoles` sabitleriyle yazılır; yanlış yazım derleme hatası verir.
- **KVKK:** TC kimlik, telefon ve e-posta gibi kişisel veriler ekranda maskelenerek gösterilir (`Extensions/StringKvkkExtensions.cs`).
- **Otomatik iş emri:** Arka plan servisi, son okumasının üzerinden 25 gün geçen sayaçlar için endeks okuma iş emri açar ve saha personeline sırayla (round-robin) atar.
- **Katmanlı yapı:** Controller'lar yalnızca arayüzlere (`Services/Interfaces`) bağımlıdır; API'ye erişen uygulamalar (`Services/Api`) tipli `HttpClient` ile bağımlılık enjeksiyonu üzerinden verilir.
- **Hata yönetimi ve kayıt:** Global exception filter, Serilog ile günlük dosyalarına kayıt, yanıt sıkıştırma.

## Teknolojiler

ASP.NET Core MVC (.NET 9) · Razor · Cookie Authentication · REST API entegrasyonu (HttpClient) · Serilog · Bootstrap 5 · jQuery Validation

## Çalıştırma

Uygulama tek başına çalışmaz; ekibin geliştirdiği arka uç API'sine bağlanır.

```bash
# API adresi depoya yazılmaz; bilgisayarınızda gizli ayar olarak saklanır
dotnet user-secrets set ApiSettings:BaseUrl "http://<api-sunucusu>:<port>"
dotnet run
```

Adres verilmezse uygulama açık bir hata mesajıyla durur.

**Not:** Arayüz, lisanslı bir Bootstrap 5 yönetim paneli temasının dosyalarını (`wwwroot/assets/`) kullanır.
Bu dosyalar lisans gereği depoda yoktur; olmadan sayfalar çalışır ancak stilsiz görünür.

## Güvenlik notları

- `.gitignore`, tema dosyalarının, kurum logolarının, API yanıt dökümlerinin ve yerel ayarların depoya girmesini engeller.
- Gizli değerler (API adresi vb.) yalnızca `user-secrets` veya ortam değişkenleriyle verilir.

## Lisans ve kullanım

Bu depo bir lisans içermez; **tüm hakları saklıdır**. Kod yalnızca portfolyo amacıyla incelenebilir;
izin alınmadan kopyalanamaz, değiştirilemez, dağıtılamaz veya başka bir projede kullanılamaz.

Proje staj kapsamında ekip olarak geliştirildiği için kod üzerindeki haklar ekip üyelerine ve staj yapılan kuruma da ait olabilir.
