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
            var sayaclar = (await _sayacService.GetAllAsync()).AsQueryable();

            if (!string.IsNullOrEmpty(filtre.FiltreSeriNo))
                sayaclar = sayaclar.Where(x => x.seri_no != null && x.seri_no.Contains(filtre.FiltreSeriNo, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filtre.FiltreMarka))
                sayaclar = sayaclar.Where(x => x.marka != null && x.marka.Equals(filtre.FiltreMarka, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filtre.FiltreDurum) && Enum.TryParse<KcetasWeb.Models.Enums.SayacDurumu>(filtre.FiltreDurum, out var seciliDurum))
                sayaclar = sayaclar.Where(x => x.durum == seciliDurum);

            var sayacList = sayaclar.OrderByDescending(x => x.sayac_id).ToList();
            int totalItems = sayacList.Count;

            filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
            filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;

            var pagedData = sayacList.Skip((filtre.CurrentPage - 1) * filtre.PageSize).Take(filtre.PageSize).ToList();
            
            filtre.TotalItems = totalItems;
            filtre.Sayaclar = pagedData;

            ViewBag.TuketimNoktalari = await _tuketimNoktasiService.GetAllAsync();
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
                faz = KcetasWeb.Models.Enums.SayacFaz.Monofaze
            });
        }

        [HttpPost]
        public async Task<IActionResult> Yeni(Sayac model)
        {
            if (!string.IsNullOrEmpty(model.muhur_no) && !model.muhur_no.StartsWith("MHR-"))
            {
                model.muhur_no = "MHR-" + model.muhur_no;
            }

            var sayaclar = await _sayacService.GetAllAsync();

            if (!string.IsNullOrEmpty(model.seri_no) && sayaclar.Any(s => s.seri_no == model.seri_no))
            {
                ModelState.AddModelError("seri_no", "HATA: Bu Sayaç Seri Numarası sistemde zaten mevcut! Lütfen farklı bir seri numarası girin.");
            }

            if (ModelState.IsValid)
            {

                model.sayac_id = sayaclar.Any()
                    ? sayaclar.Max(x => x.sayac_id) + 1
                    : 1;

                model.durum = KcetasWeb.Models.Enums.SayacDurumu.Depoda;
                model.status = "Depoda";
                model.created_at = DateTime.Now;

                await _sayacService.CreateAsync(model);
                var endeks = new EndeksOkuma
{
    sayac_id = (int)model.sayac_id,
    yeni_endeks = 0,
    onceki_endeks = 0,
    okuma_tipi = KcetasWeb.Models.Enums.OkumaTipi.IlkOkuma,
    okuma_kaynagi = KcetasWeb.Models.Enums.OkumaKaynagi.Manuel,
    okuma_zamani = DateTime.Now,
    kullanici_id = 1,
    status = "AKTIF"
};

await _endeksOkumaService.CreateAsync(endeks);

                TempData["BasariMesaji"] = "Yeni sayaç başarıyla sisteme eklendi.";

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Detay(long id)
        {
            var sayac = await _sayacService.GetByIdAsync(id);

            if (sayac == null)
                return NotFound();

            ViewBag.TuketimNoktalari = await _tuketimNoktasiService.GetAllAsync();

            ViewBag.IsEmirleri = (await _isEmriService
                .GetAllAsync())
                .Where(x => x.sayac_id == sayac.sayac_id)
                .ToList();
ViewBag.Endeksler = (await _endeksOkumaService
    .GetAllAsync())
    .Where(x => x.sayac_id == sayac.sayac_id)
    .OrderByDescending(x => x.okuma_zamani)
    .ToList();
            return View(sayac);
        }
    }
}