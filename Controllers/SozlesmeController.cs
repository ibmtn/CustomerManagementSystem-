using Microsoft.AspNetCore.Mvc;

namespace KcetasWeb.Controllers
{
    public class SozlesmeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Yeni()
        {
            return View();
        }

        // FORMDAN GELEN VERİYİ YAKALAYAN METOT
        [HttpPost]
        public IActionResult Yeni(string TuketimNoktasiKod, string TarifeGrubu, string AboneAdSoyad)
        {
            TempData["SozlesmeMesaji"] = AboneAdSoyad + " isimli abone için " + TuketimNoktasiKod + " numaralı noktada sözleşme başarıyla başlatıldı.";
            return RedirectToAction("Index");
        }
    }
}