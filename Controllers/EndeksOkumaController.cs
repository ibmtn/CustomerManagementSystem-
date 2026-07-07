using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace KcetasWeb.Controllers
{
    public class EndeksOkumaController : Controller
    {
        private readonly Services.Interfaces.IEndeksOkumaService _endeksOkumaService;
        public EndeksOkumaController(Services.Interfaces.IEndeksOkumaService endeksOkumaService)
        {
            _endeksOkumaService = endeksOkumaService;
        }

        public IActionResult Index(string? kaynak, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var okumalar = _endeksOkumaService.Filtrele(kaynak, durum, baslangic, bitis, arama);
            ViewBag.Istatistikler = _endeksOkumaService.GetIstatistikler();
            return View(okumalar);
        }

        public IActionResult Detay(long id)
        {
            var okuma = _endeksOkumaService.GetById((int)id);
            if (okuma == null) return NotFound();
            return View(okuma);
        }

        public IActionResult Yeni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(long TuketimNoktasiId, long SayacId, decimal onceki_endeks, decimal yeni_endeks)
        {
            // Gerçek projede veritabanına ekleme yapılır, mock'ta listeye eklenir. 
            // Burada basit tutuyoruz.
            TempData["OkumaMesaji"] = "Tüketim noktası " + TuketimNoktasiId + " için endeks okuma işlemi başarıyla kaydedildi.";
            return RedirectToAction("Index");
        }
    }
}
