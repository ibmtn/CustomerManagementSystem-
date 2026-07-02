using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;

namespace KcetasWeb.Controllers
{
    public class FaturaController : Controller
    {
        private static List<Fatura> _faturalar = new List<Fatura>
        {
            new Fatura {
                FaturaId = 1, FaturaNo = "FAT-2026-001", AboneTipi = "Mesken", 
                Donem = "2026-06", SonOdemeTarihi = new DateTime(2026, 7, 15), 
                TuketimKwh = 245.50m, ToplamTutar = 450.75m, Durum = "Ödendi"
            },
            new Fatura {
                FaturaId = 2, FaturaNo = "FAT-2026-002", AboneTipi = "Ticarethane", 
                Donem = "2026-06", SonOdemeTarihi = new DateTime(2026, 7, 15), 
                TuketimKwh = 1250.00m, ToplamTutar = 3250.00m, Durum = "Bekliyor"
            }
        };

        public IActionResult Index()
        {
            return View(_faturalar);
        }

        public IActionResult Detay(int id)
        {
            var fatura = _faturalar.FirstOrDefault(x => x.FaturaId == id);
            if (fatura == null)
            {
                return NotFound();
            }
            return View(fatura);
        }

        public IActionResult OdemeYap(int id)
        {
            var fatura = _faturalar.FirstOrDefault(x => x.FaturaId == id);
            if (fatura != null)
            {
                fatura.Durum = "Ödendi";
                TempData["FaturaMesaji"] = fatura.FaturaNo + " numaralı fatura başarıyla ödendi.";
            }
            return RedirectToAction("Index");
        }
    }
}
