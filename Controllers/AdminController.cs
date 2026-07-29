using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

using System.Threading.Tasks;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi")]
    public class AdminController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;

        public AdminController(IKullaniciDeposu kullaniciDeposu)
        {
            _kullaniciDeposu = kullaniciDeposu;
        }

        public async Task<IActionResult> Index(KcetasWeb.ViewModels.AdminListeViewModel filtre)
        {
            var liste = (await _kullaniciDeposu.ListeleAsync()).AsQueryable();

            if (!string.IsNullOrEmpty(filtre.FiltreKullaniciAdi))
                liste = liste.Where(x => x.kullanici_adi != null && x.kullanici_adi.Contains(filtre.FiltreKullaniciAdi, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filtre.FiltreAdSoyad))
                liste = liste.Where(x => x.ad_soyad != null && x.ad_soyad.Contains(filtre.FiltreAdSoyad, StringComparison.OrdinalIgnoreCase));

            if (filtre.FiltreRol.HasValue)
                liste = liste.Where(x => x.rol_id == filtre.FiltreRol.Value);

            var orderedList = liste.OrderByDescending(k => k.created_at).ToList();

            int totalItems = orderedList.Count;
            
            filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
            filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;
            
            var pagedData = orderedList.Skip((filtre.CurrentPage - 1) * filtre.PageSize).Take(filtre.PageSize).ToList();

            filtre.TotalItems = totalItems;
            filtre.Kullanicilar = pagedData;

            return View(filtre);
        }

        public async Task<IActionResult> Yeni()
        {
            ViewBag.Roller = RolListesi.Roller;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Yeni(string AdSoyad, string KullaniciAdi, string EPosta, string Sifre, short RolId)
        {
            if (await _kullaniciDeposu.KullaniciAdiVarMiAsync(KullaniciAdi))
            {
                TempData["PersonelMesaji"] = "Bu kullanıcı adı zaten kullanılıyor.";
                return RedirectToAction("Yeni");
            }

            if (await _kullaniciDeposu.EPostaVarMiAsync(EPosta))
            {
                TempData["PersonelMesaji"] = "Bu e-posta adresi zaten kullanılıyor.";
                return RedirectToAction("Yeni");
            }

            var yeniKullanici = new Kullanici
            {
                ad_soyad = AdSoyad,
                kullanici_adi = KullaniciAdi,
                e_posta = EPosta,
                rol_id = RolId,
                durum = KcetasWeb.Models.Enums.KullaniciDurumu.Aktif,
                created_at = DateTime.Now,
                Sifre = Sifre
            };

            yeniKullanici.Sifre = Sifre;

            try
            {
                await _kullaniciDeposu.EkleAsync(yeniKullanici);
                TempData["PersonelMesaji"] = AdSoyad + " isimli kullanıcı sisteme başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["PersonelMesaji"] = "Kullanıcı eklenirken bir hata oluştu. Bilgileri kontrol edip tekrar deneyiniz.";
                return RedirectToAction("Yeni");
            }
        }

        public async Task<IActionResult> Detay(long id)
        {
            var kullanici = await _kullaniciDeposu.BulIdAsync(id);
            if (kullanici == null) return NotFound();
            return View(kullanici);
        }

        public async Task<IActionResult> Duzenle(long id)
        {
            var kullanici = await _kullaniciDeposu.BulIdAsync(id);
            if (kullanici == null) return NotFound();
            ViewBag.Roller = RolListesi.Roller;
            return View(kullanici);
        }

        [HttpPost]
        public async Task<IActionResult> Duzenle(Kullanici model)
        {
            var mevcut = await _kullaniciDeposu.BulIdAsync(model.kullanici_id);
            if (mevcut == null) return NotFound();

            mevcut.ad_soyad = model.ad_soyad;
            mevcut.kullanici_adi = model.kullanici_adi;
            mevcut.e_posta = model.e_posta;
            mevcut.durum = model.durum;
            mevcut.rol_id = model.rol_id;

            await _kullaniciDeposu.GuncelleAsync(mevcut);

            TempData["PersonelMesaji"] = mevcut.ad_soyad + " isimli kullanıcı başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.kullanici_id });
        }

        public async Task<IActionResult> Sil(long id)
        {
            var kullanici = await _kullaniciDeposu.BulIdAsync(id);
            if (kullanici != null)
            {
                await _kullaniciDeposu.SilAsync(id);
                TempData["PersonelMesaji"] = kullanici.ad_soyad + " isimli kullanıcı başarıyla silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}