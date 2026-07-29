using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Models.entities;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi, SayacOkumaPersoneli,SahaOperasyonAmir,FaturalamaUzmani,Denetci ")]
    public class DashboardController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;
        private readonly IAboneService _aboneService;
        private readonly IFaturaService _faturaService;
        private readonly IAuditLogService _auditLogService;
        private readonly IOutboxService _outboxService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly ISozlesmeService _sozlesmeService;
        private readonly IIsEmriService _isEmriService;

        public DashboardController(
            IKullaniciDeposu kullaniciDeposu, 
            IAboneService aboneService,
            IFaturaService faturaService,
            IAuditLogService auditLogService,
            IOutboxService outboxService,
            ITuketimNoktasiService tuketimNoktasiService,
            ISozlesmeService sozlesmeService,
            IIsEmriService isEmriService)
        {
            _kullaniciDeposu = kullaniciDeposu;
            _aboneService = aboneService;
            _faturaService = faturaService;
            _auditLogService = auditLogService;
            _outboxService = outboxService;
            _tuketimNoktasiService = tuketimNoktasiService;
            _sozlesmeService = sozlesmeService;
            _isEmriService = isEmriService;
        }

        public async Task<IActionResult> Index()
        {
            var kullaniciListe = await _kullaniciDeposu.ListeleAsync();
            ViewBag.ToplamKullanici = kullaniciListe.Count;
            ViewBag.AktifKullanici = kullaniciListe.Count(k => k.durum == KcetasWeb.Models.Enums.KullaniciDurumu.Aktif);
            
            // Abone sayısı (GetPagedAsync ile hızlı çekiliyor)
            try { ViewBag.AboneCount = (await _aboneService.GetPagedAsync(1, 1)).TotalCount; } catch { ViewBag.AboneCount = 0; }
            
            // Tuketim Noktası sayısı
            ViewBag.AktifTuketimNoktasi = await _tuketimNoktasiService.GetTotalCountAsync();

            // Sözleşme sayısı
            ViewBag.SozlesmeCount = await _sozlesmeService.GetTotalCountAsync();

            // İş Emri sayısı
            ViewBag.AktifIsEmriCount = await _isEmriService.GetTotalCountAsync();

            // Fatura sayısı (Taslak, Hatalı, Ödenmedi, Hesaplandı gibi bekleyenleri farazi olarak total faturadan gösterebiliriz)
            try { ViewBag.BekleyenFaturaCount = (await _faturaService.GetPagedAsync(1, 1)).TotalCount; } catch { ViewBag.BekleyenFaturaCount = 0; }

            // AuditLog ve Outbox sayıları eklendi
            try { ViewBag.AuditLogCount = (await _auditLogService.GetAllAsync(1, 1)).TotalCount; } catch { ViewBag.AuditLogCount = 0; }
            try { ViewBag.OutboxCount = (await _outboxService.GetIstatistiklerAsync()).Toplam; } catch { ViewBag.OutboxCount = 0; }
            
            // Son Kullanıcılar
            ViewBag.SonKullanicilar = kullaniciListe
                                        .OrderByDescending(k => k.created_at)
                                        .Take(5).ToList();

            return View();
        }
    }
}
