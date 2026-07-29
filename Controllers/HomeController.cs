using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;

namespace ProjeStaj.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly KcetasWeb.Services.Interfaces.IAboneService _aboneService;
    private readonly KcetasWeb.Services.Interfaces.ISozlesmeService _sozlesmeService;
    private readonly KcetasWeb.Services.Interfaces.IIsEmriService _isEmriService;
    private readonly KcetasWeb.Services.Interfaces.IFaturaService _faturaService;

    public HomeController(
        ILogger<HomeController> logger,
        KcetasWeb.Services.Interfaces.IAboneService aboneService,
        KcetasWeb.Services.Interfaces.ISozlesmeService sozlesmeService,
        KcetasWeb.Services.Interfaces.IIsEmriService isEmriService,
        KcetasWeb.Services.Interfaces.IFaturaService faturaService)
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
