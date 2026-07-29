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

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 50; 
            var result = await _auditLogService.GetAllAsync(page, pageSize);
            var loglar = result.Data;
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.TotalPages = result.TotalPages;

            // Tüm kullanıcıları bir kez çekip View'a sözlük olarak gönderiyoruz
            // Böylece View içinde kullanici_id'yi isme çevirebiliriz.
            var kullanicilar = await _kullaniciDeposu.ListeleAsync();
            ViewBag.KullaniciDict = kullanicilar.GroupBy(k => k.kullanici_id).ToDictionary(g => g.Key, g => g.First().ad_soyad ?? g.First().kullanici_adi);

            return View(loglar);
        }
    }
}
