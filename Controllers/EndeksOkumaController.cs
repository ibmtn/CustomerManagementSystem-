using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KcetasWeb.Controllers
{  
    [Authorize(Roles = "BTYoneticisi,SozlesmeYetkilisi, SayacOkumaPersoneli,SahaOperasyonAmir,FaturalamaUzmani,Denetci ")]
    public class EndeksOkumaController : Controller
    {
        private readonly IEndeksOkumaService _endeksOkumaService;
        private readonly ISozlesmeService _sozlesmeService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly ISayacService _sayacService;
        private readonly IFaturaService _faturaService;
        private readonly IAboneService _aboneService;
        private readonly IIsEmriService _isEmriService;

        public EndeksOkumaController(
            IEndeksOkumaService endeksOkumaService,
            ISozlesmeService sozlesmeService,
            ITuketimNoktasiService tuketimNoktasiService,
            ISayacService sayacService,
            IFaturaService faturaService,
            IAboneService aboneService,
            IIsEmriService isEmriService)
        {
            _endeksOkumaService = endeksOkumaService;
            _sozlesmeService = sozlesmeService;
            _tuketimNoktasiService = tuketimNoktasiService;
            _sayacService = sayacService;
            _faturaService = faturaService;
            _aboneService = aboneService;
            _isEmriService = isEmriService;
        }

        public async Task<IActionResult> Index(KcetasWeb.ViewModels.EndeksOkumaListeViewModel filtre)
        {
            filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
            filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;

            var response = await _endeksOkumaService.GetPagedAsync(
                filtre.CurrentPage,
                filtre.PageSize,
                filtre.FiltreKaynak,
                filtre.FiltreDurum,
                filtre.BaslangicTarih,
                filtre.BitisTarih,
                filtre.AramaMetni,
                filtre.FiltreSayacId,
                filtre.FiltreDonem,
                filtre.FiltreDogrulamaDurumu
            );

            var pagedData = response.Data;
            int totalItems = response.TotalCount;

            var sozlesmeTask = _sozlesmeService.GetAllAsync();
            var aboneTask = _aboneService.GetAllAsync();
            var isEmriTask = _isEmriService.GetAllAsync();
            var tuketimNoktasiTask = _tuketimNoktasiService.GetAllAsync();
            var sayacTask = _sayacService.GetAllAsync(); 
            
            await Task.WhenAll(sozlesmeTask, aboneTask, isEmriTask, tuketimNoktasiTask, sayacTask);

            var sozlesmeler = (await sozlesmeTask);
            var aboneler = (await aboneTask);
            var isEmirleri = (await isEmriTask);
            var tuketimNoktalari = (await tuketimNoktasiTask);
            var sayaclar = (await sayacTask);

            var viewModels = pagedData.Select(o => {
                var sozlesme = sozlesmeler.FirstOrDefault(s => s.sozlesme_id == o.sozlesme_id);

                if (sozlesme == null && o.is_emri_id.HasValue)
                {
                    var isEmri = isEmirleri.FirstOrDefault(ie => ie.is_emri_id == o.is_emri_id.Value);
                    if (isEmri != null)
                    {
                        sozlesme = sozlesmeler.FirstOrDefault(s => s.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);
                    }
                }

                if ((sozlesme == null || sozlesme.durum != KcetasWeb.Models.Enums.SozlesmeDurumu.Aktif) && o.sayac_id.HasValue)
                {
                    var sayac = sayaclar.FirstOrDefault(s => s.sayac_id == o.sayac_id.Value);
                    if (sayac != null)
                    {
                         sozlesme = sozlesmeler.FirstOrDefault(s => s.tuketim_noktasi_id == sayac.tuketim_noktasi_id);
                    }
                }
                string aboneBilgisi = "Bilinmiyor";
                if (sozlesme != null)
                {
                    var abone = aboneler.FirstOrDefault(a => a.abone_id == sozlesme.abone_id);
                    if (abone != null)
                    {
                        aboneBilgisi = $"{abone.Ad} {abone.Soyad} {abone.Unvan}".Trim();
                    }
                }
                
                return new KcetasWeb.ViewModels.EndeksOkumaListeViewModel.OkumaSatirViewModel
                {
                    OkumaId = o.okuma_id,
                    TuketimNoktasiKodu = sozlesme != null && tuketimNoktalari.Any(t => t.tuketim_noktasi_id == sozlesme.tuketim_noktasi_id) ? tuketimNoktalari.First(t => t.tuketim_noktasi_id == sozlesme.tuketim_noktasi_id).tekil_kod : $"TN-{o.sozlesme_id}",
                    SayacSeriNo = o.sayac_id.HasValue && sayaclar.Any(s => s.sayac_id == o.sayac_id.Value) ? sayaclar.First(s => s.sayac_id == o.sayac_id.Value).seri_no : $"SAYAC-{o.sayac_id}",
                    OkumaTarihi = o.okuma_zamani ?? DateTime.Now,
                    OncekiEndeks = o.onceki_endeks ?? 0,
                    GuncelEndeks = o.yeni_endeks ?? 0,
                    TuketimMiktari = (o.yeni_endeks ?? 0) - (o.onceki_endeks ?? 0),
                    Kaynak = o.okuma_kaynagi,
                    Durum = o.dogrulama_durumu?.ToString() ?? "BEKLIYOR",
                    DurumRenk = o.dogrulama_durumu == KcetasWeb.Models.Enums.DogrulamaDurumu.Onaylandi ? "success" : "warning",
                    DogrulamaDurumu = o.dogrulama_durumu == KcetasWeb.Models.Enums.DogrulamaDurumu.Onaylandi,
                    AnomaliAciklamasi = o.anomali_mi ? "Tüketim yüksek/düşük" : "",
                    TarifeGrubu = "Mesken",
                    AboneBilgisi = aboneBilgisi,
                    OkumaTipi = o.okuma_tipi
                };
            }).ToList();

            viewModels = viewModels
                .OrderBy(x => x.DogrulamaDurumu ? 1 : 0)
                .ThenByDescending(x => x.OkumaTarihi)
                .ToList();

            ViewBag.Istatistikler = await _endeksOkumaService.GetIstatistiklerAsync(filtre.FiltreDonem);

            filtre.TotalItems = totalItems;
            filtre.Okumalar = viewModels;

            return View(filtre);
        }

        public async Task<IActionResult> Detay(long id)
        {
            var okuma = await _endeksOkumaService.GetByIdAsync((int)id);
            if (okuma == null) return NotFound();

            var isEmirleri = await _isEmriService.GetAllAsync();
            var isEmri = okuma.is_emri_id.HasValue ? isEmirleri.FirstOrDefault(ie => ie.is_emri_id == okuma.is_emri_id.Value) : null;
            
            var sayaclar = await _sayacService.GetAllAsync();
            var sayac = okuma.sayac_id.HasValue ? sayaclar.FirstOrDefault(s => s.sayac_id == okuma.sayac_id.Value) : null;
            
            var sozlesmeler = await _sozlesmeService.GetAllAsync();
            var sozlesme = okuma.sozlesme_id.HasValue ? sozlesmeler.FirstOrDefault(s => s.sozlesme_id == okuma.sozlesme_id.Value) : null;

            var viewModel = new KcetasWeb.ViewModels.EndeksOkumaViewModels
            {
                okuma_id = okuma.okuma_id,
                sayac_id = okuma.sayac_id,
                is_emri_id = okuma.is_emri_id,
                IsEmriNo = isEmri != null ? isEmri.is_emri_no : "-",
                seri_no = sayac != null ? sayac.seri_no : "-",
                sozlesme_no = sozlesme != null && !string.IsNullOrEmpty(sozlesme.sozlesme_no) ? sozlesme.sozlesme_no : (okuma.sozlesme_id?.ToString() ?? "-"),
                sozlesme_id = okuma.sozlesme_id,
                okuma_tipi = okuma.okuma_tipi,
                okuma_kaynagi = okuma.okuma_kaynagi,
                onceki_endeks = okuma.onceki_endeks,
                yeni_endeks = okuma.yeni_endeks,
                okuma_zamani = okuma.okuma_zamani,
                kullanici_id = okuma.kullanici_id,
                okunamam_nedeni = okuma.okunamama_nedeni,
                dogrulama_durumu = okuma.dogrulama_durumu,
                anomali_mi = okuma.anomali_mi,
                status = okuma.status,
                CreatedAt = okuma.created_at
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> TutanakYazdir(long id)
        {
            var okuma = await _endeksOkumaService.GetByIdAsync((int)id);
            if (okuma == null) return NotFound();
            return View(okuma);
        }

        public async Task<IActionResult> Yeni()
        {
            var tnTask = _tuketimNoktasiService.GetAllAsync();
            var sycTask = _sayacService.GetAllAsync();
            var szlTask = _sozlesmeService.GetAllAsync();
            await Task.WhenAll(tnTask, sycTask, szlTask);
            ViewBag.TuketimNoktalari = (await tnTask);
            ViewBag.Sayaclar = (await sycTask);
            ViewBag.Sozlesmeler = (await szlTask);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Yeni(long TuketimNoktasiId, long SayacId, string onceki_endeks, string yeni_endeks, string okuma_tipi, string okuma_kaynagi, string aciklama)
        {
            // Nokta/virgül hatasını önlemek için string olarak alıp güvenli dönüştürüyoruz
            decimal parsedOnceki = 0;
            decimal parsedYeni = 0;
            
            if (!string.IsNullOrEmpty(onceki_endeks))
            {
                onceki_endeks = onceki_endeks.Replace(",", ".");
                decimal.TryParse(onceki_endeks, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedOnceki);
            }
            if (!string.IsNullOrEmpty(yeni_endeks))
            {
                yeni_endeks = yeni_endeks.Replace(",", ".");
                decimal.TryParse(yeni_endeks, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedYeni);
            }

            // Tüketim miktarını hesapla
            decimal tuketim = parsedYeni - parsedOnceki;
            if (tuketim < 0) tuketim = 0; // Eğer negatifse (örneğin hatalı okuma veya sayaç sıfırlanması), şimdilik 0 kabul edelim

            // İlgili tüketim noktasına ait sözleşmeyi bul
            var sozlesmeler = (await _sozlesmeService.GetAllAsync()).Where(s => s.tuketim_noktasi_id == TuketimNoktasiId).ToList();
            var aktifSozlesme = sozlesmeler.FirstOrDefault(s => s.durum != KcetasWeb.Models.Enums.SozlesmeDurumu.Feshedildi && s.durum != KcetasWeb.Models.Enums.SozlesmeDurumu.Pasif) ?? sozlesmeler.FirstOrDefault();
            
            string tarifeGrubu = aktifSozlesme != null ? 
                (aktifSozlesme.tarife_id == 1 ? "Mesken" : 
                 aktifSozlesme.tarife_id == 2 ? "Sanayi" : 
                 aktifSozlesme.tarife_id == 3 ? "Ticarethane" : 
                 aktifSozlesme.tarife_id == 4 ? "Tarımsal Sulama" : "Aydınlatma") : "Mesken";
            
            // Fatura hesaplamasını yap
            var hesaplama = await _faturaService.SimulasyonHesaplaAsync(tarifeGrubu, tuketim);

            KcetasWeb.Models.Enums.OkumaTipi parsedOkumaTipi = KcetasWeb.Models.Enums.OkumaTipi.RutinDonem;
            if (int.TryParse(okuma_tipi, out int tipId))
            {
                parsedOkumaTipi = (KcetasWeb.Models.Enums.OkumaTipi)tipId;
            }

            KcetasWeb.Models.Enums.OkumaKaynagi parsedOkumaKaynagi = KcetasWeb.Models.Enums.OkumaKaynagi.Manuel;
            if (int.TryParse(okuma_kaynagi, out int kaynakId))
            {
                parsedOkumaKaynagi = (KcetasWeb.Models.Enums.OkumaKaynagi)kaynakId;
            }

            var yeniOkuma = new EndeksOkuma
            {
                sayac_id = (int)SayacId,
                sozlesme_id = (aktifSozlesme != null && aktifSozlesme.sozlesme_id > 0) ? (int)aktifSozlesme.sozlesme_id : null,
                donem = DateTime.Now.ToString("yyyy-MM"),
                okuma_tipi = parsedOkumaTipi,
                okuma_kaynagi = parsedOkumaKaynagi,
                onceki_endeks = parsedOnceki,
                yeni_endeks = parsedYeni,
                okuma_zamani = DateTime.UtcNow,
                kullanici_id = 1,
                dogrulama_durumu = KcetasWeb.Models.Enums.DogrulamaDurumu.DogrulamaBekliyor,
                anomali_mi = tuketim > 1000,
                status = "AKTIF",
                okunamama_nedeni = "",
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            string apiHataMesaji = "";

            try
            {
                await _endeksOkumaService.CreateAsync(yeniOkuma);
            }
            catch (Exception ex)
            {
                apiHataMesaji += $"Okuma API Hatası: {ex.Message} | ";
            }

            var tn = (await _tuketimNoktasiService.GetAllAsync()).FirstOrDefault(t => t.tuketim_noktasi_id == TuketimNoktasiId);

            var yeniFatura = new Fatura
            {
                fatura_no = $"FAT-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                sozlesme_id = aktifSozlesme?.sozlesme_id ?? 1000,
                tekil_kod = tn != null ? tn.tekil_kod : TuketimNoktasiId.ToString(),
                fatura_tipi = KcetasWeb.Models.Enums.FaturaTipi.Donem,
                fatura_tarihi = DateOnly.FromDateTime(DateTime.Now),
                son_odeme_tarihi = DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                donem = DateTime.Now.ToString("yyyy-MM"),
                ilk_endeks = parsedOnceki,
                son_endeks = parsedYeni,
                tuketim_kwh = tuketim,
                carpan = 1,
                enerji_bedeli = hesaplama.EnerjiBedeli,
                dagatim_bedeli = hesaplama.DagitimBedeli,
                vergi_fon_toplam = hesaplama.TrtPayi + hesaplama.EnerjiFonu + hesaplama.KdvTutari,
                toplam_tutar = hesaplama.ToplamTutar,
                reaktif_enduktif = 0m,
                reaktif_kapasitif = 0m,
                hizmet_bedeli = 0m,
                kesme_baglama_bedeli = 0m,
                durum = "HESAPLANDI",
                status = "AKTIF",
                created_at = DateTime.Now,
                kullanici_id = 1
            };

            try
            {
                await _faturaService.EkleAsync(yeniFatura);
            }
            catch (Exception ex)
            {
                apiHataMesaji += $"Fatura API Hatası: {ex.Message}";
            }

            if (!string.IsNullOrEmpty(apiHataMesaji))
            {
                TempData["OkumaMesaji"] = "Kayıt sırasında hata oluştu: " + apiHataMesaji;
            }
            else
            {
                TempData["OkumaMesaji"] = "Endeks okuması alındı ve otomatik olarak yeni fatura oluşturuldu. (Fatura No: " + yeniFatura.fatura_no + " - Tutar: " + yeniFatura.toplam_tutar?.ToString("C2") + ")";
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetSonEndeks(long sayacId)
        {
            var okumalar = (await _endeksOkumaService.GetAllAsync()).Where(x => x.sayac_id == sayacId)
                .OrderByDescending(x => x.okuma_zamani)
                .ToList();

            if (okumalar.Any())
            {
                // En son okunan yeni endeks, sıradaki okumanın "önceki_endeksi" olur.
                return Json(new { basarili = true, endeks = okumalar.First().yeni_endeks });
            }

            return Json(new { basarili = false, endeks = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> OnaylaVeFaturalandir(long id)
        {
            var okuma = await _endeksOkumaService.GetByIdAsync((int)id);
            if (okuma == null || okuma.dogrulama_durumu == KcetasWeb.Models.Enums.DogrulamaDurumu.Onaylandi)
                return RedirectToAction("Index");

            // 1. Okumayı Onayla
            okuma.dogrulama_durumu = KcetasWeb.Models.Enums.DogrulamaDurumu.Onaylandi;
            okuma.updated_at = DateTime.UtcNow;
            try { await _endeksOkumaService.UpdateAsync(okuma); } catch { }

            // 2. Fatura Oluştur
            decimal tuketim = (okuma.yeni_endeks ?? 0) - (okuma.onceki_endeks ?? 0);
            if (tuketim < 0) tuketim = 0;

            var sozlesmeler = (await _sozlesmeService.GetAllAsync()).Where(s => s.sozlesme_id == okuma.sozlesme_id).ToList();
            var aktifSozlesme = sozlesmeler.FirstOrDefault();
            
            string tarifeGrubu = aktifSozlesme != null ? 
                (aktifSozlesme.tarife_id == 1 ? "Mesken" : 
                 aktifSozlesme.tarife_id == 2 ? "Sanayi" : 
                 aktifSozlesme.tarife_id == 3 ? "Ticarethane" : 
                 aktifSozlesme.tarife_id == 4 ? "Tarımsal Sulama" : "Aydınlatma") : "Mesken";
            
            var hesaplama = await _faturaService.SimulasyonHesaplaAsync(tarifeGrubu, tuketim);

            var tn = aktifSozlesme != null ? (await _tuketimNoktasiService.GetAllAsync()).FirstOrDefault(t => t.tuketim_noktasi_id == aktifSozlesme.tuketim_noktasi_id) : null;

            var yeniFatura = new Fatura
            {
                fatura_no = $"FAT-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                sozlesme_id = okuma.sozlesme_id ?? 1000,
                tekil_kod = tn != null ? tn.tekil_kod : "BILINMIYOR",
                fatura_tipi = KcetasWeb.Models.Enums.FaturaTipi.Donem,
                fatura_tarihi = DateOnly.FromDateTime(DateTime.Now),
                son_odeme_tarihi = DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                donem = okuma.donem ?? DateTime.Now.ToString("yyyy-MM"),
                ilk_endeks = okuma.onceki_endeks,
                son_endeks = okuma.yeni_endeks,
                tuketim_kwh = tuketim,
                carpan = 1,
                enerji_bedeli = hesaplama.EnerjiBedeli,
                dagatim_bedeli = hesaplama.DagitimBedeli,
                vergi_fon_toplam = hesaplama.TrtPayi + hesaplama.EnerjiFonu + hesaplama.KdvTutari,
                toplam_tutar = hesaplama.ToplamTutar,
                reaktif_enduktif = 0m,
                reaktif_kapasitif = 0m,
                hizmet_bedeli = 0m,
                kesme_baglama_bedeli = 0m,
                durum = "HESAPLANDI",
                status = "AKTIF",
                created_at = DateTime.Now,
                kullanici_id = 1
            };

            try
            {
                await _faturaService.EkleAsync(yeniFatura);
                TempData["OkumaMesaji"] = $"Endeks okuması başarıyla onaylandı ve yeni fatura oluşturuldu. (Fatura No: {yeniFatura.fatura_no} - Tutar: {yeniFatura.toplam_tutar?.ToString("C2")})";

                // YENİ İŞ MANTIĞI: Eğer bu okuma bir İLK OKUMA ise ve fatura kesildiyse, ENERJİ AÇMA iş emri atılsın!
                if (okuma.okuma_tipi == KcetasWeb.Models.Enums.OkumaTipi.IlkOkuma)
                {
                    var acmaIsEmri = new KcetasWeb.Models.IsEmri
                    {
                        is_emri_no = $"IE-ACM-{DateTime.Now.ToString("yyyyMMdd")}-{new Random().Next(1000, 9999)}",
                        tuketim_noktasi_id = aktifSozlesme?.tuketim_noktasi_id ?? 0,
                        sayac_id = okuma.sayac_id,
                        tip = KcetasWeb.Models.Enums.IsEmriTipi.EnerjiAcma,
                        durum = KcetasWeb.Models.Enums.IsEmriDurumu.Acik,
                        oncelik = "YUKSEK",
                        planlanan_tarih = DateTime.Now.AddDays(1),
                        atanan_kullanici_id = null,
                        status = "AKTIF",
                        created_at = DateTime.Now
                    };
                    try 
                    { 
                        await _isEmriService.EkleAsync(acmaIsEmri); 
                        TempData["OkumaMesaji"] += " Ayrıca onaylanan ilk okuma sonrası sisteme otomatik 'Enerji Açma' iş emri eklendi.";
                    } 
                    catch { }
                }
            }
            catch (Exception ex)
            {
                TempData["OkumaMesaji"] = $"Okuma onaylandı ancak fatura kesilirken hata oluştu: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}



