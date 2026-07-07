using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using System;
using System.Linq;

namespace KcetasWeb.Controllers;

[Authorize(Roles = "BTYoneticisi,SahaEkibi,IsEmriYetkilisi")]
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
                IsEmriId = ie.is_emri_id,
                IsEmriNo = ie.is_emri_no,
                Tip = ie.tip,
                TuketimNoktasiId = ie.tuketim_noktasi_id,
                TuketimNoktasiKodu = "Bilgi Yok",
                PlanlananTarih = ie.planlanan_tarih,
                AtananKullaniciAdi = "Kullanıcı " + ie.atanan_kullanici_id,
                Durum = ie.durum,
                DurumRenk = IsEmriListeViewModel.GetDurumRenk(ie.durum),
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
            IsEmriId = isEmri.is_emri_id,
            IsEmriNo = isEmri.is_emri_no,
            Tip = isEmri.tip,
            Durum = isEmri.durum,
            DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.durum),
            Oncelik = isEmri.oncelik,
            PlanlananTarih = isEmri.planlanan_tarih,
            AtananKullaniciAdi = "Kullanıcı " + isEmri.atanan_kullanici_id,
            TuketimNoktasiKodu = "Bilgi Yok",
            Adres = "Adres bilgisi alınamadı",
            SayacSeriNo = "Sayaç bilgisi yok",
            SahaSonucu = isEmri.saha_sonucu,
            Gerekce = isEmri.gerekce,
            MuhurNo = isEmri.muhur_no,
            TutanakNo = isEmri.tutanak_no,
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
            IsEmriId = isEmri.is_emri_id,
            IsEmriNo = isEmri.is_emri_no,
            Tip = isEmri.tip,
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
            model.MuhurNo,
            model.KesmeEndeksi,
            model.AcmaEndeksi,
            model.EskiSayacNo,
            model.YeniSayacNo,
            model.EskiSonEndeksi,
            model.YeniIlkEndeksi
        );
        
        string mesaj = "Tutanak başarıyla kaydedildi.";
        
        // Simülasyon: Kesme veya Açma yapıldıysa
        if (model.Tip == "Kesme" && model.KesmeEndeksi.HasValue)
        {
            mesaj = $"Tutanak başarıyla kaydedildi. (Kesme Endeksi: {model.KesmeEndeksi}). Borç veya sözleşme nedeniyle yapılan kesme için, bir sonraki faturaya yansıtılacak olan 'Kesme-Bağlama Bedeli' (Örn: 118,50 TL) simülasyonu başlatılmıştır.";
        }
        else if (model.Tip == "Açma" && model.AcmaEndeksi.HasValue)
        {
            mesaj = $"Tutanak başarıyla kaydedildi. (Açma Endeksi: {model.AcmaEndeksi}). Ödeme/Sözleşme onayı sonrası açma işlemi tamamlandı.";
        }

        TempData["Mesaj"] = mesaj;
        TempData["MesajTip"] = "success";

        return RedirectToAction("Detay", new { id = model.IsEmriId });
    }

    public IActionResult TutanakGoruntule(long id)
    {
        var isEmri = _isEmriService.GetById(id);
        if (isEmri == null || string.IsNullOrEmpty(isEmri.tutanak_no))
            return NotFound();

        var viewModel = new IsEmriDetayViewModel
        {
            IsEmriId = isEmri.is_emri_id,
            IsEmriNo = isEmri.is_emri_no,
            Tip = isEmri.tip,
            Durum = isEmri.durum,
            DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.durum),
            Oncelik = isEmri.oncelik,
            PlanlananTarih = isEmri.planlanan_tarih,
            SahaSonucu = isEmri.saha_sonucu,
            Gerekce = isEmri.gerekce,
            MuhurNo = isEmri.muhur_no,
            TutanakNo = isEmri.tutanak_no,
            UpdatedAt = isEmri.UpdatedAt,
            EskiSayacNo = isEmri.eski_sayac_no,
            YeniSayacNo = isEmri.yeni_sayac_no,
            EskiSonEndeksi = isEmri.eski_son_endeksi,
            YeniIlkEndeksi = isEmri.yeni_ilk_endeksi,
            KesmeEndeksi = isEmri.kesme_endeksi,
            AcmaEndeksi = isEmri.acma_endeksi
        };

        return View(viewModel);
    }
}
