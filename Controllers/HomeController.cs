using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MusteriYonetim.Models;

namespace ProjeStaj.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MusteriYonetim.Services.Interfaces.IAboneService _aboneService;
    private readonly MusteriYonetim.Services.Interfaces.ISozlesmeService _sozlesmeService;
    private readonly MusteriYonetim.Services.Interfaces.IIsEmriService _isEmriService;
    private readonly MusteriYonetim.Services.Interfaces.IFaturaService _faturaService;

    public HomeController(
        ILogger<HomeController> logger,
        MusteriYonetim.Services.Interfaces.IAboneService aboneService,
        MusteriYonetim.Services.Interfaces.ISozlesmeService sozlesmeService,
        MusteriYonetim.Services.Interfaces.IIsEmriService isEmriService,
        MusteriYonetim.Services.Interfaces.IFaturaService faturaService)
    {
        _logger = logger;
        _aboneService = aboneService;
        _sozlesmeService = sozlesmeService;
        _isEmriService = isEmriService;
        _faturaService = faturaService;
    }

    public async System.Threading.Tasks.Task<IActionResult> Index()
    {
        // Kullanıcılar eski Ana Sayfaya veya Profil'den dönüşte buraya düşerse 
        // doğrudan Gösterge Paneline (Dashboard) yönlendiriliyor.
        return RedirectToAction("Index", "Dashboard");
    }

    public async System.Threading.Tasks.Task<IActionResult> Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async System.Threading.Tasks.Task<IActionResult> Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
