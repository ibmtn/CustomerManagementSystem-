using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,SahaOperasyonAmiri,Denetci,FaturalamaUzmani,Yonetici")]
    public class TuketimNoktasiController : Controller
    {
        public static List<TuketimNoktasi> _tuketimNoktalari = new List<TuketimNoktasi>
        {
            new TuketimNoktasi {
                TuketimNoktasiId = 1, tekil_kod = "TK-2026-001",
                musteri_ad = "Ahmet", musteri_soyad = "Yılmaz", tckn = "12345678901", telefon = "05321234567",
                ilce_id = 1, mahalle = "Merkez", bina_no = "1", bagimsiz_bolum_no = "12", acik_adres = "Merkez Mah. 1. Sokak No:1 D:12",
                tuketici_grubu = "Mesken", baglanti_grubu = "AG", status = "Aktif",
                BaglantiGucuKw = 15.5m, Enlem = "38.7205", Boylam = "35.4826",
                CreatedAt = DateTime.Now.AddDays(-10)
            },
            new TuketimNoktasi {
                TuketimNoktasiId = 2, tekil_kod = "TK-2026-002",
                musteri_unvan = "Örnek Ltd. Şti.", vkn = "1234567890", telefon = "02121234567",
                ilce_id = 2, mahalle = "Sanayi", bina_no = "2", bagimsiz_bolum_no = "4", acik_adres = "Sanayi Mah. 2. Cadde No:2 D:4",
                tuketici_grubu = "Ticarethane", baglanti_grubu = "OG", status = "Pasif",
                BaglantiGucuKw = 50.0m, Enlem = "38.7300", Boylam = "35.4900",
                CreatedAt = DateTime.Now.AddDays(-5)
            }
        };

        public IActionResult Index()
        {
            return View(_tuketimNoktalari);
        }

        public IActionResult Yeni()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(TuketimNoktasi model)
        {
            int maxId = _tuketimNoktalari.Any() ? (int)_tuketimNoktalari.Max(x => x.TuketimNoktasiId) : 0;
            model.TuketimNoktasiId = maxId + 1;
            model.tekil_kod = $"TK-2026-{(maxId + 1).ToString().PadLeft(3, '0')}";
            model.status = "Pasif";
            model.CreatedAt = DateTime.Now;

            _tuketimNoktalari.Add(model);

            TempData["BasariMesaji"] = "Harika! Yeni tüketim noktası ve abone başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        public IActionResult Detay(string id)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.tekil_kod == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult Duzenle(string id)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.tekil_kod == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Duzenle(TuketimNoktasi model)
        {
            var item = _tuketimNoktalari.FirstOrDefault(x => x.tekil_kod == model.tekil_kod);
            if (item != null)
            {
                item.musteri_ad = model.musteri_ad;
                item.musteri_soyad = model.musteri_soyad;
                item.musteri_unvan = model.musteri_unvan;
                item.tckn = model.tckn;
                item.vkn = model.vkn;
                item.telefon = model.telefon;
                item.e_posta = model.e_posta;
                item.iletisim_tercihi = model.iletisim_tercihi;
                item.ilce_id = model.ilce_id;
                item.mahalle = model.mahalle;
                item.bina_no = model.bina_no;
                item.bagimsiz_bolum_no = model.bagimsiz_bolum_no;
                item.acik_adres = model.acik_adres;
                item.BaglantiGucuKw = model.BaglantiGucuKw;
                item.Enlem = model.Enlem;
                item.Boylam = model.Boylam;
                item.tuketici_grubu = model.tuketici_grubu;
                item.baglanti_grubu = model.baglanti_grubu;
                item.status = model.status;
                item.UpdatedAt = DateTime.Now;
            }
            TempData["BasariMesaji"] = model.tekil_kod + " kodlu nokta başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.tekil_kod });
        }
    }
}