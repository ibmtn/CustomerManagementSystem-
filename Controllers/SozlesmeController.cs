using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,Denetci ")]
    public class SozlesmeController : Controller
    {
        private readonly ISozlesmeService _sozlesmeService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly IIsEmriService _isEmriService;
        private readonly IAboneService _aboneService;
        private readonly IAuditLogService _auditLogService;
        private readonly IKullaniciDeposu _kullaniciDeposu;

        public SozlesmeController(
            ISozlesmeService sozlesmeService, 
            ITuketimNoktasiService tuketimNoktasiService,
            IIsEmriService isEmriService,
            IAboneService aboneService,
            IAuditLogService auditLogService,
            IKullaniciDeposu kullaniciDeposu)
        {
            _sozlesmeService = sozlesmeService;
            _tuketimNoktasiService = tuketimNoktasiService;
            _isEmriService = isEmriService;
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

        public async System.Threading.Tasks.Task<IActionResult> Index(KcetasWeb.ViewModels.SozlesmeListeViewModel filtre)
        {
            filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
            filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;

            List<Sozlesme> pagedData;
            int totalItems;

            var pagedResponse = await _sozlesmeService.GetPagedAsync(
                filtre.CurrentPage,
                filtre.PageSize,
                filtre.FiltreSozlesmeNo,
                filtre.FiltreDurum,
                filtre.FiltreTekilKod,
                filtre.FiltreSozlesmeTipi);

            pagedData = pagedResponse.Data;
            totalItems = pagedResponse.TotalCount;
            
            filtre.TotalItems = totalItems;

            var aboneIds = pagedData.Where(x => x.abone_id.HasValue).Select(x => x.abone_id.Value).Distinct();
            var aboneTasks = aboneIds.Select(id => _aboneService.GetByIdAsync(id));

            var tnIds = pagedData.Where(s => string.IsNullOrWhiteSpace(s.tekil_kod)).Select(s => s.tuketim_noktasi_id).Distinct();
            var tnTasks = tnIds.Select(id => _tuketimNoktasiService.GetByIdAsync(id));
            
            var aboneSonuclarTask = Task.WhenAll(aboneTasks);
            var tnSonuclarTask = Task.WhenAll(tnTasks);
            
            await Task.WhenAll(aboneSonuclarTask, tnSonuclarTask);

            var aboneler = (await aboneSonuclarTask).Where(a => a != null).GroupBy(a => a.abone_id).ToDictionary(g => g.Key, g => g.First());
            var tuketimNoktalari = (await tnSonuclarTask).Where(t => t != null).GroupBy(t => t.tuketim_noktasi_id).ToDictionary(g => g.Key, g => g.First());

            var viewModels = pagedData.Select(s => {
                var abone = aboneler.ContainsKey(s.abone_id ?? 0) ? aboneler[s.abone_id ?? 0] : null;
                var tuketimNoktasi = tuketimNoktalari.ContainsKey(s.tuketim_noktasi_id) ? tuketimNoktalari[s.tuketim_noktasi_id] : null;
                
                return new KcetasWeb.ViewModels.SozlesmeViewModels
                {
                    sozlesme_id = s.sozlesme_id,
                    sozlesme_no = s.sozlesme_no,
                    tuketim_noktasi_id = s.tuketim_noktasi_id,
                    abone_id = s.abone_id ?? 0,
                    ad = abone != null && !string.IsNullOrEmpty(abone.Ad) ? abone.Ad : "",
                    soyad = abone != null && !string.IsNullOrEmpty(abone.Soyad) ? abone.Soyad : "",
                    unvan = abone != null && !string.IsNullOrEmpty(abone.Unvan) ? abone.Unvan : "",
                    tckn = abone?.tckn ?? "",
                    vkn = abone?.vkn ?? "",
                    telefon = abone?.telefon ?? "",
                    e_posta = abone?.e_posta ?? "",
                    iletisim_tercihi = "Bilinmiyor",
                    sozlesme_tipi = s.sozlesme_tipi?.ToString() ?? "",
                    baslangic_tarihi = s.baslangic_tarihi ?? DateTime.Now,
                    bitis_tarihi = s.bitis_tarihi,
                    statu = s.durum?.ToString() ?? "",
                    tarife_id = s.tarife_id ?? 0,
                    tarife_grubu = (s.tarife_id ?? 0) switch
                    {
                        1 => "Mesken",
                        2 => "Sanayi",
                        3 => "Ticarethane",
                        4 => "Tarımsal Sulama",
                        5 => "Aydınlatma",
                        _ => "Bilinmiyor"
                    },
                    guvence_bedeli = s.guvence_bedeli ?? 0m,
                    created_at = s.created_at,
                    updated_at = s.updated_at,
                    tekil_kod = !string.IsNullOrWhiteSpace(s.tekil_kod) ? s.tekil_kod : tuketimNoktasi?.tekil_kod ?? "Bilinmiyor"
                };
            }).ToList();

            filtre.Sozlesmeler = viewModels;

            return View(filtre);
        }

        public IActionResult Yeni()
        {
            return View(new KcetasWeb.ViewModels.SozlesmeViewModels
            {
                sozlesme_tipi = "Bireysel"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Yeni(KcetasWeb.ViewModels.SozlesmeViewModels model)
        {
            var sozlesmeler = await _sozlesmeService.GetAllAsync();
            
            // İŞ KURALI: 1 Tüketim noktasına sadece 1 aktif sözleşme yapılabilir.
            bool aktifSozlesmeVarMi = sozlesmeler.Any(s => s.tuketim_noktasi_id == model.tuketim_noktasi_id && 
                s.durum != KcetasWeb.Models.Enums.SozlesmeDurumu.Feshedildi && s.durum != KcetasWeb.Models.Enums.SozlesmeDurumu.Pasif);

            if (aktifSozlesmeVarMi)
            {
                ModelState.AddModelError("tuketim_noktasi_id", "HATA: Bu tüketim noktası üzerinde zaten aktif veya işlem bekleyen bir sözleşme bulunmaktadır. 1 tüketim noktasına aynı anda sadece 1 sözleşme bağlanabilir.");
                await PrepareYeniSelectionTextAsync(model);
                return View(model);
            }

            int count = sozlesmeler.Count + 45; // Start from SZL-10045 if using mock generation logic
            
            var yeniSozlesme = new Sozlesme
            {
                sozlesme_id = count,
                sozlesme_no = $"SZL-{10000 + count}",
                tuketim_noktasi_id = (int)model.tuketim_noktasi_id,
                abone_id = (int)model.abone_id,
                baslangic_tarihi = DateTime.Now,
                sozlesme_tipi = model.sozlesme_tipi ?? "Bireysel",
                tarife_id = model.tarife_id,
                guvence_bedeli = model.guvence_bedeli,
                durum = KcetasWeb.Models.Enums.SozlesmeDurumu.GuvenceBekliyor,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            await _sozlesmeService.CreateAsync(yeniSozlesme);
            
            await _auditLogService.EkleAsync("Sozlesme", yeniSozlesme.sozlesme_id, "CREATE", "", yeniSozlesme.sozlesme_no, await GetCurrentUserId(), "Sisteme Yeni Sözleşme Eklendi");

            // Otomatik Yeni Bağlantı İş Emri Oluştur
            var isEmri = new IsEmri
            {
                tip = KcetasWeb.Models.Enums.IsEmriTipi.YeniBaglanti,
                durum = KcetasWeb.Models.Enums.IsEmriDurumu.Acik,
                is_emri_no = $"IE-{DateTime.Now.Year}-{(count + 1).ToString().PadLeft(4, '0')}", // Geçici mock numara üretimi (gerçek sistemde API atar)
                tuketim_noktasi_id = model.tuketim_noktasi_id,
                planlanan_tarih = DateTime.Now.AddDays(1),
                oncelik = "Normal",
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                saha_sonucu = "",
                gerekce = "",
                muhur_no = "",
                tutanak_no = "",
                status = "ACIK"
            };
            
            try
            {
                await _isEmriService.EkleAsync(isEmri);
                await _auditLogService.EkleAsync("IsEmri", 0, "CREATE", "", isEmri.is_emri_no, await GetCurrentUserId(), "Otomatik Yeni Bağlantı İş Emri Atandı");
            }
            catch
            {
                // API hatası olsa bile sözleşme oluşturulduğu için işlemi kesmiyoruz
            }

            TempData["SozlesmeMesaji"] = model.ad + " " + model.unvan + " için sözleşme başarıyla başlatıldı ve Yeni Bağlantı iş emri oluşturuldu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> TuketimNoktasiAra(string? q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
            {
                return Json(new { results = Array.Empty<object>() });
            }

            var response = await _tuketimNoktasiService.GetPagedAsync(1, 20, q.Trim());
            var results = response.Data
                .Select(t => new
                {
                    id = t.tuketim_noktasi_id,
                    text = FormatTuketimNoktasiSecim(t, t.tuketim_noktasi_id)
                })
                .ToList();

            return Json(new { results });
        }

        [HttpGet]
        public async Task<IActionResult> AboneAra(string? q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
            {
                return Json(new { results = Array.Empty<object>() });
            }

            var search = q.Trim();
            var results = new List<object>();

            if (search.Length <= 6 && !search.StartsWith("0") && int.TryParse(search, out var aboneId) && aboneId > 0)
            {
                var abone = await _aboneService.GetByIdAsync(aboneId);
                if (abone != null)
                {
                    results.Add(new { id = abone.abone_id, text = FormatAboneSecim(abone) });
                    return Json(new { results });
                }
            }

            var aboneler = await _aboneService.GetAllAsync();
            results = aboneler
                .Where(a => AboneMatchesSearch(a, search))
                .Take(20)
                .Select(a => new { id = a.abone_id, text = FormatAboneSecim(a) })
                .Cast<object>()
                .ToList();

            return Json(new { results });
        }

        public async Task<IActionResult> Detay(string id)
        {
            var item = await _sozlesmeService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var tn = await _tuketimNoktasiService.GetByIdAsync(item.tuketim_noktasi_id);
            if (tn != null)
            {
                var gecmisPaged = await _sozlesmeService.GetPagedAsync(1, 100, null, null, tn.tekil_kod);
                ViewBag.GecmisSozlesmeler = gecmisPaged.Data
                    .Where(s => s.sozlesme_id != item.sozlesme_id)
                    .OrderByDescending(s => s.baslangic_tarihi)
                    .ToList();
            }
            else
            {
                ViewBag.GecmisSozlesmeler = new List<Sozlesme>();
            }

            var viewModel = new KcetasWeb.ViewModels.SozlesmeViewModels
            {
                sozlesme_id = item.sozlesme_id,
                sozlesme_no = item.sozlesme_no,
                tuketim_noktasi_id = item.tuketim_noktasi_id,
                abone_id = item.abone_id ?? 0,
                baslangic_tarihi = item.baslangic_tarihi ?? DateTime.Now,
                bitis_tarihi = item.bitis_tarihi,
                statu = item.durum?.ToString() ?? "",
                sozlesme_tipi = item.sozlesme_tipi?.ToString() ?? "",
                tarife_id = item.tarife_id ?? 0,
                guvence_bedeli = item.guvence_bedeli ?? 0m,
                created_at = item.created_at
            };

            var abone = await _aboneService.GetByIdAsync((int)item.abone_id);
            if (abone != null)
            {
                viewModel.ad = abone.Ad;
                viewModel.soyad = abone.Soyad;
                viewModel.unvan = abone.Unvan;
                viewModel.tckn = abone.tckn;
                viewModel.vkn = abone.vkn;
                viewModel.telefon = abone.telefon;
                viewModel.e_posta = abone.e_posta;
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Duzenle(string id)
        {
            var item = await _sozlesmeService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var viewModel = new KcetasWeb.ViewModels.SozlesmeViewModels
            {
                sozlesme_id = item.sozlesme_id,
                sozlesme_no = item.sozlesme_no,
                tuketim_noktasi_id = item.tuketim_noktasi_id,
                abone_id = item.abone_id ?? 0,
                baslangic_tarihi = item.baslangic_tarihi ?? DateTime.Now,
                bitis_tarihi = item.bitis_tarihi,
                statu = item.durum?.ToString() ?? "",
                sozlesme_tipi = item.sozlesme_tipi?.ToString() ?? "",
                tarife_id = item.tarife_id ?? 0,
                guvence_bedeli = item.guvence_bedeli ?? 0m,
                created_at = item.created_at
            };

            var abone = await _aboneService.GetByIdAsync((int)item.abone_id);
            if (abone != null)
            {
                viewModel.ad = abone.Ad;
                viewModel.soyad = abone.Soyad;
                viewModel.unvan = abone.Unvan;
                viewModel.tckn = abone.tckn;
                viewModel.vkn = abone.vkn;
                viewModel.telefon = abone.telefon;
                viewModel.e_posta = abone.e_posta;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Duzenle(KcetasWeb.ViewModels.SozlesmeViewModels model)
        {
            var item = await _sozlesmeService.GetByIdAsync(model.sozlesme_no);
            if (item != null)
            {
                item.sozlesme_tipi = model.sozlesme_tipi ?? item.sozlesme_tipi;
                item.tarife_id = model.tarife_id;
                item.guvence_bedeli = model.guvence_bedeli;
                item.durum = Enum.TryParse<KcetasWeb.Models.Enums.SozlesmeDurumu>(model.statu, out var pStatu) ? pStatu : item.durum;
                item.updated_at = DateTime.Now;
                await _sozlesmeService.UpdateAsync(item);
                
                var abone = await _aboneService.GetByIdAsync((int)item.abone_id);
                if (abone != null)
                {
                    abone.Ad = model.ad;
                    abone.Soyad = model.soyad;
                    abone.Unvan = model.unvan;
                    abone.tckn = model.tckn;
                    abone.vkn = model.vkn;
                    abone.telefon = model.telefon;
                    abone.e_posta = model.e_posta;
                    await _aboneService.UpdateAsync(abone);
                }
            }
            TempData["SozlesmeMesaji"] = model.sozlesme_no + " numaralı sözleşme başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.sozlesme_no });
        }

        public async Task<IActionResult> Feshet(string id)
        {
            var item = await _sozlesmeService.GetByIdAsync(id);
            if (item != null)
            {
                item.durum = KcetasWeb.Models.Enums.SozlesmeDurumu.Feshedildi;
                item.bitis_tarihi = DateTime.Now;
                item.updated_at = DateTime.Now;

                await _sozlesmeService.UpdateAsync(item);
                TempData["SozlesmeMesaji"] = id + " numaralı sözleşme başarıyla feshedildi.";
            }
            return RedirectToAction("Index");
        }

        private async Task PrepareYeniSelectionTextAsync(KcetasWeb.ViewModels.SozlesmeViewModels model)
        {
            if (model.tuketim_noktasi_id > 0)
            {
                var tuketimNoktasi = await _tuketimNoktasiService.GetByIdAsync(model.tuketim_noktasi_id);
                ViewBag.SelectedTuketimNoktasiText = FormatTuketimNoktasiSecim(tuketimNoktasi, model.tuketim_noktasi_id);
            }

            if (model.abone_id > 0)
            {
                var abone = await _aboneService.GetByIdAsync(model.abone_id);
                if (abone != null)
                {
                    ViewBag.SelectedAboneText = FormatAboneSecim(abone);
                }
            }
        }

        private static string FormatTuketimNoktasiSecim(TuketimNoktasi? tuketimNoktasi, long fallbackId)
        {
            var kod = !string.IsNullOrWhiteSpace(tuketimNoktasi?.tekil_kod)
                ? tuketimNoktasi.tekil_kod
                : $"TN-{fallbackId}";
            var adresParcalari = new[]
            {
                tuketimNoktasi?.mahalle,
                tuketimNoktasi?.bina_no,
                tuketimNoktasi?.acik_adres
            };
            var adres = Shorten(string.Join(" ", adresParcalari.Where(x => !string.IsNullOrWhiteSpace(x))), 90);

            return string.IsNullOrWhiteSpace(adres) ? kod : $"{kod} - {adres}";
        }

        private static string FormatAboneSecim(Abone abone)
        {
            var kimlik = !string.IsNullOrWhiteSpace(abone.tckn)
                ? abone.tckn
                : !string.IsNullOrWhiteSpace(abone.vkn)
                    ? abone.vkn
                    : abone.abone_no;
            var adSoyad = string.Join(" ", new[] { abone.Ad, abone.Soyad, abone.Unvan }.Where(x => !string.IsNullOrWhiteSpace(x)));
            var aboneLabel = string.IsNullOrWhiteSpace(adSoyad) ? $"Abone {abone.abone_id}" : adSoyad;

            return string.IsNullOrWhiteSpace(kimlik)
                ? aboneLabel
                : $"{MaskKimlik(kimlik)} - {aboneLabel}";
        }

        private static bool AboneMatchesSearch(Abone abone, string search)
        {
            return abone.abone_id.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                || ContainsIgnoreCase(abone.abone_no, search)
                || ContainsIgnoreCase(abone.tckn, search)
                || ContainsIgnoreCase(abone.vkn, search)
                || ContainsIgnoreCase(abone.telefon, search)
                || ContainsIgnoreCase(abone.Ad, search)
                || ContainsIgnoreCase(abone.Soyad, search)
                || ContainsIgnoreCase(abone.Unvan, search)
                || ContainsIgnoreCase(abone.e_posta, search);
        }

        private static bool ContainsIgnoreCase(string? value, string search)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.Contains(search, StringComparison.OrdinalIgnoreCase);
        }

        private static string MaskKimlik(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 6)
            {
                return value;
            }

            return value[..3] + new string('*', value.Length - 6) + value[^3..];
        }

        private static string Shorten(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            var trimmed = value.Trim();
            return trimmed.Length <= maxLength ? trimmed : $"{trimmed[..maxLength]}...";
        }
    }
}
