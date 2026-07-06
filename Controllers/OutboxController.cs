using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using System;
using System.Linq;

namespace KcetasWeb.Controllers;

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
                OutboxId = k.Id,
                IslemTipi = k.EventType,
                ReferansNo = k.AggregateId,
                HedefSistem = k.AggregateType,
                Durum = k.Status,
                DurumRenk = OutboxListeViewModel.GetOutboxDurumRenk(k.Status),
                DenemeSayisi = 0,
                SonHataMesaji = "",
                OlusturulmaZamani = k.CreatedAt,
                GonderimZamani = k.ProcessedAt,
                PayloadOnizleme = k.Payload != null && k.Payload.Length > 100 ? k.Payload[..100] + "..." : (k.Payload ?? "")
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
            outboxId = kayit.Id,
            islemTipi = kayit.EventType,
            referansNo = kayit.AggregateId,
            hedefSistem = kayit.AggregateType,
            durum = kayit.Status,
            payload = kayit.Payload,
            denemeSayisi = 0,
            sonHataMesaji = "",
            olusturulmaZamani = kayit.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
            gonderimZamani = kayit.ProcessedAt?.ToString("dd.MM.yyyy HH:mm")
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
