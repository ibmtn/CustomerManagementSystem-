using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models.enums;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;

namespace KcetasWeb.Controllers;

public class EndeksOkumaController : Controller
{
    private readonly IEndeksOkumaService _endeksOkumaService;

    public EndeksOkumaController(IEndeksOkumaService endeksOkumaService)
    {
        _endeksOkumaService = endeksOkumaService;
    }

    // GET: /EndeksOkuma
    public IActionResult Index(OkumaKaynagi? kaynak, OkumaDurumu? durum,
        DateTime? baslangicTarih, DateTime? bitisTarih, string? arama)
    {
        var okumalar = _endeksOkumaService.Filtrele(kaynak, durum, baslangicTarih, bitisTarih, arama);
        var istatistikler = _endeksOkumaService.GetIstatistikler();

        var viewModel = new EndeksOkumaListeViewModel
        {
            FiltreKaynak = kaynak,
            FiltreDurum = durum,
            BaslangicTarih = baslangicTarih,
            BitisTarih = bitisTarih,
            AramaMetni = arama,
            ToplamOkuma = istatistikler.Toplam,
            ManuelOkuma = istatistikler.Manuel,
            OSOSOkuma = istatistikler.OSOS,
            AnomaliSayisi = istatistikler.Anomali,
            OrtalamaTuketim = istatistikler.OrtalamaTuketim,
            Okumalar = okumalar.Select(o => new EndeksOkumaListeViewModel.OkumaSatirViewModel
            {
                OkumaId = (int)o.OkumaId,
                TuketimNoktasiKodu = o.TuketimNoktasiKodu,
                SayacSeriNo = o.SayacSeriNo,
                OkumaTarihi = o.OkumaTarihi,
                OncekiEndeks = o.OncekiEndeks,
                GuncelEndeks = o.GuncelEndeks,
                TuketimMiktari = o.TuketimMiktari,
                Kaynak = o.OkumaKaynagi,
                KaynakAdi = EndeksOkumaListeViewModel.GetKaynakAdi(o.OkumaKaynagi),
                Durum = o.OkumaDurumu,
                DurumAdi = EndeksOkumaListeViewModel.GetOkumaDurumAdi(o.OkumaDurumu),
                DurumRenk = EndeksOkumaListeViewModel.GetOkumaDurumRenk(o.OkumaDurumu),
                DogrulamaDurumu = o.DogrulamaDurumu,
                AnomaliAciklamasi = o.AnomaliAciklamasi,
                TarifeGrubu = o.TarifeGrubu
            }).ToList()
        };

        return View(viewModel);
    }
}
