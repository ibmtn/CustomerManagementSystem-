using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


namespace KcetasWeb.Controllers
{
    

    [Authorize(Roles = "BTYoneticisi,FaturalamaUzmani,Denetcis")]
    public class FaturaController : Controller
    {
        private readonly KcetasWeb.Services.Interfaces.ITuketimNoktasiService _tuketimNoktasiService;
        private readonly KcetasWeb.Services.Interfaces.IFaturaService _faturaService;
        private readonly KcetasWeb.Services.Interfaces.ISozlesmeService _sozlesmeService;
        private readonly KcetasWeb.Services.Interfaces.IAboneService _aboneService;
        private readonly KcetasWeb.Services.Interfaces.IAuditLogService _auditLogService;
        private readonly KcetasWeb.Services.Interfaces.IKullaniciDeposu _kullaniciDeposu;

        public FaturaController(
            KcetasWeb.Services.Interfaces.ITuketimNoktasiService tuketimNoktasiService,
            KcetasWeb.Services.Interfaces.IFaturaService faturaService,
            KcetasWeb.Services.Interfaces.ISozlesmeService sozlesmeService,
            KcetasWeb.Services.Interfaces.IAboneService aboneService,
            KcetasWeb.Services.Interfaces.IAuditLogService auditLogService,
            KcetasWeb.Services.Interfaces.IKullaniciDeposu kullaniciDeposu)
        {
            _tuketimNoktasiService = tuketimNoktasiService;
            _faturaService = faturaService;
            _sozlesmeService = sozlesmeService;
            _aboneService = aboneService;
            _auditLogService = auditLogService;
            _kullaniciDeposu = kullaniciDeposu;
        }

        private async Task<int> GetCurrentUserId()
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(username))
            {
                var user = await _kullaniciDeposu.BulKullaniciAdiIleAsync(username);
                if (user != null) return user.kullanici_id;
            }
            return 1;
        }

        public async Task<IActionResult> Index(KcetasWeb.ViewModels.FaturaListeViewModel filtre)
        {
            filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
            filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;

            var faturaTask = _faturaService.GetPagedAsync(filtre.CurrentPage, filtre.PageSize);
            var sozlesmeTask = _sozlesmeService.GetAllAsync();
            var tuketimNoktasiTask = _tuketimNoktasiService.GetAllAsync();
            
            await Task.WhenAll(faturaTask, sozlesmeTask, tuketimNoktasiTask);

            var pagedResponse = faturaTask.Result;
            var faturalar = pagedResponse.Data;
            var sozlesmeler = sozlesmeTask.Result.GroupBy(s => s.sozlesme_id).ToDictionary(g => g.Key, g => g.First());
            var tuketimNoktalari = tuketimNoktasiTask.Result.GroupBy(t => t.tuketim_noktasi_id).ToDictionary(g => g.Key, g => g.First());
            
            var viewModels = faturalar.Select(f => {
                string gercekTekilKod = f.tekil_kod ?? "";
                string sozlesmeNo = "";
                if (sozlesmeler.ContainsKey(f.sozlesme_id))
                {
                    var sozlesme = sozlesmeler[f.sozlesme_id];
                    sozlesmeNo = sozlesme.sozlesme_no ?? "";
                    if (sozlesme.tuketim_noktasi_id > 0 && tuketimNoktalari.ContainsKey(sozlesme.tuketim_noktasi_id))
                    {
                        gercekTekilKod = tuketimNoktalari[sozlesme.tuketim_noktasi_id].tekil_kod;
                    }
                }

                return new KcetasWeb.ViewModels.FaturaSimulasyonViewModel
                {
                    fatura_id = f.fatura_id,
                    fatura_no = f.fatura_no,
                    sozlesme_id = f.sozlesme_id,
                    okuma_id = f.okuma_id,
                    tekil_kod = gercekTekilKod,
                    fatura_tipi = f.fatura_tipi,
                    fatura_tarihi = f.fatura_tarihi,
                    son_odeme_tarihi = f.son_odeme_tarihi,
                    donem = f.donem,
                    ilk_endeks = f.ilk_endeks,
                    son_endeks = f.son_endeks,
                    tuketim_kwh = f.tuketim_kwh,
                    carpan = f.carpan,
                    enerji_bedeli = f.enerji_bedeli,
                    dagatim_bedeli = f.dagatim_bedeli,
                    vergi_fon_toplama = f.vergi_fon_toplam,
                    toplam_tutar = f.toplam_tutar,
                    durum = f.durum,
                    status = f.status,
                    created_at = f.created_at,
                    TarifeGrubu = f.fatura_tipi?.ToString() ?? "Bilinmiyor",
                    TuketimMiktari = f.tuketim_kwh ?? 0,
                    sozlesme_no = sozlesmeNo
                };
            }).ToList();

            if (!string.IsNullOrEmpty(filtre.FiltreFaturaNo))
                viewModels = viewModels.Where(x => x.fatura_no != null && x.fatura_no.Contains(filtre.FiltreFaturaNo, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrEmpty(filtre.FiltreTekilKod))
                viewModels = viewModels.Where(x => x.tekil_kod != null && x.tekil_kod.Contains(filtre.FiltreTekilKod, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrEmpty(filtre.FiltreDonem))
                viewModels = viewModels.Where(x => x.donem != null && x.donem.Contains(filtre.FiltreDonem, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(filtre.FiltreSozlesmeNo))
                viewModels = viewModels.Where(x => x.sozlesme_no != null && x.sozlesme_no.Contains(filtre.FiltreSozlesmeNo, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(filtre.FiltreDurum))
                viewModels = viewModels.Where(x => x.durum != null && x.durum.Equals(filtre.FiltreDurum, StringComparison.OrdinalIgnoreCase)).ToList();

            viewModels = viewModels
                .OrderBy(x => x.durum?.Equals("ONAYLANDI", StringComparison.OrdinalIgnoreCase) == true ? 1 : 0)
                .ThenByDescending(x => x.created_at)
                .ToList();
            
            filtre.TotalItems = pagedResponse.TotalCount;
            filtre.Faturalar = viewModels;

            return View(filtre);
        }

        public IActionResult Olustur()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Olustur(Fatura fatura)
        {
            fatura.fatura_no = $"FAT-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}";
            fatura.fatura_tarihi = DateOnly.FromDateTime(DateTime.Now);
            fatura.son_odeme_tarihi = DateOnly.FromDateTime(DateTime.Now.AddDays(15));
            fatura.donem = DateTime.Now.ToString("yyyy-MM");
            fatura.durum = "TASLAK";
            fatura.status = "Active";
            fatura.created_at = DateTime.Now;
            fatura.kullanici_id = await GetCurrentUserId();
            fatura.tekil_kod = "BILINMIYOR"; // Varsayılan

            if (fatura.sozlesme_id > 0)
            {
                var sozlesme = (await _sozlesmeService.GetAllAsync()).FirstOrDefault(s => s.sozlesme_id == fatura.sozlesme_id);
                if (sozlesme != null)
                {
                    var tn = (await _tuketimNoktasiService.GetAllAsync()).FirstOrDefault(t => t.tuketim_noktasi_id == sozlesme.tuketim_noktasi_id);
                    if (tn != null)
                    {
                        fatura.tekil_kod = tn.tekil_kod;
                    }
                }
            }
            
            await _faturaService.EkleAsync(fatura);
            
            await _auditLogService.EkleAsync("Fatura", fatura.fatura_id, "CREATE", "", fatura.fatura_no, fatura.kullanici_id ?? 1, "Manuel Yeni Fatura Kesildi");
            
            TempData["BasariMesaji"] = fatura.fatura_no + " numaralı fatura başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detay(int id)
        {
            var fatura = await _faturaService.GetByIdAsync(id);
            if (fatura == null)
            {
                return NotFound();
            }

            string aboneBilgisi = "Abone Bilgisi Alınamadı";
            string gercekTekilKod = fatura.tekil_kod ?? "";

            var sozlesme = (await _sozlesmeService.GetAllAsync()).FirstOrDefault(s => s.sozlesme_id == fatura.sozlesme_id);
            if (sozlesme != null)
            {
                if (sozlesme.abone_id > 0)
                {
                    var abone = await _aboneService.GetByIdAsync((int)sozlesme.abone_id);
                    if (abone != null)
                    {
                        aboneBilgisi = (abone.abone_tipi == KcetasWeb.Models.Enums.AboneTipi.Kurumsal)
                            ? $"{abone.Unvan} (VKN: {abone.vkn})"
                            : $"{abone.Ad} {abone.Soyad} (TC: {abone.tckn})";
                    }
                }

                if (sozlesme.tuketim_noktasi_id > 0)
                {
                    var tn = (await _tuketimNoktasiService.GetAllAsync()).FirstOrDefault(t => t.tuketim_noktasi_id == sozlesme.tuketim_noktasi_id);
                    if (tn != null)
                    {
                        gercekTekilKod = tn.tekil_kod;
                    }
                }
            }

            ViewBag.AboneBilgisi = aboneBilgisi;

            var viewModel = new KcetasWeb.ViewModels.FaturaSimulasyonViewModel
            {
                fatura_id = fatura.fatura_id,
                fatura_no = fatura.fatura_no,
                sozlesme_id = fatura.sozlesme_id,
                okuma_id = fatura.okuma_id,
                tekil_kod = gercekTekilKod,
                fatura_tipi = fatura.fatura_tipi,
                fatura_tarihi = fatura.fatura_tarihi,
                son_odeme_tarihi = fatura.son_odeme_tarihi,
                donem = fatura.donem,
                ilk_endeks = fatura.ilk_endeks,
                son_endeks = fatura.son_endeks,
                tuketim_kwh = fatura.tuketim_kwh,
                carpan = fatura.carpan,
                enerji_bedeli = fatura.enerji_bedeli,
                dagatim_bedeli = fatura.dagatim_bedeli,
                vergi_fon_toplama = fatura.vergi_fon_toplam,
                toplam_tutar = fatura.toplam_tutar,
                durum = fatura.durum,
                status = fatura.status,
                created_at = fatura.created_at,
                TarifeGrubu = fatura.fatura_tipi?.ToString() ?? "Bilinmiyor",
                TuketimMiktari = fatura.tuketim_kwh ?? 0,
                Kalemler = new List<KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel>
                {
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Enerji Bedeli", Miktar = fatura.tuketim_kwh ?? 0, BirimFiyat = 2.85m, Tutar = fatura.enerji_bedeli ?? 0 },
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Dağıtım Bedeli", Miktar = fatura.tuketim_kwh ?? 0, BirimFiyat = 0.65m, Tutar = fatura.dagatim_bedeli ?? 0 },
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Vergi ve Fonlar", Miktar = 1, BirimFiyat = fatura.vergi_fon_toplam ?? 0, Tutar = fatura.vergi_fon_toplam ?? 0 },
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Hizmet Bedeli", Miktar = 1, BirimFiyat = 0, Tutar = 0 },
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Kesme / Bağlama Bedeli", Miktar = 1, BirimFiyat = 0, Tutar = 0 },
                    new KcetasWeb.ViewModels.FaturaSimulasyonViewModel.SimulasyonKalemViewModel { KalemAdi = "Gecikme Zammı", Miktar = 1, BirimFiyat = 0, Tutar = 0 }
                }
            };

            return View(viewModel);
        }

        public async Task<IActionResult> OdemeYap(int id)
        {
            var fatura = await _faturaService.GetByIdAsync(id);
            if (fatura != null)
            {
                string eskiDurum = fatura.durum;
                fatura.durum = "Ödendi";
                fatura.updated_at = DateTime.Now;
                await _faturaService.GuncelleAsync(fatura);
                
                await _auditLogService.EkleAsync("Fatura", fatura.fatura_id, "PAYMENT", eskiDurum, "Ödendi", await GetCurrentUserId(), "Fatura Ödemesi Alındı");
                
                TempData["FaturaMesaji"] = fatura.fatura_no + " numaralı fatura başarıyla ödendi.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Odeme(int id)
        {
            var fatura = await _faturaService.GetByIdAsync(id);
            if (fatura == null) return NotFound();

            return View(fatura);
        }

        [HttpPost]
        public async Task<IActionResult> Odeme(int id, string cardName, string cardNumber, string expiryDate, string cvv)
        {
            var fatura = await _faturaService.GetByIdAsync(id);
            if (fatura == null) return NotFound();

            string eskiDurum = fatura.durum;
            fatura.durum = "ODENDI";
            fatura.updated_at = DateTime.Now;
            await _faturaService.GuncelleAsync(fatura);
            
            await _auditLogService.EkleAsync("Fatura", fatura.fatura_id, "PAYMENT", eskiDurum, "ODENDI", await GetCurrentUserId(), "Kredi Kartı ile Online Ödeme");
            
            TempData["SuccessMessage"] = "Ödeme Başarılı! Faturanız ödendi.";
            return RedirectToAction("Detay", new { id = fatura.fatura_id });
        }
    }
}
