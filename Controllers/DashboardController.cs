using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Models.entities;
using System.Linq;
using System.Threading.Tasks;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi, SayacOkumaPersoneli,SahaOperasyonAmir,FaturalamaUzmani,Denetci ")]
    public class DashboardController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly IAboneService _aboneService;
        private readonly ISozlesmeService _sozlesmeService;
        private readonly IIsEmriService _isEmriService;
        private readonly IFaturaService _faturaService;

        public DashboardController(
            IKullaniciDeposu kullaniciDeposu, 
            ITuketimNoktasiService tuketimNoktasiService,
            IAboneService aboneService,
            ISozlesmeService sozlesmeService,
            IIsEmriService isEmriService,
            IFaturaService faturaService)
        {
            _kullaniciDeposu = kullaniciDeposu;
            _tuketimNoktasiService = tuketimNoktasiService;
            _aboneService = aboneService;
            _sozlesmeService = sozlesmeService;
            _isEmriService = isEmriService;
            _faturaService = faturaService;
        }

        public async Task<IActionResult> Index()
        {
            var kullaniciTask = _kullaniciDeposu.ListeleAsync();
            var tuketimNoktasiTask = _tuketimNoktasiService.GetAllAsync();
            var aboneTask = _aboneService.GetPagedAsync(1, 1);
            var sozlesmeTask = _sozlesmeService.GetAllAsync();
            var isEmriTask = _isEmriService.GetAllAsync();
            var faturaTask = _faturaService.GetAllAsync();

            await Task.WhenAll(kullaniciTask, tuketimNoktasiTask, aboneTask, sozlesmeTask, isEmriTask, faturaTask);

            var kullaniciListe = kullaniciTask.Result;
            ViewBag.ToplamKullanici = kullaniciListe.Count;
            ViewBag.AktifKullanici = kullaniciListe.Count(k => k.durum == KcetasWeb.Models.Enums.KullaniciDurumu.Aktif);
            
            // Veritabanından TuketimNoktasi sayısı
            var tuketimNoktalari = tuketimNoktasiTask.Result;
            ViewBag.AktifTuketimNoktasi = tuketimNoktalari.Count;

            var aboneCount = 0;
            var sozlesmeCount = 0;
            var aktifIsEmriCount = 0;
            var bekleyenFaturaCount = 0;

            try { aboneCount = aboneTask.Result.TotalCount; } catch { }
            try { sozlesmeCount = sozlesmeTask.Result.Count(s => s.durum == KcetasWeb.Models.Enums.SozlesmeDurumu.Aktif); } catch { }
            try { 
                var isEmirleri = isEmriTask.Result;
                aktifIsEmriCount = isEmirleri.Count(i => i.durum != KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi && i.durum != KcetasWeb.Models.Enums.IsEmriDurumu.Iptal);
            } catch { }
            try { bekleyenFaturaCount = faturaTask.Result.Count(f => f.durum == "TASLAK" || f.durum == "HESAPLANDI" || f.durum == "HATALI" || f.durum == "ODENMEDI"); } catch { }

            ViewBag.AboneCount = aboneCount;
            ViewBag.SozlesmeCount = sozlesmeCount;
            ViewBag.AktifIsEmriCount = aktifIsEmriCount;
            ViewBag.BekleyenFaturaCount = bekleyenFaturaCount;
            
            // Recent users
            ViewBag.SonKullanicilar = kullaniciListe
                                        .OrderByDescending(k => k.created_at)
                                        .Take(5).ToList();

            return View();
        }
    }
}
