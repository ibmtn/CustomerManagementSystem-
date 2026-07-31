using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Models;
using System.Linq;

using System.Threading.Tasks;

namespace KcetasWeb.Controllers
{
    // Yalnızca BT Yöneticisi ve Denetçi rollerinin görmesini sağlıyoruz.
    [Authorize(Roles = "BTYoneticisi,Denetci")]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;
        private readonly IKullaniciDeposu _kullaniciDeposu;

        public AuditLogController(IAuditLogService auditLogService, IKullaniciDeposu kullaniciDeposu)
        {
            _auditLogService = auditLogService;
            _kullaniciDeposu = kullaniciDeposu;
        }

        public async Task<IActionResult> Index(int page = 1, string? q = null, string? kullanici = null, int? kayitId = null, string? islemTipi = null, DateTime? tarih = null)
        {
            int pageSize = 50; 
            
            // Tüm parametreleri API'ye iletiyoruz. Filtreleme artık API (Veritabanı) tarafında yapılacak.
            string tarihStr = tarih?.ToString("yyyy-MM-dd");
            var result = await _auditLogService.GetAllAsync(page, pageSize, q, kullanici, kayitId, islemTipi, tarihStr);
            var loglar = result.Data ?? new List<AuditLog>();

            var kullanicilar = await _kullaniciDeposu.ListeleAsync();
            var kullaniciDict = kullanicilar.GroupBy(k => k.kullanici_id).ToDictionary(g => g.Key, g => g.First().ad_soyad ?? g.First().kullanici_adi);

            ViewBag.CurrentPage = page;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.TotalPages = result.TotalPages;

            ViewBag.SearchQ = q;
            ViewBag.SearchKullanici = kullanici;
            ViewBag.SearchKayitId = kayitId;
            ViewBag.SearchIslemTipi = islemTipi;
            ViewBag.SearchTarih = tarihStr;
            ViewBag.KullaniciDict = kullaniciDict;

            return View(loglar);
        }
    }
}
