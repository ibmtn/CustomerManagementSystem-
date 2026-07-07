using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,SayacYetkilisi")]
    public class SayacController : Controller
    {
        private static List<Sayac> _sayaclar = new List<Sayac>
        {
            new Sayac { sayac_id = 1, seri_no = "S-1001", marka = "Makel", model = "M500", faz = "Monofaze", carpan = 1.0m, muhur_no = 0, durum = "Depoda", status = "Depoda", CreatedAt = DateTime.Now },
            new Sayac { sayac_id = 2, seri_no = "S-1002", marka = "Luna", model = "L300", faz = "Trifaze", carpan = 1.5m, muhur_no = 0, durum = "Depoda", status = "Depoda", CreatedAt = DateTime.Now },
            new Sayac { sayac_id = 3, seri_no = "S-1003", marka = "Makel", model = "M500", faz = "Monofaze", carpan = 1.0m, muhur_no = 556677, durum = "Bağlı", status = "Bağlı", tuketim_noktasi_id = 1, CreatedAt = DateTime.Now }
        };

        public IActionResult Index()
        {
            ViewBag.TuketimNoktalari = TuketimNoktasiController._tuketimNoktalari;
            return View(_sayaclar);
        }

        public IActionResult Bagla(long id)
        {
            var sayac = _sayaclar.FirstOrDefault(s => s.sayac_id == id);
            if (sayac == null) return NotFound();

            ViewBag.TuketimNoktalari = TuketimNoktasiController._tuketimNoktalari;
            return View(sayac);
        }

        [HttpPost]
        public IActionResult Bagla(long sayac_id, int tuketim_noktasi_id, long muhur_no, decimal ilk_endeks)
        {
            var sayac = _sayaclar.FirstOrDefault(s => s.sayac_id == sayac_id);
            if (sayac != null)
            {
                sayac.tuketim_noktasi_id = tuketim_noktasi_id;
                sayac.durum = tuketim_noktasi_id > 0 ? "Bağlı" : "Depoda";
                sayac.status = sayac.durum;
                sayac.muhur_no = muhur_no;
                
                // Normalde ilk_endeks IsEmri veya EndeksOkuma tablosuna yazilir, 
                // ancak MVP senaryosunda bu bilgi aliniyor ve loglaniyor farzediyoruz.
                sayac.UpdatedAt = DateTime.Now;

                TempData["BasariMesaji"] = $"Sayaç başarıyla {(tuketim_noktasi_id > 0 ? "bağlandı" : "boşa alındı")}. Mühür No: {muhur_no}, İlk Endeks: {ilk_endeks}";
            }
            return RedirectToAction("Index");
        }
    }
}
