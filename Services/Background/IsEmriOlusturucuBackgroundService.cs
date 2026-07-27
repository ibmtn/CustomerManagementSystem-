using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KcetasWeb.Models.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;

namespace KcetasWeb.Services.Background
{
    public class IsEmriOlusturucuBackgroundService : BackgroundService
    {
        private readonly ILogger<IsEmriOlusturucuBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        
        // Demo için her 1 dakikada bir çalıştır. (Canlı ortamda 24 saat olacak şekilde ayarlanabilir)
        private readonly TimeSpan _period = TimeSpan.FromMinutes(1);

        public IsEmriOlusturucuBackgroundService(
            ILogger<IsEmriOlusturucuBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Otomatik İş Emri Oluşturucu (BackgroundService) başlatıldı.");
            
            using var timer = new PeriodicTimer(_period);
            
            try
            {
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        await ProcessIsEmirleriAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Otomatik iş emri oluşturulurken bir hata meydana geldi.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("BackgroundService durduruluyor.");
            }
        }

        private async Task ProcessIsEmirleriAsync()
        {
            _logger.LogInformation("25 günlük periyodik endeks okuma iş emirleri kontrolü başlatıldı...");
            
            using var scope = _serviceProvider.CreateScope();
            
            var isEmriService = scope.ServiceProvider.GetRequiredService<IIsEmriService>();
            var sayacService = scope.ServiceProvider.GetRequiredService<ISayacService>();
            var endeksOkumaService = scope.ServiceProvider.GetRequiredService<IEndeksOkumaService>();
            var kullaniciDeposu = scope.ServiceProvider.GetRequiredService<IKullaniciDeposu>();

            // Tüm sayaç ve okumaları getir
            var tumSayaclar = await sayacService.GetAllAsync() ?? new List<Sayac>();
            var tumOkumalar = await endeksOkumaService.GetAllAsync() ?? new List<EndeksOkuma>();
            
            // Bekleyen (Tamamlanmamış) endeks okuma iş emirlerini getir
            var acikIsEmirleri = (await isEmriService.FiltreleAsync("ENDEKS_OKUMA", null, null, null, null))
                ?.Where(x => x.durum != KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi && x.durum != KcetasWeb.Models.Enums.IsEmriDurumu.Iptal)
                .ToList() ?? new List<IsEmri>();

            // Saha personellerini getir (SayacOkumaPersoneli)
            var personeller = (await kullaniciDeposu.ListeleAsync())
                ?.Where(k => k.durum == KcetasWeb.Models.Enums.KullaniciDurumu.Aktif && k.Rol?.rol_adi == AppRoles.SayacOkumaPersoneli)
                .ToList();

            if (personeller == null || !personeller.Any())
            {
                _logger.LogWarning("Sistemde aktif 'SayacOkumaPersoneli' bulunamadı! İş emirleri personelsiz (havuza) açılacak.");
            }

            int atanacakPersonelIndex = 0;
            int olusturulanIsEmriSayisi = 0;
            var bugun = DateTime.UtcNow;

            foreach (var sayac in tumSayaclar)
            {
                // Sayacın bir tüketim noktasına bağlı olması zorunlu
                if (sayac.tuketim_noktasi_id == null || sayac.tuketim_noktasi_id <= 0)
                    continue;

                // Bu sayaca ait en son okumayı bul
                var sonOkuma = tumOkumalar
                    .Where(o => o.sayac_id == sayac.sayac_id)
                    .OrderByDescending(o => o.okuma_zamani ?? o.created_at)
                    .FirstOrDefault();

                bool isEmriGerekiyor = false;

                if (sonOkuma != null)
                {
                    // Üzerinden 25 gün geçmiş mi kontrol et
                    var sonTarih = sonOkuma.okuma_zamani ?? sonOkuma.created_at;
                    if ((bugun - sonTarih).TotalDays >= 25)
                    {
                        isEmriGerekiyor = true;
                    }
                }
                
                if (isEmriGerekiyor)
                {
                    // Zaten açık bir endeks okuma iş emri var mı kontrol et
                    bool acikEmirVarMi = acikIsEmirleri.Any(ie => ie.sayac_id == sayac.sayac_id);
                    if (acikEmirVarMi)
                        continue; // Zaten bekleyen bir iş emri var, tekrar açmaya gerek yok

                    // Sıradaki personeli seç (Round-Robin Yöntemi - Otomatik Sırayla Atama)
                    long? atananPersonelId = null;
                    if (personeller != null && personeller.Any())
                    {
                        atananPersonelId = personeller[atanacakPersonelIndex % personeller.Count].kullanici_id;
                        atanacakPersonelIndex++;
                    }

                    var isEmriNo = "IE-OKM-" + bugun.ToString("yyyyMMdd") + "-" + sayac.sayac_id;

                    var yeniIsEmri = new IsEmri
                    {
                        is_emri_no = isEmriNo,
                        tuketim_noktasi_id = sayac.tuketim_noktasi_id.Value,
                        sayac_id = sayac.sayac_id,
                        tip = IsEmriTipi.EndeksOkuma,
                        durum = atananPersonelId.HasValue ? IsEmriDurumu.Atandi : IsEmriDurumu.Acik,
                        oncelik = "NORMAL",
                        planlanan_tarih = bugun.AddDays(5), // Ayın 10'unda okunduysa, 10-15 arası gibi atamalar için 5 günlük pencere
                        atanan_kullanici_id = atananPersonelId,
                        status = "AKTIF",
                        created_at = bugun
                    };

                    try
                    {
                        await isEmriService.EkleAsync(yeniIsEmri);
                        olusturulanIsEmriSayisi++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Sayaç ID {sayac.sayac_id} için otomatik iş emri oluşturulurken hata alındı.");
                    }
                }
            }

            if (olusturulanIsEmriSayisi > 0)
            {
                _logger.LogInformation($"Periyodik kontrol tamamlandı. {olusturulanIsEmriSayisi} adet otomatik iş emri oluşturuldu ve sırayla personellere atandı.");
            }
            else
            {
                _logger.LogInformation("Periyodik kontrol tamamlandı. Şu an için 25 günü aşan yeni okuma tespit edilmedi.");
            }
        }
    }
}
