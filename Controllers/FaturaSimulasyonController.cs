// using Microsoft.AspNetCore.Mvc;
// using KcetasWeb.Services.Interfaces;
// using KcetasWeb.ViewModels;

// namespace KcetasWeb.Controllers
// {
//     public class FaturaSimulasyonController : Controller
//     {
//         private readonly IFaturaService _faturaService;

//         public FaturaSimulasyonController(IFaturaService faturaService)
//         {
//             _faturaService = faturaService;
//         }

//         // GET: /FaturaSimulasyon
//         public IActionResult Index()
//         {
//             var viewModel = new FaturaSimulasyonViewModel
//             {
//                 TarifeGrubu = "Mesken",
//                 TuketimMiktari = 0,
//                 DonemBaslangic = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
//                 DonemBitis = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
//                     DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
//             };

//             return View(viewModel);
//         }

//         // POST: /FaturaSimulasyon/Hesapla
//         [HttpPost]
//         public IActionResult Hesapla(FaturaSimulasyonViewModel model)
//         {
//             if (model.TuketimMiktari <= 0)
//             {
//                 ModelState.AddModelError("TuketimMiktari", "Tüketim miktarı 0'dan büyük olmalıdır.");
//                 return View("Index", model);
//             }

//             var sonuc = _faturaService.SimulasyonHesapla(model.TarifeGrubu, model.TuketimMiktari);

//             model.BirimFiyat = sonuc.BirimFiyat;
//             model.EnerjiBedeli = sonuc.EnerjiBedeli;
//             model.DagitimBedeli = sonuc.DagitimBedeli;
//             model.TrtPayi = sonuc.TrtPayi;
//             model.EnerjiFonu = sonuc.EnerjiFonu;
//             model.KdvTutari = sonuc.KdvTutari;
//             model.ToplamTutar = sonuc.ToplamTutar;
//             model.Kalemler = sonuc.Kalemler.Select(k => new FaturaSimulasyonViewModel.SimulasyonKalemViewModel
//             {
//                 KalemAdi = k.KalemAdi,
//                 Miktar = k.Miktar,
//                 BirimFiyat = k.BirimFiyat,
//                 Tutar = k.Tutar
//             }).ToList();

//             return View("Index", model);
//         }

//         // POST: /FaturaSimulasyon/HesaplaAjax (AJAX endpoint)
//         [HttpPost]
//         public IActionResult HesaplaAjax([FromBody] SimulasyonRequest request)
//         {
//             if (string.IsNullOrEmpty(request.TarifeGrubu) || request.TuketimMiktari <= 0)
//                 return BadRequest(new { error = "Geçersiz parametreler" });

//             var sonuc = _faturaService.SimulasyonHesapla(request.TarifeGrubu, request.TuketimMiktari);

//             return Json(new
//             {
//                 birimFiyat = sonuc.BirimFiyat,
//                 enerjiBedeli = sonuc.EnerjiBedeli,
//                 dagitimBedeli = sonuc.DagitimBedeli,
//                 trtPayi = sonuc.TrtPayi,
//                 enerjiFonu = sonuc.EnerjiFonu,
//                 kdvTutari = sonuc.KdvTutari,
//                 toplamTutar = sonuc.ToplamTutar,
//                 kalemler = sonuc.Kalemler.Select(k => new
//                 {
//                     kalemAdi = k.KalemAdi,
//                     miktar = k.Miktar,
//                     birimFiyat = k.BirimFiyat,
//                     tutar = k.Tutar
//                 })
//             });
//         }
//     }

//     public class SimulasyonRequest
//     {
//         public string TarifeGrubu { get; set; }
//         public decimal TuketimMiktari { get; set; }
//     }
// }
