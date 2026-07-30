using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = $"{AppRoles.FaturalamaUzmani},{AppRoles.SozlesmeYetkilisi},{AppRoles.Denetci},{AppRoles.BTYoneticisi},Yonetici")]
    public class BelgelerController : Controller
    {
        private readonly IFaturaService _faturaService;
        private readonly IIsEmriService _isEmriService;
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly ISozlesmeService _sozlesmeService;
        private readonly IAboneService _aboneService;

        public BelgelerController(IFaturaService faturaService, IIsEmriService isEmriService, ITuketimNoktasiService tuketimNoktasiService, ISozlesmeService sozlesmeService, IAboneService aboneService)
        {
            _faturaService = faturaService;
            _isEmriService = isEmriService;
            _tuketimNoktasiService = tuketimNoktasiService;
            _sozlesmeService = sozlesmeService;
            _aboneService = aboneService;
        }

        public async Task<IActionResult> Index(KcetasWeb.ViewModels.BelgelerListeViewModel filtre)
        {
            var viewModel = filtre ?? new BelgelerListeViewModel();
            
            var tuketimNoktasiTask = _tuketimNoktasiService.GetAllAsync();
            var faturaTask = _faturaService.GetAllAsync();
            var isEmriTask = _isEmriService.GetAllAsync();
            var sozlesmeTask = _sozlesmeService.GetAllAsync();

            await Task.WhenAll(tuketimNoktasiTask, faturaTask, isEmriTask, sozlesmeTask);

            var tnMap = (await tuketimNoktasiTask).GroupBy(t => t.tuketim_noktasi_id).ToDictionary(g => g.Key, g => g.First());
            var tumBelgeler = new List<BelgeSatirViewModel>();

            // 1. Faturaları Çek
            var faturalar = (await faturaTask);
            var sozlesmelerD = (await sozlesmeTask).GroupBy(s => s.sozlesme_id).ToDictionary(g => g.Key, g => g.First());

            foreach (var f in faturalar)
            {
                string gercekTekilKod = f.tekil_kod ?? "";
                if (sozlesmelerD.ContainsKey(f.sozlesme_id))
                {
                    var sozlesme = sozlesmelerD[f.sozlesme_id];
                    if (sozlesme.tuketim_noktasi_id > 0 && tnMap.ContainsKey(sozlesme.tuketim_noktasi_id))
                    {
                        gercekTekilKod = tnMap[sozlesme.tuketim_noktasi_id].tekil_kod;
                    }
                }

                tumBelgeler.Add(new BelgeSatirViewModel
                {
                    BelgeTipi = "Fatura",
                    BelgeNo = !string.IsNullOrEmpty(f.fatura_no) ? f.fatura_no : $"FAT-{f.fatura_id}",
                    TuketimNoktasiKodu = gercekTekilKod,
                    Tarih = f.fatura_tarihi.HasValue ? f.fatura_tarihi.Value.ToDateTime(TimeOnly.MinValue) : (f.created_at ?? DateTime.Now),
                    Tutar = f.toplam_tutar,
                    Aciklama = "Dönem Faturası",
                    Url = $"/Belgeler/Goruntule?tip=Fatura&id={f.fatura_id}"
                });
            }

            // 2. Tutanakları Çek (İş Emri tablosunda durumu Tamamlandi olanlar)
            var tumIsEmirleri = (await isEmriTask);
            var isEmirleri = tumIsEmirleri.Where(ie => ie.durum == KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi);
            foreach (var ie in isEmirleri)
            {
                var tn = tnMap.ContainsKey(ie.tuketim_noktasi_id) ? tnMap[ie.tuketim_noktasi_id] : null;
                tumBelgeler.Add(new BelgeSatirViewModel
                {
                    BelgeTipi = "Tutanak",
                    BelgeNo = !string.IsNullOrEmpty(ie.tutanak_no) ? ie.tutanak_no : $"TUT-{ie.is_emri_id}",
                    TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{ie.tuketim_noktasi_id}",
                    Tarih = ie.updated_at ?? ie.created_at,
                    Tutar = null,
                    Aciklama = ie.tip switch
                    {
                        KcetasWeb.Models.Enums.IsEmriTipi.YeniBaglanti => "Yeni Bağlantı Tutanağı",
                        KcetasWeb.Models.Enums.IsEmriTipi.Sokme => "Sayaç Sökme Tutanağı",
                        KcetasWeb.Models.Enums.IsEmriTipi.Degistirme => "Sayaç Değişim Tutanağı",
                        KcetasWeb.Models.Enums.IsEmriTipi.Kesme => "Enerji Kesme Tutanağı",
                        KcetasWeb.Models.Enums.IsEmriTipi.Acma => "Enerji Açma Tutanağı",
                        _ => "Saha Operasyon Tutanağı"
                    },
                    Url = $"/Belgeler/Goruntule?tip=Tutanak&id={ie.is_emri_id}"
                });
            }

            // 3. Sözleşmeleri Çek
            var sozlesmeler = (await sozlesmeTask);
            foreach (var s in sozlesmeler)
            {
                var tn = tnMap.ContainsKey(s.tuketim_noktasi_id) ? tnMap[s.tuketim_noktasi_id] : null;
                tumBelgeler.Add(new BelgeSatirViewModel
                {
                    BelgeTipi = "Sözleşme",
                    BelgeNo = s.sozlesme_no,
                    TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{s.tuketim_noktasi_id}",
                    Tarih = s.created_at,
                    Tutar = null,
                    Aciklama = $"{s.sozlesme_tipi} Sözleşmesi",
                    Url = $"/Belgeler/Goruntule?tip=Sozlesme&id={s.sozlesme_id}"
                });
            }

            // Filtreleme
            if (viewModel.FiltreBelgeTarihi.HasValue)
                tumBelgeler = tumBelgeler.Where(b => b.Tarih.Date == viewModel.FiltreBelgeTarihi.Value.Date).ToList();

            if (!string.IsNullOrEmpty(viewModel.FiltreBelgeNo))
                tumBelgeler = tumBelgeler.Where(b => b.BelgeNo != null && b.BelgeNo.Contains(viewModel.FiltreBelgeNo, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(viewModel.FiltreTekilKod))
                tumBelgeler = tumBelgeler.Where(b => b.TuketimNoktasiKodu != null && b.TuketimNoktasiKodu.Contains(viewModel.FiltreTekilKod, StringComparison.OrdinalIgnoreCase)).ToList();

            // Aktif Sekme Filtresi (Böylece her sekme kendi içinde sayfalanır)
            if (viewModel.AktifSekme == "Fatura")
                tumBelgeler = tumBelgeler.Where(b => b.BelgeTipi == "Fatura").ToList();
            else if (viewModel.AktifSekme == "Sozlesme")
                tumBelgeler = tumBelgeler.Where(b => b.BelgeTipi == "Sözleşme").ToList();
            else if (viewModel.AktifSekme == "Tutanak")
                tumBelgeler = tumBelgeler.Where(b => b.BelgeTipi == "Tutanak").ToList();

            // Sıralama (En yeni en üstte)
            tumBelgeler = tumBelgeler.OrderByDescending(b => b.Tarih).ToList();

            int totalItems = tumBelgeler.Count;
            
            viewModel.CurrentPage = viewModel.CurrentPage > 0 ? viewModel.CurrentPage : 1;
            viewModel.PageSize = viewModel.PageSize > 0 && viewModel.PageSize != 50 ? viewModel.PageSize : 2000;
            
            var pagedData = tumBelgeler.Skip((viewModel.CurrentPage - 1) * viewModel.PageSize).Take(viewModel.PageSize).ToList();

            viewModel.TotalItems = totalItems;
            viewModel.Belgeler = pagedData;

            return View(viewModel);
        }

        public async Task<IActionResult> Goruntule(string tip, int id)
        {
            var viewModel = new BelgeGoruntuleViewModel();
            viewModel.BelgeTipi = tip;

            async Task<string> GetAboneAdi(int? aboneId)
            {
                if (!aboneId.HasValue || aboneId.Value <= 0) return "Abone Bilgisi Yok";
                var abone = await _aboneService.GetByIdAsync(aboneId.Value);
                if (abone == null) return "Bilinmeyen Abone";
                return (abone.abone_tipi == KcetasWeb.Models.Enums.AboneTipi.Bireysel) 
                    ? $"{abone.Ad} {abone.Soyad} (TC: {abone.tckn})" 
                    : $"{abone.Unvan} (VKN: {abone.vkn})";
            }

            if (tip == "Fatura")
            {
                var fatura = await _faturaService.GetByIdAsync(id);
                if (fatura == null) return NotFound();

                viewModel.BelgeNo = fatura.fatura_no;
                viewModel.Tarih = fatura.fatura_tarihi.HasValue ? fatura.fatura_tarihi.Value.ToDateTime(TimeOnly.MinValue) : (fatura.created_at ?? DateTime.Now);
                viewModel.TuketimNoktasiKod = fatura.tekil_kod;
                viewModel.FaturaTutar = fatura.toplam_tutar;
                viewModel.FaturaTuketim = fatura.tuketim_kwh;
                viewModel.FaturaDurum = fatura.durum;

                var sozlesme = (await _sozlesmeService.GetAllAsync()).FirstOrDefault(s => s.sozlesme_id == fatura.sozlesme_id);
                if (sozlesme != null)
                {
                    viewModel.AboneBilgisi = await GetAboneAdi(sozlesme.abone_id);
                }
            }
            else if (tip == "Sozlesme")
            {
                var sozlesme = (await _sozlesmeService.GetAllAsync()).FirstOrDefault(s => s.sozlesme_id == id);
                if (sozlesme == null) return NotFound();

                viewModel.BelgeNo = sozlesme.sozlesme_no;
                viewModel.Tarih = sozlesme.created_at;
                viewModel.SozlesmeTipi = sozlesme.sozlesme_tipi?.ToString();
                viewModel.SozlesmeDurum = sozlesme.durum?.ToString();
                viewModel.GuvenceBedeli = sozlesme.guvence_bedeli;
                viewModel.AboneBilgisi = await GetAboneAdi(sozlesme.abone_id);

                var tn = (await _tuketimNoktasiService.GetAllAsync()).FirstOrDefault(t => t.tuketim_noktasi_id == sozlesme.tuketim_noktasi_id);
                viewModel.TuketimNoktasiKod = tn?.tekil_kod ?? $"TK-ID-{sozlesme.tuketim_noktasi_id}";
            }
            else if (tip == "Tutanak")
            {
                var isEmri = await _isEmriService.GetByIdAsync(id);
                if (isEmri == null) return NotFound();

                viewModel.BelgeNo = !string.IsNullOrEmpty(isEmri.tutanak_no) ? isEmri.tutanak_no : $"TUT-{isEmri.is_emri_id}";
                viewModel.Tarih = isEmri.updated_at ?? isEmri.created_at;
                viewModel.TutanakIslemTipi = isEmri.tip.ToString();
                viewModel.TutanakSahaSonucu = isEmri.saha_sonucu ?? "Sonuç girilmemiş";
                viewModel.TutanakGerekce = isEmri.gerekce;
                viewModel.TutanakDurum = isEmri.durum.ToString();
                
                var tn = (await _tuketimNoktasiService.GetAllAsync()).FirstOrDefault(t => t.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);
                if (tn != null)
                {
                    viewModel.TuketimNoktasiKod = tn.tekil_kod;
                    var aboneSozlesme = (await _sozlesmeService.GetAllAsync()).FirstOrDefault(s => s.tuketim_noktasi_id == tn.tuketim_noktasi_id && s.durum == KcetasWeb.Models.Enums.SozlesmeDurumu.Aktif);
                    if (aboneSozlesme != null)
                    {
                        viewModel.AboneBilgisi = await GetAboneAdi(aboneSozlesme.abone_id);
                    }
                }
            }
            else
            {
                return BadRequest();
            }

            return View(viewModel);
        }
    }
}

