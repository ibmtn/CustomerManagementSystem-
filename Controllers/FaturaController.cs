using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace KcetasWeb.Controllers
{
    public class FaturaController : Controller
    {
        private static List<Fatura> _faturalar = new List<Fatura>
        {
            new Fatura {
                FaturaId = 1, FaturaNo = "FAT-2026-001", SozlesmeId = 1001, EndeksOkumaId = 5001,
                FaturaTarihi = new DateTime(2026, 6, 30), SonOdemeTarihi = new DateTime(2026, 7, 15), 
                ToplamTutar = 450.75m, KdvTutari = 75.12m, OdemeDurumu = "Ödendi", OdemeTarihi = new DateTime(2026, 7, 10), CreatedAt = new DateTime(2026, 6, 30)
            },
            new Fatura {
                FaturaId = 2, FaturaNo = "FAT-2026-002", SozlesmeId = 1002, EndeksOkumaId = 5002,
                FaturaTarihi = new DateTime(2026, 6, 30), SonOdemeTarihi = new DateTime(2026, 7, 15), 
                ToplamTutar = 3250.00m, KdvTutari = 541.66m, OdemeDurumu = "Bekliyor", CreatedAt = new DateTime(2026, 6, 30)
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
                fatura.OdemeDurumu = "Ödendi";
                fatura.OdemeTarihi = DateTime.Now;
                fatura.UpdatedAt = DateTime.Now;
                TempData["FaturaMesaji"] = fatura.FaturaNo + " numaralı fatura başarıyla ödendi.";
            }
            return RedirectToAction("Index");
        }
    }
}
