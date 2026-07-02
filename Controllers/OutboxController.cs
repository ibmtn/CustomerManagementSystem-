using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models.enums;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;

namespace KcetasWeb.Controllers;

public class OutboxController : Controller
{
    private readonly IOutboxService _outboxService;

    public OutboxController(IOutboxService outboxService)
    {
        _outboxService = outboxService;
    }

    // GET: /Outbox
    public IActionResult Index(OutboxDurumu? durum, string? islemTipi,
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
                IslemTipi = k.islem_tipi,
                ReferansNo = k.referans_no,
                HedefSistem = k.hedef_sistem,
                Durum = k.durum,
                DurumAdi = OutboxListeViewModel.GetOutboxDurumAdi(k.durum),
                DurumRenk = OutboxListeViewModel.GetOutboxDurumRenk(k.durum),
                DenemeSayisi = k.deneme_sayisi,
                SonHataMesaji = k.son_hata_mesaji,
                OlusturulmaZamani = k.olusturulma_zamani,
                GonderimZamani = k.gonderim_zamani,
                PayloadOnizleme = k.payload.Length > 100 ? k.payload[..100] + "..." : k.payload
            }).ToList()
        };

        return View(viewModel);
    }

    // GET: /Outbox/Detay/5
    public IActionResult Detay(int id)
    {
        var kayit = _outboxService.GetById(id);
        if (kayit == null)
            return NotFound();

        return Json(new
        {
            outboxId = kayit.outbox_id,
            islemTipi = kayit.islem_tipi,
            referansNo = kayit.referans_no,
            hedefSistem = kayit.hedef_sistem,
            durum = kayit.durum.ToString(),
            payload = kayit.payload,
            denemeSayisi = kayit.deneme_sayisi,
            sonHataMesaji = kayit.son_hata_mesaji,
            olusturulmaZamani = kayit.olusturulma_zamani.ToString("dd.MM.yyyy HH:mm"),
            gonderimZamani = kayit.gonderim_zamani?.ToString("dd.MM.yyyy HH:mm")
        });
    }

    // POST: /Outbox/YenidenGonder/5
    [HttpPost]
    public IActionResult YenidenGonder(int id)
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
