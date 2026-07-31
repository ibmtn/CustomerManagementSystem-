using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
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


            DateTime? baslangic = filtre.FiltreOkumaTarihi;
            DateTime? bitis = filtre.FiltreOkumaTarihi.HasValue ? filtre.FiltreOkumaTarihi.Value.Date.AddDays(1).AddTicks(-1) : null;

            var response = await _endeksOkumaService.GetPagedAsync(
                filtre.CurrentPage,
                filtre.PageSize,
                filtre.FiltreKaynak,
                filtre.FiltreDurum,
                baslangic,
                bitis,
                filtre.AramaMetni,
                filtre.FiltreSayacId,
                filtre.FiltreDonem,
                filtre.FiltreDogrulamaDurumu,
                filtre.FiltreTuketimNoktasi,
                filtre.FiltreAbone,
                filtre.FiltreOkumaNo
            );

            var pagedData = response.Data;
            filtre.TotalItems = response.TotalCount;

            // Fetch relations only for the paginated items to avoid OutOfMemory
            var viewModels = new List<KcetasWeb.ViewModels.EndeksOkumaListeViewModel.OkumaSatirViewModel>();

            foreach (var o in pagedData)
            {
                KcetasWeb.Models.Sozlesme? sozlesme = null;
                if (o.sozlesme_id.HasValue)
                {
                    sozlesme = await _sozlesmeService.GetByIdAsync(o.sozlesme_id.Value);
                }

                if (sozlesme == null && o.is_emri_id.HasValue)
                {
                    var isEmri = await _isEmriService.GetByIdAsync(o.is_emri_id.Value);
                    if (isEmri != null)
                    {
                        var sozlesmelerPaged = await _sozlesmeService.GetPagedAsync(1, 1, null, null, null, null); // wait we don't have GetByTuketimNoktasiId for Sozlesme directly by id without tekil_kod. Actually let's use GetByIdAsync if we can. Wait, we can't easily fetch Sozlesme by tuketim_noktasi_id here without GetAll. So we'll skip complex fallbacks or just do a quick loop on a small list? No, we shouldn't.
                    }
                }
                
                // Let's simplify and use what we have:
                string tuketimNoktasiKodu = $"TN-{o.sozlesme_id}";
                if (sozlesme != null)
                {
                    var tn = await _tuketimNoktasiService.GetByIdAsync(sozlesme.tuketim_noktasi_id);
                    if (tn != null) tuketimNoktasiKodu = tn.tekil_kod;
                }

                string sayacSeriNo = $"SAYAC-{o.sayac_id}";
                if (o.sayac_id.HasValue)
                {
                    var sayac = await _sayacService.GetByIdAsync(o.sayac_id.Value);
                    if (sayac != null) sayacSeriNo = sayac.seri_no;
                }

                string aboneBilgisi = "Bilinmiyor";
                if (sozlesme != null && sozlesme.abone_id > 0)
                {
                    var abone = await _aboneService.GetByIdAsync((int)sozlesme.abone_id);
                    if (abone != null)
                    {
                        aboneBilgisi = $"{abone.Ad} {abone.Soyad} {abone.Unvan}".Trim();
                    }
                }

                viewModels.Add(new KcetasWeb.ViewModels.EndeksOkumaListeViewModel.OkumaSatirViewModel
                {
                    OkumaId = o.okuma_id,
                    TuketimNoktasiKodu = tuketimNoktasiKodu,
                    SayacSeriNo = sayacSeriNo,
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
                });
            }

            viewModels = viewModels
                .OrderBy(x => x.DogrulamaDurumu ? 1 : 0)
                .ThenByDescending(x => x.OkumaTarihi)
                .ToList();

            ViewBag.Istatistikler = await _endeksOkumaService.GetIstatistiklerAsync(filtre.FiltreDonem);

            filtre.Okumalar = viewModels;

            return View(filtre);
        }

        public async Task<IActionResult> Detay(long id)
        {
            var okuma = await _endeksOkumaService.GetByIdAsync((int)id);
            if (okuma == null) return NotFound();

            var isEmri = okuma.is_emri_id.HasValue ? await _isEmriService.GetByIdAsync(okuma.is_emri_id.Value) : null;
            var sayac = okuma.sayac_id.HasValue ? await _sayacService.GetByIdAsync(okuma.sayac_id.Value) : null;
            var sozlesme = okuma.sozlesme_id.HasValue ? await _sozlesmeService.GetByIdAsync(okuma.sozlesme_id.Value) : null;

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

        public IActionResult Yeni()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SozlesmeAra(string? q, int page = 1)
        {
            int pageSize = 20;
            var secimler = await _endeksOkumaService.YeniOkumaSecimAraAsync(q, page, pageSize);

            var results = secimler
                .Select(s => new
                {
                    id = s.SozlesmeId,
                    text = $"{(!string.IsNullOrWhiteSpace(s.SozlesmeNo) ? s.SozlesmeNo : $"Sözleşme {s.SozlesmeId}")} - {(!string.IsNullOrWhiteSpace(s.TekilKod) ? s.TekilKod : $"TN: {s.TuketimNoktasiId}")} - Sayaç: {s.SayacSeriNo}",
                    basarili = true,
                    sozlesmeId = s.SozlesmeId,
                    sozlesmeNo = s.SozlesmeNo,
                    tuketimNoktasiId = s.TuketimNoktasiId,
                    tuketimNoktasiKodu = s.TekilKod,
                    adres = s.Adres,
                    sayacId = s.SayacId,
                    sayacSeriNo = s.SayacSeriNo,
                    sayacMarkaModel = "",
                    sonEndeksBasarili = s.SonEndeks.HasValue,
                    sonEndeks = s.SonEndeks ?? 0m,
                    donem = s.Donem
                })
                .ToList();

            return Json(new 
            { 
                results = results,
                pagination = new { more = secimler.Count == pageSize }
            });
        }

        [HttpGet]
        public async Task<IActionResult> SozlesmeSecimBilgisi(long sozlesmeId)
        {
            var sozlesme = await _sozlesmeService.GetByIdAsync(sozlesmeId);
            if (sozlesme == null)
            {
                return Json(new { basarili = false, mesaj = "Sözleşme bulunamadı." });
            }

            var tuketimNoktasiTask = _tuketimNoktasiService.GetByIdAsync(sozlesme.tuketim_noktasi_id);
            var sayacTask = _sayacService.GetByTuketimNoktasiIdAsync(sozlesme.tuketim_noktasi_id);
            await Task.WhenAll(tuketimNoktasiTask, sayacTask);

            var tuketimNoktasi = await tuketimNoktasiTask;
            var sayac = await sayacTask;

            if (sayac == null)
            {
                return Json(new
                {
                    basarili = false,
                    mesaj = "Bu sözleşmenin tüketim noktasına bağlı aktif sayaç bulunamadı.",
                    tuketimNoktasiId = sozlesme.tuketim_noktasi_id,
                    tuketimNoktasiKodu = tuketimNoktasi?.tekil_kod ?? sozlesme.tuketim_noktasi_id.ToString()
                });
            }

            var sonEndeks = await GetSonEndeksForSayacAsync(sayac.sayac_id, sayac.seri_no);

            return Json(new
            {
                basarili = true,
                sozlesmeId = sozlesme.sozlesme_id,
                sozlesmeNo = sozlesme.sozlesme_no,
                tuketimNoktasiId = sozlesme.tuketim_noktasi_id,
                tuketimNoktasiKodu = tuketimNoktasi?.tekil_kod ?? sozlesme.tuketim_noktasi_id.ToString(),
                adres = tuketimNoktasi?.acik_adres,
                sayacId = sayac.sayac_id,
                sayacSeriNo = sayac.seri_no,
                sayacMarkaModel = $"{sayac.marka} {sayac.model}".Trim(),
                sonEndeksBasarili = sonEndeks.Basarili,
                sonEndeks = sonEndeks.Endeks
            });
        }

        [HttpPost]
        public async Task<IActionResult> Yeni(long SozlesmeId, long TuketimNoktasiId, long SayacId, string SayacSeriNo, string onceki_endeks, string yeni_endeks, string okuma_tipi, string okuma_kaynagi, string aciklama)
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

            var aktifSozlesme = SozlesmeId > 0 ? await _sozlesmeService.GetByIdAsync(SozlesmeId) : null;
            if (aktifSozlesme == null || aktifSozlesme.tuketim_noktasi_id != TuketimNoktasiId)
            {
                var sozlesmeler = (await _sozlesmeService.GetAllAsync()).Where(s => s.tuketim_noktasi_id == TuketimNoktasiId).ToList();
                aktifSozlesme = sozlesmeler.FirstOrDefault(s => s.durum != KcetasWeb.Models.Enums.SozlesmeDurumu.Feshedildi && s.durum != KcetasWeb.Models.Enums.SozlesmeDurumu.Pasif) ?? sozlesmeler.FirstOrDefault();
            }

            if (aktifSozlesme == null)
            {
                TempData["OkumaMesaji"] = "HATA: Seçilen tüketim noktasına ait bir sözleşme bulunamadı! Endeks okuması ve faturalandırma yapılabilmesi için öncelikle bir sözleşme oluşturmalısınız.";
                TempData["OkumaMesajTip"] = "danger";
                return RedirectToAction("Yeni");
            }

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
                bool okumaZatenVar = await OkumaDonemKaydiVarAsync(SayacId, SayacSeriNo, yeniOkuma.donem);

                if (okumaZatenVar)
                {
                    TempData["OkumaMesaji"] = $"HATA: Bu sayaç için {yeniOkuma.donem} döneminde zaten bir endeks okuması kaydı bulunmaktadır. Aynı döneme birden fazla okuma girilemez.";
                    TempData["OkumaMesajTip"] = "danger";
                    return RedirectToAction("Yeni");
                }

                await _endeksOkumaService.CreateAsync(yeniOkuma);
            }
            catch (Exception ex)
            {
                // BAZI DURUMLARDA BACKEND API KAYDI BAŞARIYLA YAPIYOR ANCAK İKİNCİL BİR İŞLEMDE 
                // HATA VERİP 500 DÖNDÜRÜYOR. BU YÜZDEN KAYDİ GERÇEKTEN YAPIP YAPMADIĞINI KONTROL EDELİM.
                bool gercektenKaydetti = await OkumaDonemKaydiVarAsync(SayacId, SayacSeriNo, yeniOkuma.donem);

                if (!gercektenKaydetti)
                {
                    apiHataMesaji += $"Okuma API Hatası: {ex.Message} | ";
                }
            }

            if (!string.IsNullOrEmpty(apiHataMesaji))
            {
                TempData["OkumaMesaji"] = "Kayıt sırasında hata oluştu: " + apiHataMesaji;
                TempData["OkumaMesajTip"] = "danger";
            }
            else
            {
                TempData["OkumaMesaji"] = "Endeks okuması alındı ve onay bekliyor. Fatura oluşturmak için listeden onaylayınız.";
                TempData["OkumaMesajTip"] = "success";
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetSonEndeks(long sayacId, string? seriNo)
        {
            var sonEndeks = await GetSonEndeksForSayacAsync(sayacId, seriNo);

            if (sonEndeks.Basarili)
            {
                return Json(new { basarili = true, endeks = sonEndeks.Endeks });
            }

            return Json(new { basarili = false, endeks = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> OnaylaVeFaturalandir(long id)
        {
            var okuma = await _endeksOkumaService.GetByIdAsync((int)id);
            if (okuma == null)
            {
                TempData["OkumaMesaji"] = "HATA: Onaylanacak endeks okuması bulunamadı.";
                TempData["OkumaMesajTip"] = "danger";
                return RedirectToAction("Index");
            }

            if (okuma.dogrulama_durumu == KcetasWeb.Models.Enums.DogrulamaDurumu.Onaylandi)
            {
                TempData["OkumaMesaji"] = "Bu endeks okuması zaten onaylanmış.";
                TempData["OkumaMesajTip"] = "success";
                return RedirectToAction("Index");
            }

            decimal tuketim = (okuma.yeni_endeks ?? 0) - (okuma.onceki_endeks ?? 0);
            if (tuketim < 0) tuketim = 0;

            var aktifSozlesme = await ResolveSozlesmeForOkumaAsync(okuma);
            if (aktifSozlesme == null)
            {
                TempData["OkumaMesaji"] = "HATA: Bu okuma için bağlı sözleşme bulunamadı. Fatura oluşturulamadı.";
                TempData["OkumaMesajTip"] = "danger";
                return RedirectToAction("Index");
            }
            
            string tarifeGrubu = GetTarifeGrubu(aktifSozlesme.tarife_id);
            var hesaplama = await _faturaService.SimulasyonHesaplaAsync(tarifeGrubu, tuketim);
            var tn = await _tuketimNoktasiService.GetByIdAsync(aktifSozlesme.tuketim_noktasi_id);
            var donem = okuma.donem ?? DateTime.Now.ToString("yyyy-MM");
            var mevcutFaturalar = (await _faturaService.GetPagedAsync(1, 100, sozlesmeId: aktifSozlesme.sozlesme_id)).Data ?? new List<Fatura>();
            var mevcutOkumaFaturasi = mevcutFaturalar.FirstOrDefault(f => f.okuma_id == okuma.okuma_id)
                ?? mevcutFaturalar.FirstOrDefault(f =>
                    f.donem == donem &&
                    f.sozlesme_id == aktifSozlesme.sozlesme_id &&
                    SameAmount(f.ilk_endeks, okuma.onceki_endeks) &&
                    SameAmount(f.son_endeks, okuma.yeni_endeks));

            var faturaNo = mevcutOkumaFaturasi?.fatura_no;
            var faturaToplam = mevcutOkumaFaturasi?.toplam_tutar;

            var yeniFatura = new Fatura
            {
                fatura_no = $"FAT-{DateTime.Now:yyyyMMddHHmmss}-{okuma.okuma_id}",
                sozlesme_id = aktifSozlesme.sozlesme_id,
                tekil_kod = tn != null ? tn.tekil_kod : aktifSozlesme.tuketim_noktasi_id.ToString(),
                fatura_tipi = KcetasWeb.Models.Enums.FaturaTipi.Donem,
                fatura_tarihi = DateOnly.FromDateTime(DateTime.Now),
                son_odeme_tarihi = DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                donem = donem,
                okuma_id = okuma.okuma_id,
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
                durum = "HESAPLANDI", // Fatura API bu string'i otomatik olarak enum (2) yapacak
                status = "AKTIF",
                created_at = DateTime.Now,
                kullanici_id = 1
            };

            try
            {
                if (mevcutOkumaFaturasi == null)
                {
                    var olusanFatura = await _faturaService.EkleAsync(yeniFatura);
                    faturaNo = !string.IsNullOrWhiteSpace(olusanFatura.fatura_no)
                        ? olusanFatura.fatura_no
                        : yeniFatura.fatura_no;
                    faturaToplam = olusanFatura.toplam_tutar ?? yeniFatura.toplam_tutar;
                }

                okuma.dogrulama_durumu = KcetasWeb.Models.Enums.DogrulamaDurumu.Onaylandi;
                okuma.updated_at = DateTime.UtcNow;
                await _endeksOkumaService.UpdateAsync(okuma);

                TempData["OkumaMesaji"] = mevcutOkumaFaturasi == null
                    ? $"Endeks okuması başarıyla onaylandı ve yeni fatura oluşturuldu. (Fatura No: {faturaNo} - Tutar: {faturaToplam?.ToString("C2")})"
                    : $"Endeks okuması başarıyla onaylandı. Bu okuma için daha önce oluşturulmuş fatura kullanılacak. (Fatura No: {faturaNo})";
                TempData["OkumaMesajTip"] = "success";
            }
            catch (Exception ex)
            {
                TempData["OkumaMesaji"] = $"HATA: Okuma onaylama ve fatura oluşturma sırasında hata oluştu: {ex.Message}";
                TempData["OkumaMesajTip"] = "danger";
                return RedirectToAction("Index");
            }

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
                    catch (Exception ex) 
                    {
                        TempData["OkumaMesaji"] += $" Ancak otomatik 'Enerji Açma' iş emri oluşturulurken bir hata oluştu: {ex.Message}";
                    }
                }


            return RedirectToAction("Index");
        }

        private static IEnumerable<EndeksOkuma> ApplyLocalEndeksFilters(
            IEnumerable<EndeksOkuma> okumalar,
            KcetasWeb.ViewModels.EndeksOkumaListeViewModel filtre,
            List<Sozlesme> sozlesmeler,
            List<TuketimNoktasi> tuketimNoktalari,
            List<Sayac> sayaclar,
            List<IsEmri> isEmirleri,
            List<Abone> aboneler)
        {
            var query = okumalar.AsEnumerable();

            if (filtre.FiltreOkumaTarihi.HasValue)
            {
                var targetDate = filtre.FiltreOkumaTarihi.Value.Date;
                query = query.Where(o => o.okuma_zamani.HasValue && o.okuma_zamani.Value.Date == targetDate);
            }

            if (!string.IsNullOrWhiteSpace(filtre.FiltreAbone))
            {
                var aboneSearch = NormalizeForSearch(filtre.FiltreAbone);
                var eslesenAboneIdleri = aboneler
                    .Where(a => NormalizeForSearch(a.Ad).Contains(aboneSearch, StringComparison.OrdinalIgnoreCase) ||
                                NormalizeForSearch(a.Soyad).Contains(aboneSearch, StringComparison.OrdinalIgnoreCase) ||
                                NormalizeForSearch(a.Unvan).Contains(aboneSearch, StringComparison.OrdinalIgnoreCase) ||
                                NormalizeForSearch(a.TcKimlikNo).Contains(aboneSearch, StringComparison.OrdinalIgnoreCase) ||
                                NormalizeForSearch(a.VergiNo).Contains(aboneSearch, StringComparison.OrdinalIgnoreCase))
                    .Select(a => a.abone_id)
                    .ToHashSet();

                var eslesenSozlesmeIdleri = sozlesmeler
                    .Where(s => s.abone_id > 0 && eslesenAboneIdleri.Contains((int)s.abone_id))
                    .Select(s => s.sozlesme_id)
                    .ToHashSet();

                query = query.Where(o => o.sozlesme_id.HasValue && eslesenSozlesmeIdleri.Contains(o.sozlesme_id.Value));
            }

            if (!string.IsNullOrWhiteSpace(filtre.FiltreDonem))
            {
                query = query.Where(o => string.Equals(o.donem, filtre.FiltreDonem.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (TryParseDogrulamaDurumu(filtre.FiltreDogrulamaDurumu, out var dogrulamaDurumu))
            {
                query = query.Where(o => o.dogrulama_durumu == dogrulamaDurumu);
            }

            if (!string.IsNullOrWhiteSpace(filtre.FiltreSayacId))
            {
                var sayacSearch = NormalizeForSearch(filtre.FiltreSayacId);
                query = query.Where(o =>
                    o.sayac_id.HasValue &&
                    (o.sayac_id.Value.ToString().Contains(sayacSearch, StringComparison.OrdinalIgnoreCase) ||
                     sayaclar.Any(s => s.sayac_id == o.sayac_id.Value && NormalizeForSearch(s.seri_no).Contains(sayacSearch, StringComparison.OrdinalIgnoreCase))));
            }

            if (!string.IsNullOrWhiteSpace(filtre.FiltreTuketimNoktasi))
            {
                var tuketimSearch = NormalizeForSearch(filtre.FiltreTuketimNoktasi);
                var eslesenTuketimNoktasiIdleri = tuketimNoktalari
                    .Where(t => NormalizeForSearch(t.tekil_kod).Contains(tuketimSearch, StringComparison.OrdinalIgnoreCase) ||
                                NormalizeForSearch(t.acik_adres).Contains(tuketimSearch, StringComparison.OrdinalIgnoreCase) ||
                                NormalizeForSearch(t.mahalle).Contains(tuketimSearch, StringComparison.OrdinalIgnoreCase))
                    .Select(t => t.tuketim_noktasi_id)
                    .ToHashSet();

                var eslesenSozlesmeIdleri = sozlesmeler
                    .Where(s => eslesenTuketimNoktasiIdleri.Contains(s.tuketim_noktasi_id))
                    .Select(s => s.sozlesme_id)
                    .ToHashSet();

                query = query.Where(o => EndeksOkumaMatchesTuketimNoktasi(o, eslesenTuketimNoktasiIdleri, eslesenSozlesmeIdleri, sayaclar, isEmirleri));
            }

            return query;
        }

        private static bool EndeksOkumaMatchesTuketimNoktasi(
            EndeksOkuma okuma,
            HashSet<int> tuketimNoktasiIdleri,
            HashSet<int> sozlesmeIdleri,
            List<Sayac> sayaclar,
            List<IsEmri> isEmirleri)
        {
            if (okuma.sozlesme_id.HasValue && sozlesmeIdleri.Contains(okuma.sozlesme_id.Value))
            {
                return true;
            }

            if (okuma.is_emri_id.HasValue)
            {
                var isEmri = isEmirleri.FirstOrDefault(ie => ie.is_emri_id == okuma.is_emri_id.Value);
                if (isEmri != null && tuketimNoktasiIdleri.Contains(isEmri.tuketim_noktasi_id))
                {
                    return true;
                }
            }

            if (okuma.sayac_id.HasValue)
            {
                var sayac = sayaclar.FirstOrDefault(s => s.sayac_id == okuma.sayac_id.Value);
                if (sayac?.tuketim_noktasi_id != null && tuketimNoktasiIdleri.Contains(sayac.tuketim_noktasi_id.Value))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<Sozlesme?> ResolveSozlesmeForOkumaAsync(EndeksOkuma okuma)
        {
            if (okuma.sozlesme_id.HasValue)
            {
                return await _sozlesmeService.GetByIdAsync(okuma.sozlesme_id.Value);
            }

            long? tuketimNoktasiId = null;

            if (okuma.is_emri_id.HasValue)
            {
                var isEmri = await _isEmriService.GetByIdAsync(okuma.is_emri_id.Value);
                if (isEmri != null)
                {
                    tuketimNoktasiId = isEmri.tuketim_noktasi_id;
                }
            }

            if (!tuketimNoktasiId.HasValue && okuma.sayac_id.HasValue)
            {
                var sayac = await _sayacService.GetByIdAsync(okuma.sayac_id.Value);
                if (sayac != null)
                {
                    tuketimNoktasiId = sayac.tuketim_noktasi_id;
                }
            }

            if (tuketimNoktasiId.HasValue)
            {
                var tn = await _tuketimNoktasiService.GetByIdAsync((int)tuketimNoktasiId.Value);
                if (tn != null)
                {
                    var pagedSozlesme = await _sozlesmeService.GetPagedAsync(1, 10, null, null, tn.tekil_kod);
                    return pagedSozlesme.Data.FirstOrDefault();
                }
            }

            return null;
        }

        private static string GetTarifeGrubu(int? tarifeId) => tarifeId switch
        {
            2 => "Sanayi",
            3 => "Ticarethane",
            4 => "Tarımsal Sulama",
            5 => "Aydınlatma",
            _ => "Mesken"
        };

        private static bool TryParseDogrulamaDurumu(string? value, out KcetasWeb.Models.Enums.DogrulamaDurumu dogrulamaDurumu)
        {
            dogrulamaDurumu = default;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var trimmed = value.Trim();
            if (int.TryParse(trimmed, out var intValue) && Enum.IsDefined(typeof(KcetasWeb.Models.Enums.DogrulamaDurumu), intValue))
            {
                dogrulamaDurumu = (KcetasWeb.Models.Enums.DogrulamaDurumu)intValue;
                return true;
            }

            return Enum.TryParse(trimmed, ignoreCase: true, out dogrulamaDurumu);
        }

        private async Task<(bool Basarili, decimal Endeks)> GetSonEndeksForSayacAsync(long sayacId, string? seriNo)
        {
            if (!string.IsNullOrWhiteSpace(seriNo))
            {
                var sonOkumaResponse = await _endeksOkumaService.GetPagedAsync(
                    1,
                    1,
                    null,
                    null,
                    null,
                    null,
                    null,
                    seriNo,
                    null,
                    null,
                    null,
                    null,
                    null);

                var sonOkuma = sonOkumaResponse.Data
                    .OrderByDescending(x => x.okuma_zamani)
                    .FirstOrDefault();

                if (sonOkuma?.yeni_endeks != null)
                {
                    return (true, sonOkuma.yeni_endeks.Value);
                }

                return (false, 0m);
            }

            var okumalar = (await _endeksOkumaService.GetAllAsync()).Where(x => x.sayac_id == sayacId)
                .OrderByDescending(x => x.okuma_zamani)
                .ToList();

            if (okumalar.Any() && okumalar.First().yeni_endeks != null)
            {
                return (true, okumalar.First().yeni_endeks!.Value);
            }

            return (false, 0m);
        }

        private async Task<bool> OkumaDonemKaydiVarAsync(long sayacId, string? seriNo, string? donem)
        {
            if (!string.IsNullOrWhiteSpace(seriNo) && !string.IsNullOrWhiteSpace(donem))
            {
                var response = await _endeksOkumaService.GetPagedAsync(
                    1,
                    5,
                    null,
                    null,
                    null,
                    null,
                    null,
                    seriNo,
                    donem,
                    null,
                    null,
                    null,
                    null);

                if (response.Data.Any(x => x.sayac_id == sayacId && x.donem == donem))
                {
                    return true;
                }

                return false;
            }

            var okumalar = await _endeksOkumaService.GetAllAsync();
            return okumalar.Any(x => x.sayac_id == sayacId && x.donem == donem);
        }

        private static bool SameAmount(decimal? left, decimal? right)
        {
            return Math.Abs((left ?? 0m) - (right ?? 0m)) < 0.0001m;
        }

        private static string NormalizeForSearch(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.Trim()
                .ToLowerInvariant()
                .Replace('ç', 'c')
                .Replace('ğ', 'g')
                .Replace('ı', 'i')
                .Replace('ö', 'o')
                .Replace('ş', 's')
                .Replace('ü', 'u')
                .Replace('â', 'a')
                .Replace('î', 'i')
                .Replace('û', 'u');
        }
    }
}
