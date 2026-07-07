using Microsoft.AspNetCore.Mvc;


using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;

using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using System;
using System.Linq;

namespace KcetasWeb.Controllers;

[Authorize(Roles = "BTYoneticisi")]
public class OutboxController : Controller
{
    private readonly IOutboxService _outboxService;

    public OutboxController(IOutboxService outboxService)
    {
        _outboxService = outboxService;
    }

    public IActionResult Index(string? durum, string? islemTipi,
        DateTime? baslangicTarih, DateTime? bitisTarih)
    {
        var kayitlar = _outboxService.Filtrele(durum, islemTipi, baslangicTarih, bitisTarih);
        var istatistikler = _outboxService.GetIstatistikler();

        var viewModel = new OutboxListeViewModel
        {
            FiltreDurum = durum,
            FiltreIslemTipi = islemTipi,
            BaslangicTarih = baslangicTarih,
            BitisTarih = bitisTarih,
            ToplamKayit = istatistikler.Toplam,
            BekleyenSayisi = istatistikler.Bekleyen,
            GonderilmisSayisi = istatistikler.Gonderilmis,
            BasarisizSayisi = istatistikler.Basarisiz,
            Kayitlar = kayitlar.Select(k => new OutboxListeViewModel.OutboxSatirViewModel
            {
                OutboxId = k.outbox_id,
                IslemTipi = "Fatura Entegrasyonu",
                ReferansNo = k.corrolation_id,
                HedefSistem = k.hedef_sistem,
                Durum = k.durum,
                DurumRenk = OutboxListeViewModel.GetOutboxDurumRenk(k.durum),
                DenemeSayisi = k.retry_count,
                SonHataMesaji = k.hata_mesaji,
                OlusturulmaZamani = k.created_at,
                GonderimZamani = k.gonderim_zamani,
                PayloadOnizleme = k.paload != null && k.paload.Length > 100 ? k.paload[..100] + "..." : (k.paload ?? "")
            }).ToList()
        };

        return View(viewModel);
    }

    public IActionResult Detay(long id)
    {
        var kayit = _outboxService.GetById(id);
        if (kayit == null)
            return NotFound();

        return Json(new
        {
            outboxId = kayit.outbox_id,
            islemTipi = "Fatura Entegrasyonu",
            referansNo = kayit.corrolation_id,
            hedefSistem = kayit.hedef_sistem,
            durum = kayit.durum,
            payload = kayit.paload,
            denemeSayisi = kayit.retry_count,
            sonHataMesaji = kayit.hata_mesaji,
            olusturulmaZamani = kayit.created_at.ToString("dd.MM.yyyy HH:mm"),
            gonderimZamani = kayit.gonderim_zamani.ToString("dd.MM.yyyy HH:mm")
        });
    }

    [HttpPost]
    public IActionResult YenidenGonder(long id)
    {
        var sonuc = _outboxService.YenidenGonder(id);

        if (sonuc)
        {
            TempData["Mesaj"] = "Kayıt yeniden gönderim kuyruğuna eklendi.";
            TempData["MesajTip"] = "success";
        }
        else
        {
            TempData["Mesaj"] = "Yeniden gönderim başarısız oldu.";
            TempData["MesajTip"] = "danger";
        }

        return RedirectToAction("Index");
    }
}
