using Microsoft.AspNetCore.Mvc;


using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;

using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using System;
using System.Linq;

using System.Threading.Tasks;

namespace KcetasWeb.Controllers;

[Authorize(Roles = AppRoles.BTYoneticisi)]
public class OutboxController : Controller
{
    private readonly IOutboxService _outboxService;

    public OutboxController(IOutboxService outboxService)
    {
        _outboxService = outboxService;
    }

    public async Task<IActionResult> Index(string? durum, string? hedefSistem,
        DateTime? olusturmaTarihi, string? faturaNo, long? kayitNo, DateTime? sonDenemeTarihi, int CurrentPage = 1)
    {
        int pageSize = 50;
        
        DateTime? bitisTarihi = olusturmaTarihi.HasValue ? olusturmaTarihi.Value.Date.AddDays(1).AddTicks(-1) : null;

        // Pass olusturmaTarihi as baslangic and bitisTarihi as bitis to IOutboxService to filter for a single day
        var paginatedResult = await _outboxService.FiltreleAsync(durum, hedefSistem, olusturmaTarihi, bitisTarihi, faturaNo, kayitNo, sonDenemeTarihi, CurrentPage, pageSize);
        var istatistikler = await _outboxService.GetIstatistiklerAsync();

        var viewModels = paginatedResult.Data.Select(k => new OutboxListeViewModel.OutboxSatirViewModel
        {
            OutboxId = k.outbox_id,
            FaturaId = k.fatura_id,
            FaturaNo = k.fatura?.fatura_no,
            ReferansNo = k.corrolation_id ?? k.fatura?.fatura_no ?? k.idempotency_key,
            HedefSistem = k.hedef_sistem?.ToString(),
            IdempotencyKey = k.idempotency_key,
            Durum = k.durum?.ToString(),
            DurumEtiketi = OutboxListeViewModel.GetOutboxDurumEtiketi(k.durum?.ToString()),
            DurumRenk = OutboxListeViewModel.GetOutboxDurumRenk(k.durum?.ToString()),
            DenemeSayisi = k.retry_count,
            HataKodu = k.hata_kodu,
            SonHataMesaji = k.hata_mesaji,
            OlusturulmaZamani = k.created_at,
            SonDenemeTarihi = k.son_deneme_tarihi,
            GonderimZamani = k.gonderim_zamani,
            PayloadOnizleme = k.paload != null && k.paload.Length > 120 ? k.paload[..120] + "..." : (k.paload ?? "")
        }).ToList();

        var viewModel = new OutboxListeViewModel
        {
            FiltreDurum = durum,
            FiltreHedefSistem = hedefSistem,
            FiltreFaturaNo = faturaNo,
            FiltreKayitNo = kayitNo,
            OlusturmaTarihi = olusturmaTarihi,
            SonDenemeTarihi = sonDenemeTarihi,
            ToplamKayit = istatistikler.Toplam,
            BekleyenSayisi = istatistikler.Bekleyen,
            GonderilmisSayisi = istatistikler.Gonderilmis,
            BasarisizSayisi = istatistikler.Basarisiz,
            Kayitlar = viewModels,
            CurrentPage = CurrentPage,
            PageSize = pageSize,
            TotalItems = paginatedResult.TotalCount
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Detay(long id)
    {
        var kayit = await _outboxService.GetByIdAsync(id);
        if (kayit == null)
            return NotFound();

        return Json(new
        {
            outboxId = kayit.outbox_id,
            faturaId = kayit.fatura_id,
            faturaNo = kayit.fatura?.fatura_no,
            referansNo = kayit.corrolation_id ?? kayit.fatura?.fatura_no ?? kayit.idempotency_key,
            hedefSistem = kayit.hedef_sistem?.ToString(),
            durum = kayit.durum?.ToString(),
            durumEtiketi = OutboxListeViewModel.GetOutboxDurumEtiketi(kayit.durum?.ToString()),
            idempotencyKey = kayit.idempotency_key,
            correlationId = kayit.corrolation_id,
            payload = kayit.paload,
            denemeSayisi = kayit.retry_count,
            hataKodu = kayit.hata_kodu,
            sonHataMesaji = kayit.hata_mesaji,
            olusturulmaZamani = kayit.created_at.ToString("dd.MM.yyyy HH:mm"),
            sonDenemeTarihi = kayit.son_deneme_tarihi?.ToString("dd.MM.yyyy HH:mm") ?? "-",
            gonderimZamani = kayit.gonderim_zamani?.ToString("dd.MM.yyyy HH:mm") ?? "-"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> YenidenGonder(long id)
    {
        var sonuc = await _outboxService.YenidenGonderAsync(id);

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
