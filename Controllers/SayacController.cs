using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using System.Threading.Tasks;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi, SayacOkumaPersoneli,SahaOperasyonAmir,FaturalamaUzmani,Denetci")]
    public class SayacController : Controller
    {
        private readonly ISayacService _sayacService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly IIsEmriService _isEmriService;
        private readonly IEndeksOkumaService _endeksOkumaService;
        

      public SayacController(
    ISayacService sayacService,
    ITuketimNoktasiService tuketimNoktasiService,
    IIsEmriService isEmriService,
    IEndeksOkumaService endeksOkumaService)
{
    _sayacService = sayacService;
    _tuketimNoktasiService = tuketimNoktasiService;
    _isEmriService = isEmriService;
    _endeksOkumaService = endeksOkumaService;

}
        public async Task<IActionResult> Index(KcetasWeb.ViewModels.SayacListeViewModel filtre)
        {
            filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
            filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;

            int? durumParam = null;
            if (!string.IsNullOrEmpty(filtre.FiltreDurum) && Enum.TryParse<KcetasWeb.Models.Enums.SayacDurumu>(filtre.FiltreDurum, out var seciliDurum))
            {
                durumParam = (int)seciliDurum;
            }

            var response = await _sayacService.GetPagedAsync(
                filtre.CurrentPage, 
                filtre.PageSize, 
                filtre.FiltreSeriNo, 
                durumParam,
                null, // tuketimNoktasiId
                filtre.FiltreTuketimNoktasi,
                filtre.FiltreFaz);

            var pagedData = response.Data;

            // Not: Marka ve Mühür No filtrelemesi API tarafında desteklenmediği için mecburen o sayfada dönen 50 kayıt üzerinde yapılıyor.
            if (!string.IsNullOrEmpty(filtre.FiltreMarka))
            {
                pagedData = pagedData.Where(x => x.marka != null && x.marka.Equals(filtre.FiltreMarka, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(filtre.FiltreMuhurNo))
            {
                pagedData = pagedData.Where(x => x.muhur_no != null && x.muhur_no.Contains(filtre.FiltreMuhurNo, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            filtre.TotalItems = response.TotalCount;
            filtre.Sayaclar = pagedData;

            var tnIds = pagedData.Where(s => s.tuketim_noktasi_id != null).Select(s => s.tuketim_noktasi_id.Value).Distinct();
            var tnTasks = tnIds.Select(id => _tuketimNoktasiService.GetByIdAsync(id));
            var tnList = (await Task.WhenAll(tnTasks)).Where(t => t != null).ToList();
            ViewBag.TuketimNoktalari = tnList;
            
            return View(filtre);
        }

        public async Task<IActionResult> Bagla(long id)
        {
            var sayac = await _sayacService.GetByIdAsync(id);
            if (sayac == null)
                return NotFound();

            ViewBag.TuketimNoktalari = await _tuketimNoktasiService.GetAllAsync();

            return View(sayac);
        }

        [HttpPost]
        public async Task<IActionResult> Bagla(long sayac_id, int tuketim_noktasi_id, string muhur_no, decimal ilk_endeks)
        {
            if (!string.IsNullOrEmpty(muhur_no) && !muhur_no.StartsWith("MHR-"))
            {
                muhur_no = "MHR-" + muhur_no;
            }

            var sayaclar = await _sayacService.GetAllAsync();
            if (!string.IsNullOrEmpty(muhur_no) && sayaclar.Any(s => s.muhur_no == muhur_no && s.sayac_id != sayac_id))
            {
                TempData["HataMesaji"] = "HATA: Bu mühür numarası sistemde başka bir sayaçta kayıtlı! Lütfen farklı bir mühür numarası girin.";
                return RedirectToAction("Bagla", new { id = sayac_id });
            }

            var sayac = sayaclar.FirstOrDefault(x => x.sayac_id == sayac_id);

            if (sayac != null)
            {
                sayac.tuketim_noktasi_id = tuketim_noktasi_id;
                sayac.durum = tuketim_noktasi_id > 0 ? KcetasWeb.Models.Enums.SayacDurumu.Bagli : KcetasWeb.Models.Enums.SayacDurumu.Depoda;
                sayac.status = sayac.durum.ToString();
                sayac.muhur_no = muhur_no;
                sayac.updated_at = DateTime.Now;

                await _sayacService.UpdateAsync(sayac);

                TempData["BasariMesaji"] =
                    $"Sayaç başarıyla {(tuketim_noktasi_id > 0 ? "bağlandı" : "boşa alındı")}. " +
                    $"Mühür No: {muhur_no}, İlk Endeks: {ilk_endeks}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Yeni()
        {
            return View(new Sayac
            {
                carpan = 1.0m,
                faz = KcetasWeb.Models.Enums.SayacFaz.Monofaze,
                uretim_yili = DateTime.Now.Year
            });
        }

        [HttpPost]
        public async Task<IActionResult> Yeni(Sayac sayac)
        {
            NormalizeYeniSayacModel(sayac);

            var sayaclar = await _sayacService.GetAllAsync();

            if (string.IsNullOrWhiteSpace(sayac.marka))
            {
                ModelState.AddModelError(nameof(Sayac.marka), "Marka alanı zorunludur.");
            }

            if (string.IsNullOrWhiteSpace(sayac.model))
            {
                ModelState.AddModelError(nameof(Sayac.model), "Model alanı zorunludur.");
            }

            if (string.IsNullOrWhiteSpace(sayac.muhur_no))
            {
                ModelState.AddModelError(nameof(Sayac.muhur_no), "Mühür no alanı zorunludur.");
            }

            if (sayac.uretim_yili < 2000 || sayac.uretim_yili > DateTime.Now.Year)
            {
                ModelState.AddModelError(nameof(Sayac.uretim_yili), $"Üretim yılı 2000 ile {DateTime.Now.Year} arasında olmalıdır.");
            }

            if (!string.IsNullOrEmpty(sayac.seri_no) && sayaclar.Any(s => s.seri_no == sayac.seri_no))
            {
                ModelState.AddModelError("seri_no", "HATA: Bu Sayaç Seri Numarası sistemde zaten mevcut! Lütfen farklı bir seri numarası girin.");
            }

            if (!string.IsNullOrEmpty(sayac.muhur_no) && sayaclar.Any(s => s.muhur_no == sayac.muhur_no))
            {
                ModelState.AddModelError("muhur_no", "HATA: Bu mühür numarası sistemde başka bir sayaçta kayıtlı! Lütfen farklı bir mühür numarası girin.");
            }

            if (ModelState.IsValid)
            {
                sayac.durum = KcetasWeb.Models.Enums.SayacDurumu.Depoda;
                sayac.status = "DEPODA";
                sayac.created_at = DateTime.Now;
                sayac.updated_at = DateTime.Now;

                try
                {
                    await _sayacService.CreateAsync(sayac);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Sayaç oluşturulamadı. API detayı: {ex.Message}");
                    return View(sayac);
                }

                TempData["BasariMesaji"] = "Yeni sayaç başarıyla sisteme eklendi.";

                return RedirectToAction("Index");
            }

            return View(sayac);
        }

        private void NormalizeYeniSayacModel(Sayac sayac)
        {
            sayac.marka = sayac.marka?.Trim();
            sayac.model = sayac.model?.Trim();
            sayac.muhur_no = sayac.muhur_no?.Trim();

            if (string.IsNullOrWhiteSpace(sayac.seri_no))
            {
                sayac.seri_no = "SYC-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            if (!string.IsNullOrEmpty(sayac.muhur_no) && !sayac.muhur_no.StartsWith("MHR-"))
            {
                sayac.muhur_no = "MHR-" + sayac.muhur_no;
            }

            if (sayac.uretim_yili <= 0)
            {
                sayac.uretim_yili = DateTime.Now.Year;
            }

            if (sayac.carpan <= 0)
            {
                sayac.carpan = 1.0m;
            }

            sayac.faz ??= KcetasWeb.Models.Enums.SayacFaz.Monofaze;

            ModelState.Remove(nameof(Sayac.sayac_id));
            ModelState.Remove(nameof(Sayac.seri_no));
            ModelState.Remove(nameof(Sayac.status));
            ModelState.Remove(nameof(Sayac.created_at));
            ModelState.Remove(nameof(Sayac.updated_at));
            ModelState.Remove(nameof(Sayac.created_by));
            ModelState.Remove(nameof(Sayac.updated_by));
            ModelState.Remove(nameof(Sayac.tuketim_noktasi_id));
            ModelState.Remove(nameof(Sayac.durum));
        }

        public async Task<IActionResult> Detay(long id)
        {
            var sayac = await _sayacService.GetByIdAsync(id);

            if (sayac == null)
                return NotFound();

            var tnList = new List<KcetasWeb.Models.TuketimNoktasi>();
            if (sayac.tuketim_noktasi_id != null)
            {
                var tn = await _tuketimNoktasiService.GetByIdAsync((int)sayac.tuketim_noktasi_id.Value);
                if (tn != null) tnList.Add(tn);
            }
            ViewBag.TuketimNoktalari = tnList;

            ViewBag.IsEmirleri = (await _isEmriService.GetAllAsync())
                .Where(x => x.sayac_id == sayac.sayac_id)
                .ToList();

            var endeksPaged = await _endeksOkumaService.GetPagedAsync(1, 100, null, null, null, null, null, sayac.sayac_id.ToString(), null, null, null, null, null);
            ViewBag.Endeksler = endeksPaged.Data.OrderByDescending(x => x.okuma_zamani).ToList();
            return View(sayac);
        }
    }
}
