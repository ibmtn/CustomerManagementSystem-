using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;



namespace KcetasWeb.Controllers;

[Authorize(Roles = AppRoles.FaturalamaUzmani + "," + AppRoles.BTYoneticisi)]  
public class IsEmriController : Controller
{
    private readonly IIsEmriService _isEmriService;

    public IsEmriController(IIsEmriService isEmriService)
    {
        _isEmriService = isEmriService;
    }

    public IActionResult Index(string? tip, string? durum,
        DateTime? baslangicTarih, DateTime? bitisTarih, string? arama)
    {
        var isEmirleri = _isEmriService.Filtrele(tip, durum, baslangicTarih, bitisTarih, arama);

        var viewModel = new IsEmriListeViewModel
        {
            FiltreTip = tip,
            FiltreDurum = durum,
            BaslangicTarih = baslangicTarih,
            BitisTarih = bitisTarih,
            AramaMetni = arama,
            IsEmirleri = isEmirleri.Select(ie => new IsEmriSatirViewModel
            {
                IsEmriId = ie.IsEmriId,
                IsEmriNo = ie.IsEmriNo,
                Tip = ie.Tip,
                TuketimNoktasiId = ie.TuketimNoktasiId,
                TuketimNoktasiKodu = "Bilgi Yok",
                PlanlananTarih = ie.PlanlananTarih,
                AtananKullaniciAdi = "Kullanıcı " + ie.AtananKullaniciId,
                Durum = ie.Durum,
                DurumRenk = IsEmriListeViewModel.GetDurumRenk(ie.Durum),
                Adres = "Adres bilgisi alınamadı"
            }).ToList()
        };

        return View(viewModel);
    }

    public IActionResult Detay(long id)
    {
        var isEmri = _isEmriService.GetById(id);
        if (isEmri == null)
            return NotFound();

        var viewModel = new IsEmriDetayViewModel
        {
            IsEmriId = isEmri.IsEmriId,
            IsEmriNo = isEmri.IsEmriNo,
            Tip = isEmri.Tip,
            Durum = isEmri.Durum,
            DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.Durum),
            Oncelik = isEmri.Oncelik,
            PlanlananTarih = isEmri.PlanlananTarih,
            AtananKullaniciAdi = "Kullanıcı " + isEmri.AtananKullaniciId,
            TuketimNoktasiKodu = "Bilgi Yok",
            Adres = "Adres bilgisi alınamadı",
            SayacSeriNo = "Sayaç bilgisi yok",
            SahaSonucu = isEmri.SahaSonucu,
            Gerekce = isEmri.Gerekce,
            MuhurNo = isEmri.MuhurNo,
            TutanakNo = isEmri.TutanakNo,
            CreatedAt = isEmri.CreatedAt,
            UpdatedAt = isEmri.UpdatedAt
        };

        return View(viewModel);
    }

    public IActionResult TutanakGiris(long id)
    {
        var isEmri = _isEmriService.GetById(id);
        if (isEmri == null)
            return NotFound();

        var viewModel = new TutanakGirisViewModel
        {
            IsEmriId = isEmri.IsEmriId,
            IsEmriNo = isEmri.IsEmriNo,
            Tip = isEmri.Tip,
            IslemTarihi = DateTime.Now
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TutanakKaydet(TutanakGirisViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("TutanakGiris", model);
        }

        _isEmriService.TutanakKaydet(
            model.IsEmriId,
            model.TutanakNo,
            model.SahaSonucu,
            model.Gerekce,
            model.MuhurNo
        );

        TempData["Mesaj"] = "Tutanak başarıyla kaydedildi.";
        TempData["MesajTip"] = "success";

        return RedirectToAction("Detay", new { id = model.IsEmriId });
    }
}
