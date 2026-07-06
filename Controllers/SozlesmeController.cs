using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Controllers
{
    public class SozlesmeController : Controller
    {
        private static List<Sozlesme> _sozlesmeler = new List<Sozlesme>
        {
            new Sozlesme { SozlesmeId = 1, SozlesmeNo = "SZL-10045", AboneId = 1, TuketimNoktasiId = 1001, Durum = "Aktif", BaslangicTarihi = DateTime.Now.AddMonths(-12) },
            new Sozlesme { SozlesmeId = 2, SozlesmeNo = "SZL-10046", AboneId = 2, TuketimNoktasiId = 1002, Durum = "Güvence Bekliyor", BaslangicTarihi = DateTime.Now.AddDays(-2) },
            new Sozlesme { SozlesmeId = 3, SozlesmeNo = "SZL-10047", AboneId = 3, TuketimNoktasiId = 1003, Durum = "Feshedildi", BaslangicTarihi = DateTime.Now.AddMonths(-24), BitisTarihi = DateTime.Now.AddDays(-10) }
        };

        public IActionResult Index()
        {
            return View(_sozlesmeler);
        }

        public IActionResult Yeni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(long TuketimNoktasiId, long AboneId)
        {
            int count = _sozlesmeler.Count + 45; // Start from SZL-10045
            _sozlesmeler.Add(new Sozlesme
            {
                SozlesmeId = count,
                SozlesmeNo = $"SZL-{10000 + count}",
                AboneId = AboneId,
                TuketimNoktasiId = TuketimNoktasiId,
                Durum = "Güvence Bekliyor",
                BaslangicTarihi = DateTime.Now,
                CreatedAt = DateTime.Now
            });

            TempData["SozlesmeMesaji"] = AboneId + " ID'li abone için sözleşme başarıyla başlatıldı.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(string id)
        {
            var item = _sozlesmeler.FirstOrDefault(x => x.SozlesmeNo == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult Duzenle(string id)
        {
            var item = _sozlesmeler.FirstOrDefault(x => x.SozlesmeNo == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Duzenle(string SozlesmeNo, long AboneId, string Durum)
        {
            var item = _sozlesmeler.FirstOrDefault(x => x.SozlesmeNo == SozlesmeNo);
            if (item != null)
            {
                item.AboneId = AboneId;
                item.Durum = Durum;
            }
            TempData["SozlesmeMesaji"] = SozlesmeNo + " numaralı sözleşme başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = SozlesmeNo });
        }

        public IActionResult Feshet(string id)
        {
            var item = _sozlesmeler.FirstOrDefault(x => x.SozlesmeNo == id);
            if (item != null)
            {
                item.Durum = "Feshedildi";
                item.BitisTarihi = DateTime.Now;
                item.UpdatedAt = DateTime.Now;
                TempData["SozlesmeMesaji"] = id + " numaralı sözleşme başarıyla feshedildi.";
            }
            return RedirectToAction("Index");
        }
    }
}
