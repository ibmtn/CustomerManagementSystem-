using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using System;
using System.Linq;

namespace KcetasWeb.Controllers;

[Authorize(Roles = "BTYoneticisi,SahaOperasyonAmiri,SayacOkumaPersoneli")]
public class IsEmriController : Controller
{
    private readonly IIsEmriService _isEmriService;
    private readonly IKullaniciDeposu _kullaniciDeposu;

    public IsEmriController(IIsEmriService isEmriService, IKullaniciDeposu kullaniciDeposu)
    {
        _isEmriService = isEmriService;
        _kullaniciDeposu = kullaniciDeposu;
    }

    public IActionResult Index(IsEmriListeViewModel filtre)
    {
        var isEmirleri = _isEmriService.Filtrele(filtre.FiltreTip, filtre.FiltreDurum, filtre.BaslangicTarih, filtre.BitisTarih, filtre.AramaMetni);

        filtre.IsEmirleri = isEmirleri.Select(ie => {
            var kullanici = ie.atanan_kullanici_id.HasValue ? _kullaniciDeposu.BulId(ie.atanan_kullanici_id.Value) : null;
            var tn = TuketimNoktasiController._tuketimNoktalari.FirstOrDefault(t => t.TuketimNoktasiId == ie.tuketim_noktasi_id);

            return new IsEmriSatirViewModel
            {
                IsEmriId = ie.is_emri_id,
                IsEmriNo = ie.is_emri_no,
                Tip = ie.tip,
                TuketimNoktasiId = ie.tuketim_noktasi_id,
                tekil_kod = tn != null ? tn.tekil_kod : $"TK-ID-{ie.tuketim_noktasi_id}",
                TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{ie.tuketim_noktasi_id}",
                musteri_ad = tn?.musteri_ad,
                musteri_soyad = tn?.musteri_soyad,
                musteri_unvan = tn?.musteri_unvan,
                PlanlananTarih = ie.planlanan_tarih,
                olusturulma_tarihi = ie.CreatedAt,
                oncelik = ie.oncelik,
                AtananKullaniciAdi = kullanici != null ? kullanici.ad_soyad : "Atanmadı",
                Durum = ie.durum,
                DurumRenk = IsEmriListeViewModel.GetDurumRenk(ie.durum),
                Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı"
            };
        }).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreIsEmriNo))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.IsEmriNo != null && x.IsEmriNo.Contains(filtre.FiltreIsEmriNo, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreTekilKod))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.tekil_kod != null && x.tekil_kod.Contains(filtre.FiltreTekilKod, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreAboneAdi))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.musteriDurum != null && x.musteriDurum.Contains(filtre.FiltreAboneAdi, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltrePersonel))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.AtananKullaniciAdi != null && x.AtananKullaniciAdi.Contains(filtre.FiltrePersonel, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreOncelik))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.oncelik != null && x.oncelik.Contains(filtre.FiltreOncelik, StringComparison.OrdinalIgnoreCase)).ToList();

        return View(filtre);
    }

        public IActionResult Detay(long id)
        {
            var isEmri = _isEmriService.GetById(id);
            if (isEmri == null)
                return NotFound();

            var tn = TuketimNoktasiController._tuketimNoktalari.FirstOrDefault(t => t.TuketimNoktasiId == isEmri.tuketim_noktasi_id);

            var viewModel = new IsEmriDetayViewModel
            {
                IsEmriId = isEmri.is_emri_id,
                IsEmriNo = isEmri.is_emri_no,
                Tip = isEmri.tip,
                Durum = isEmri.durum,
                DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.durum),
                Oncelik = isEmri.oncelik,
                PlanlananTarih = isEmri.planlanan_tarih,
                AtananKullaniciAdi = isEmri.atanan_kullanici_id.HasValue 
                    ? (_kullaniciDeposu.BulId(isEmri.atanan_kullanici_id.Value)?.ad_soyad ?? "Atanmadı") 
                    : "Atanmadı",
                musteri_ad = tn?.musteri_ad,
                musteri_soyad = tn?.musteri_soyad,
                musteri_unvan = tn?.musteri_unvan,
                telefon = tn?.telefon,
                TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{isEmri.tuketim_noktasi_id}",
                Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı",
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

        var tn = TuketimNoktasiController._tuketimNoktalari.FirstOrDefault(t => t.TuketimNoktasiId == isEmri.tuketim_noktasi_id);

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
            TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{isEmri.tuketim_noktasi_id}",
            Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı",
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
