using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace KcetasWeb.Controllers
{
    [Authorize]
    public class FaturaController : Controller
    {
        private static List<Fatura> _faturalar = new List<Fatura>
        {
            new Fatura {
                fatura_id = 1, fatura_no = "FAT-2026-001", sozlesme_id = 1001, okuma_id = 5001, tekil_kod = "1001", fatura_tipi = "Mesken",
                fatura_tarihi = new DateTime(2026, 6, 30), son_odeme_tarihi = new DateTime(2026, 7, 15), donem = "2026-06",
                ilk_endeks = 15200, son_endeks = 15520, tuketim_kwh = 320, carpan = 1, enerji_bedeli = 912, dagatim_bedeli = 208,
                vergi_fon_toplama = 224, toplam_tutar = 1344.00m, durum = "Ödendi", status = "Active", created_at = new DateTime(2026, 6, 30)
            },
            new Fatura {
                fatura_id = 2, fatura_no = "FAT-2026-002", sozlesme_id = 1002, okuma_id = 5002, tekil_kod = "1002", fatura_tipi = "Ticarethane",
                fatura_tarihi = new DateTime(2026, 6, 30), son_odeme_tarihi = new DateTime(2026, 7, 15), donem = "2026-06",
                ilk_endeks = 18400, son_endeks = 18750, tuketim_kwh = 350, carpan = 1, enerji_bedeli = 1207.50m, dagatim_bedeli = 227.50m,
                vergi_fon_toplama = 287, toplam_tutar = 1722.00m, durum = "Bekliyor", status = "Active", created_at = new DateTime(2026, 6, 30)
            }
        };

        public IActionResult Index()
        {
            return View(_faturalar);
        }

        public IActionResult Olustur()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Olustur(Fatura fatura)
        {
            fatura.fatura_id = _faturalar.Count > 0 ? _faturalar.Max(f => f.fatura_id) + 1 : 1;
            fatura.fatura_no = $"FAT-{DateTime.Now.Year}-{(fatura.fatura_id).ToString().PadLeft(3, '0')}";
            fatura.fatura_tarihi = DateTime.Now;
            fatura.son_odeme_tarihi = DateTime.Now.AddDays(15);
            fatura.donem = DateTime.Now.ToString("yyyy-MM");
            fatura.durum = "Bekliyor";
            fatura.status = "Active";
            fatura.created_at = DateTime.Now;
            
            _faturalar.Add(fatura);
            
            TempData["BasariMesaji"] = fatura.fatura_no + " numaralı fatura başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(int id)
        {
            var fatura = _faturalar.FirstOrDefault(x => x.fatura_id == id);
            if (fatura == null)
            {
                return NotFound();
            }
            return View(fatura);
        }

        public IActionResult OdemeYap(int id)
        {
            var fatura = _faturalar.FirstOrDefault(x => x.fatura_id == id);
            if (fatura != null)
            {
                fatura.durum = "Ödendi";
                fatura.updated_at = DateTime.Now;
                TempData["FaturaMesaji"] = fatura.fatura_no + " numaralı fatura başarıyla ödendi.";
            }
            return RedirectToAction("Index");
        }
    }
}
