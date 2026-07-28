using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using KcetasWeb.Models.entities;



using System.Threading.Tasks;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,Denetci")]
    public class AboneSorgulamaController : Controller
    {
        private readonly IAboneService _aboneService;

        public AboneSorgulamaController(IAboneService aboneService)
        {
            _aboneService = aboneService;
        }

        public IActionResult Index(string q)
        {
            ViewBag.Query = q;
            return View(null);
        }

        [HttpGet]
        public async Task<IActionResult> SearchData(string q, int page = 1, int pageSize = 50)
        {
            var tumAboneler = await _aboneService.GetAllAsync();
            
            IEnumerable<Abone> query = tumAboneler;
            
            if (!string.IsNullOrEmpty(q) && !q.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = tumAboneler.Where(a =>
                {
                    string adSoyad = $"{a.Ad} {a.Soyad}".Trim();
                    return (a.abone_no != null && a.abone_no.Contains(q, StringComparison.CurrentCultureIgnoreCase)) ||
                           (adSoyad.Contains(q, StringComparison.CurrentCultureIgnoreCase)) ||
                           (a.tckn != null && a.tckn.Contains(q, StringComparison.CurrentCultureIgnoreCase)) ||
                           (a.vkn != null && a.vkn.Contains(q, StringComparison.CurrentCultureIgnoreCase)) ||
                           (a.Unvan != null && a.Unvan.Contains(q, StringComparison.CurrentCultureIgnoreCase));
                });
            }

            int totalItems = query.Count();
            var results = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Json(new {
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                Data = results
            });
        }

        public async Task<IActionResult> Detay(long id)
        {
            var abone = await _aboneService.GetByIdAsync((int)id);
            if (abone == null)
            {
                return NotFound();
            }
            return View(abone);
        }
    }
}