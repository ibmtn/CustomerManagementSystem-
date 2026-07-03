using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models.enums;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;

namespace KcetasWeb.Controllers;

public class IsEmriController : Controller
{
    private readonly IIsEmriService _isEmriService;

    public IsEmriController(IIsEmriService isEmriService)
    {
        _isEmriService = isEmriService;
    }

    // GET: /IsEmri
    public IActionResult Index(IsEmriTipi? tip, IsEmriDurumu? durum,
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
                IsEmriId = (int)ie.IsEmriId,
                IsEmriNo = ie.IsEmriNo,
                Tip = ie.Tip,
                TipAdi = IsEmriListeViewModel.GetTipAdi(ie.Tip),
                TuketimNoktasiKodu = ie.TuketimNoktasiKodu ?? "-",
                PlanlananTarih = ie.PlanlananTarih,
                AtananKullaniciAdi = ie.AtananKullaniciAdi,
                Durum = ie.Durum,
                DurumAdi = IsEmriListeViewModel.GetDurumAdi(ie.Durum),
                DurumRenk = IsEmriListeViewModel.GetDurumRenk(ie.Durum),
                Adres = ie.Adres
            }).ToList()
        };

        return View(viewModel);
    }

    // GET: /IsEmri/Detay/5
    public IActionResult Detay(int id)
    {
        var isEmri = _isEmriService.GetById(id);
        if (isEmri == null)
            return NotFound();

        var viewModel = new IsEmriDetayViewModel
        {
            IsEmriId = (int)isEmri.IsEmriId,
            IsEmriNo = isEmri.IsEmriNo,
            Tip = isEmri.Tip,
            TipAdi = IsEmriListeViewModel.GetTipAdi(isEmri.Tip),
            Durum = isEmri.Durum,
            DurumAdi = IsEmriListeViewModel.GetDurumAdi(isEmri.Durum),
            DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.Durum),
            Oncelik = isEmri.Oncelik,
            PlanlananTarih = isEmri.PlanlananTarih,
            AtananKullaniciAdi = isEmri.AtananKullaniciAdi,
            TuketimNoktasiKodu = isEmri.TuketimNoktasiKodu,
            Adres = isEmri.Adres,
            SayacSeriNo = isEmri.SayacSeriNo,
            SahaSonucu = isEmri.SahaSonucu,
            Gerekce = isEmri.Gerekce,
            EskiSayacNo = isEmri.EskiSayacNo,
            EskiSonEndeksi = isEmri.EskiSonEndeksi,
            YeniSayacNo = isEmri.YeniSayacNo,
            YeniIlkEndeksi = isEmri.YeniIlkEndeksi,
            MuhurNo = isEmri.MuhurNo,
            TutanakNo = isEmri.TutanakNo,
            KesmeEndeksi = isEmri.KesmeEndeksi,
            AcmaEndeksi = isEmri.AcmaEndeksi,
            CreatedAt = isEmri.CreatedAt,
            UpdatedAt = isEmri.UpdatedAt
        };

        return View(viewModel);
    }

    // GET: /IsEmri/TutanakGiris/5
    public IActionResult TutanakGiris(int id)
    {
        var isEmri = _isEmriService.GetById(id);
        if (isEmri == null)
            return NotFound();

        var viewModel = new TutanakGirisViewModel
        {
            IsEmriId = (int)isEmri.IsEmriId,
            IsEmriNo = isEmri.IsEmriNo,
            Tip = isEmri.Tip,
            TipAdi = IsEmriListeViewModel.GetTipAdi(isEmri.Tip),
            IslemTarihi = DateTime.Now,
            // Mevcut sayaç bilgilerini önceden doldur
            EskiSayacNo = isEmri.SayacSeriNo
        };

        return View(viewModel);
    }

    // POST: /IsEmri/TutanakKaydet
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
            model.TutanakNo!,
            model.SahaSonucu!,
            model.Gerekce,
            model.EskiSayacNo,
            model.EskiSonEndeksi,
            model.YeniSayacNo,
            model.YeniIlkEndeksi,
            model.MuhurNo,
            model.KesmeEndeksi,
            model.AcmaEndeksi
        );

        TempData["Mesaj"] = "Tutanak başarıyla kaydedildi.";
        TempData["MesajTip"] = "success";

        return RedirectToAction("Detay", new { id = model.IsEmriId });
    }
}
