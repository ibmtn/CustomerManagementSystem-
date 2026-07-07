using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using KcetasWeb.Models.entities;



namespace KcetasWeb.Controllers
{
    [Authorize(Roles = AppRoles.FaturalamaUzmani + "," + AppRoles.BTYoneticisi)]
    public class AboneSorgulamaController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;

        private static List<Abone> _aboneler = new List<Abone>
        {
            new Abone { AboneId = 1, AboneNo = "ABN-10045", AboneTipi = "Bireysel", Ad = "Ahmet", Soyad = "Yılmaz", TcKimlikNo = "12345678901", Telefon = "05321234567", EPosta = "ahmet@ornek.com", Durum = "Aktif", CreatedAt = DateTime.Now.AddDays(-100) },
            new Abone { AboneId = 2, AboneNo = "ABN-10046", AboneTipi = "Kurumsal", Unvan = "Örnek Ltd. Şti.", VergiNo = "1234567890", Ad = "Örnek", Soyad = "Ltd. Şti.", TcKimlikNo = "98765432101", Telefon = "02121234567", EPosta = "iletisim@ornek.com.tr", Durum = "Aktif", CreatedAt = DateTime.Now.AddDays(-200) },
            new Abone { AboneId = 3, AboneNo = "ABN-10047", AboneTipi = "Bireysel", Ad = "Ayşe", Soyad = "Demir", TcKimlikNo = "11122233344", Telefon = "05339998877", EPosta = "ayse@ornek.com", Durum = "Pasif", CreatedAt = DateTime.Now.AddDays(-50) }
        };

        public AboneSorgulamaController(IKullaniciDeposu kullaniciDeposu)
        {
            _kullaniciDeposu = kullaniciDeposu;
        }

        private List<Abone> TumAboneleriGetir()
        {
            var liste = new List<Abone>(_aboneler);

            var kayitliAboneler = _kullaniciDeposu.Listele()
                .Where(k => k.Rol?.rol_adi == "Abone");

            foreach (var kullanici in kayitliAboneler)
            {
                var adSoyad = kullanici.ad_soyad?.Split(' ') ?? new string[] { "", "" };
                var ad = adSoyad[0];
                var soyad = adSoyad.Length > 1 ? string.Join(" ", adSoyad.Skip(1)) : "";

                liste.Add(new Abone
                {
                    AboneId = 100000 + kullanici.kullanici_id,
                    AboneNo = "ABN-" + (20000 + kullanici.kullanici_id),
                    AboneTipi = "Bireysel",
                    Ad = ad,
                    Soyad = soyad,
                    TcKimlikNo = "11111111111",
                    Telefon = "05000000000",
                    EPosta = kullanici.e_posta ?? "",
                    Durum = kullanici.durum == "AKTIF" ? "Aktif" : "Pasif",
                    CreatedAt = kullanici.created_at,
                    UpdatedAt = kullanici.updated_at
                });
            }

            return liste;
        }

        public IActionResult Index(string q)
        {
            ViewBag.Query = q;
            if (string.IsNullOrEmpty(q))
            {
                return View(null);
            }

            var tumAboneler = TumAboneleriGetir();

            var results = tumAboneler.Where(a =>
                (a.AboneNo != null && a.AboneNo.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (a.Ad != null && a.Ad.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (a.Soyad != null && a.Soyad.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (a.TcKimlikNo != null && a.TcKimlikNo.Contains(q, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            return View(results);
        }

        public IActionResult Detay(long id)
        {
            var tumAboneler = TumAboneleriGetir();
            var abone = tumAboneler.FirstOrDefault(x => x.AboneId == id);
            if (abone == null)
            {
                return NotFound();
            }
            return View(abone);
        }
    }
}