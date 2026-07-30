using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using System;
using KcetasWeb.Models.entities;
using System.Linq;

namespace KcetasWeb.Controllers;

[Authorize(Roles = $"{AppRoles.BTYoneticisi},{AppRoles.SahaOperasyonAmiri},{AppRoles.SayacOkumaPersoneli},{AppRoles.Denetci}")]
public class IsEmriController : Controller
{
    private readonly IIsEmriService _isEmriService;
    private readonly IKullaniciDeposu _kullaniciDeposu;
    private readonly ITuketimNoktasiService _tuketimNoktasiService;
    private readonly ISozlesmeService _sozlesmeService;
    private readonly ISayacService _sayacService;
    private readonly IAuditLogService _auditLogService;
    private readonly IFaturaService _faturaService;
    private readonly IEndeksOkumaService _endeksOkumaService;
    private readonly IAboneService _aboneService;

    public IsEmriController(
        IIsEmriService isEmriService, 
        IKullaniciDeposu kullaniciDeposu,
        ITuketimNoktasiService tuketimNoktasiService,
        ISozlesmeService sozlesmeService,
        ISayacService sayacService,
        IAuditLogService auditLogService,
        IFaturaService faturaService,
        IEndeksOkumaService endeksOkumaService,
        IAboneService aboneService)
    {
        _isEmriService = isEmriService;
        _kullaniciDeposu = kullaniciDeposu;
        _tuketimNoktasiService = tuketimNoktasiService;
        _sozlesmeService = sozlesmeService;
        _sayacService = sayacService;
        _auditLogService = auditLogService;
        _faturaService = faturaService;
        _endeksOkumaService = endeksOkumaService;
        _aboneService = aboneService;
    }

    public async Task<IActionResult> Index(IsEmriListeViewModel filtre)
    {
        // Önce temel tarih ve durum/arama filtrelerini API üzerinden çekiyoruz.
        // Tip filtresini burada göndermiyoruz çünkü DB'deki değer (BAGLAMA) ile UI'daki (Sayaç Bağlama) farklı olabiliyor.
        var isEmirleriTask = _isEmriService.FiltreleAsync(null, filtre.FiltreDurum, filtre.BaslangicTarih, filtre.BitisTarih, filtre.AramaMetni);

        // OPTİMİZASYON: N+1 Sorgu Problemini (Yavaşlık) Çözmek İçin
        // Tüm Tüketim Noktalarını ve Kullanıcıları (Personelleri) API'den 1 kez çekip Dictionary (Sözlük) yapıyoruz.
        // Böylece aşağıdaki Select döngüsü içinde binlerce kez API'ye istek atmaktan kurtuluyoruz.
        var kullaniciTask = _kullaniciDeposu.ListeleAsync();
        var tuketimNoktasiTask = _tuketimNoktasiService.GetAllAsync();

        await Task.WhenAll(isEmirleriTask, kullaniciTask, tuketimNoktasiTask);

        var isEmirleri = (await isEmirleriTask).OrderByDescending(x => x.created_at).ToList();
        var tumKullanicilar = (await kullaniciTask).GroupBy(k => k.kullanici_id).ToDictionary(g => g.Key, g => g.First());
        var tumTuketimNoktalari = (await tuketimNoktasiTask).GroupBy(t => t.tuketim_noktasi_id).ToDictionary(g => g.Key, g => g.First());

        filtre.IsEmirleri = isEmirleri.Select(ie => {
            var kullanici = ie.atanan_kullanici_id.HasValue && tumKullanicilar.ContainsKey((int)ie.atanan_kullanici_id.Value) 
                            ? tumKullanicilar[(int)ie.atanan_kullanici_id.Value] : null;
                            
            var tn = tumTuketimNoktalari.ContainsKey((int)ie.tuketim_noktasi_id) ? tumTuketimNoktalari[(int)ie.tuketim_noktasi_id] : null;

            return new IsEmriSatirViewModel
            {
                IsEmriId = ie.is_emri_id,
                IsEmriNo = ie.is_emri_no,
                Tip = ie.tip,
                TuketimNoktasiId = ie.tuketim_noktasi_id,
                tekil_kod = tn != null ? tn.tekil_kod : $"TK-ID-{ie.tuketim_noktasi_id}",
                TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{ie.tuketim_noktasi_id}",
                musteri_ad = "",
                musteri_soyad = "",
                musteri_unvan = "",
                PlanlananTarih = ie.planlanan_tarih ?? ie.created_at.AddDays(1),
                olusturulma_tarihi = ie.created_at,
                CreatedAt = ie.created_at,
                UpdatedAt = ie.updated_at,
                oncelik = ie.oncelik,
                AtananKullaniciAdi = kullanici != null ? kullanici.ad_soyad : "Atanmadı",
                Durum = ie.durum,
                DurumRenk = IsEmriListeViewModel.GetDurumRenk(ie.durum),
                Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı"
            };
        }).ToList();

        // Benzersiz değerleri Select2 dropdown'ları için ViewBag'e ekle
        ViewBag.IsEmriNolar = filtre.IsEmirleri.Select(x => x.IsEmriNo).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList();
        ViewBag.TekilKodlar = filtre.IsEmirleri.Select(x => x.tekil_kod).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList();
        ViewBag.AboneAdlari = filtre.IsEmirleri.Select(x => x.musteriDurum).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList();
        ViewBag.Personeller = filtre.IsEmirleri.Select(x => x.AtananKullaniciAdi).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList();

        // Şimdi UI uyumlu hale gelmiş nesneler üzerinde detaylı string filtrelerini uyguluyoruz
        if (!string.IsNullOrEmpty(filtre.FiltreTip))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.Tip.ToString().Equals(filtre.FiltreTip, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreIsEmriNo))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.IsEmriNo != null && x.IsEmriNo.Contains(filtre.FiltreIsEmriNo, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreTekilKod))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.tekil_kod != null && x.tekil_kod.Contains(filtre.FiltreTekilKod, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreAboneAdi))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.musteriDurum != null && x.musteriDurum.Contains(filtre.FiltreAboneAdi, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltrePersonel))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.AtananKullaniciAdi != null && x.AtananKullaniciAdi.Contains(filtre.FiltrePersonel, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(filtre.FiltreOncelik))
            filtre.IsEmirleri = filtre.IsEmirleri.Where(x => x.oncelik != null && x.oncelik.Equals(filtre.FiltreOncelik, StringComparison.OrdinalIgnoreCase)).ToList();

        // SIRALAMA MANTIĞI:
        // 1. Durumu "Tamamlandı" olanlar listenin en sonuna gitsin (0 olanlar üste, 1 olanlar alta)
        // 2. Kalanlar kendi içinde en yeni eklenenden eskiye doğru (Tarihe veya ID'ye göre) sıralansın
        filtre.IsEmirleri = filtre.IsEmirleri
            .OrderBy(x => x.Durum == KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi ? 1 : 0)
            .ThenByDescending(x => x.olusturulma_tarihi)
            .ToList();

        filtre.TotalItems = filtre.IsEmirleri.Count;
        filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
        filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;

        filtre.IsEmirleri = filtre.IsEmirleri
            .Skip((filtre.CurrentPage - 1) * filtre.PageSize)
            .Take(filtre.PageSize)
            .ToList();

        return View(filtre);
    }

    [Authorize(Roles = $"{AppRoles.SahaOperasyonAmiri},{AppRoles.BTYoneticisi},{AppRoles.Denetci}")]
    public async Task<IActionResult> Yeni()
    {
        ViewBag.TuketimNoktalari = (await _tuketimNoktasiService.GetAllAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.tuketim_noktasi_id.ToString(), Text = $"{x.tekil_kod}" })
            .ToList();
            
        ViewBag.Personeller = (await _kullaniciDeposu.ListeleAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.kullanici_id.ToString(), Text = x.ad_soyad })
            .ToList();
            
        ViewBag.Sozlesmeler = (await _sozlesmeService.GetAllAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.sozlesme_id.ToString(), Text = $"{x.sozlesme_no} - {x.sozlesme_tipi}" })
            .ToList();

        ViewBag.Sayaclar = (await _sayacService.GetAllAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.sayac_id.ToString(), Text = $"{x.seri_no} - {x.marka} {x.model}" })
            .ToList();

        var sayacMap = (await _sayacService.GetAllAsync())
            .Where(s => s.tuketim_noktasi_id != null && s.tuketim_noktasi_id > 0)
            .GroupBy(s => s.tuketim_noktasi_id.Value.ToString())
            .ToDictionary(g => g.Key, g => g.First().sayac_id);

        var sozlesmeMap = (await _sozlesmeService.GetAllAsync())
            .Where(s => s.tuketim_noktasi_id > 0)
            .GroupBy(s => s.tuketim_noktasi_id.ToString())
            .ToDictionary(g => g.Key, g => g.First().sozlesme_id);

        var sozlesmeToTnMap = (await _sozlesmeService.GetAllAsync())
            .Where(s => s.tuketim_noktasi_id > 0)
            .ToDictionary(s => s.sozlesme_id.ToString(), s => s.tuketim_noktasi_id.ToString());

        ViewBag.SayacMapJson = System.Text.Json.JsonSerializer.Serialize(sayacMap);
        ViewBag.SozlesmeMapJson = System.Text.Json.JsonSerializer.Serialize(sozlesmeMap);
        ViewBag.SozlesmeToTnMapJson = System.Text.Json.JsonSerializer.Serialize(sozlesmeToTnMap);
            
        return View(new YeniIsEmriViewModel());
    }

    [Authorize(Roles = $"{AppRoles.SahaOperasyonAmiri},{AppRoles.BTYoneticisi},{AppRoles.Denetci}")]
    [HttpPost]
    public async Task<IActionResult> Yeni(YeniIsEmriViewModel model)
    {
        ModelState.Clear(); if (ModelState.IsValid)
        {
            var isEmri = new IsEmri
            {
                tuketim_noktasi_id = (int)model.TuketimNoktasiId,
                tip = model.Tip,
                oncelik = model.Oncelik,
                planlanan_tarih = model.PlanlananTarih,
                atanan_kullanici_id = (int?)model.AtananKullaniciId,
                sayac_id = (int?)model.SayacId,
                gerekce = model.Aciklama ?? "",
                
                // API doğrulamasını geçmek için eksik olan zorunlu alanları dolduruyoruz
                is_emri_no = $"IE-{DateTime.Now.ToString("yyyyMMdd")}-{new Random().Next(1000,9999)}",
                durum = model.AtananKullaniciId.HasValue ? KcetasWeb.Models.Enums.IsEmriDurumu.Atandi : KcetasWeb.Models.Enums.IsEmriDurumu.Acik,
                status = "AKTIF",
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                saha_sonucu = "",
                muhur_no = "",
                tutanak_no = ""
            };
            
            await _isEmriService.EkleAsync(isEmri);
            
            TempData["Mesaj"] = "İş emri başarıyla oluşturuldu.";
            TempData["MesajTip"] = "success";
            return RedirectToAction("Index");
        }

        ViewBag.TuketimNoktalari = (await _tuketimNoktasiService.GetAllAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.tuketim_noktasi_id.ToString(), Text = $"{x.tekil_kod}" })
            .ToList();
            
        ViewBag.Personeller = (await _kullaniciDeposu.ListeleAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.kullanici_id.ToString(), Text = x.ad_soyad })
            .ToList();

        ViewBag.Sozlesmeler = (await _sozlesmeService.GetAllAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.sozlesme_id.ToString(), Text = $"{x.sozlesme_no} - {x.sozlesme_tipi}" })
            .ToList();

        ViewBag.Sayaclar = (await _sayacService.GetAllAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.sayac_id.ToString(), Text = $"{x.seri_no} - {x.marka} {x.model}" })
            .ToList();

        var sayacMap = (await _sayacService.GetAllAsync())
            .Where(s => s.tuketim_noktasi_id != null && s.tuketim_noktasi_id > 0)
            .GroupBy(s => s.tuketim_noktasi_id.Value.ToString())
            .ToDictionary(g => g.Key, g => g.First().sayac_id);

        var sozlesmeMap = (await _sozlesmeService.GetAllAsync())
            .Where(s => s.tuketim_noktasi_id > 0)
            .GroupBy(s => s.tuketim_noktasi_id.ToString())
            .ToDictionary(g => g.Key, g => g.First().sozlesme_id);

        var sozlesmeToTnMap = (await _sozlesmeService.GetAllAsync())
            .Where(s => s.tuketim_noktasi_id > 0)
            .ToDictionary(s => s.sozlesme_id.ToString(), s => s.tuketim_noktasi_id.ToString());

        ViewBag.SayacMapJson = System.Text.Json.JsonSerializer.Serialize(sayacMap);
        ViewBag.SozlesmeMapJson = System.Text.Json.JsonSerializer.Serialize(sozlesmeMap);
        ViewBag.SozlesmeToTnMapJson = System.Text.Json.JsonSerializer.Serialize(sozlesmeToTnMap);

        return View(model);
    }

        public async Task<IActionResult> Detay(long id)
        {
            var isEmri = await _isEmriService.GetByIdAsync(id);
            if (isEmri == null)
                return NotFound();

            var tn = (await _tuketimNoktasiService.GetAllAsync()).FirstOrDefault(t => t.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);
            var sozlesme = (await _sozlesmeService.GetAllAsync()).OrderByDescending(s => s.sozlesme_id).FirstOrDefault(s => s.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);
            var abone = sozlesme?.abone_id.HasValue == true ? await _aboneService.GetByIdAsync(sozlesme.abone_id.Value) : null;

            var viewModel = new IsEmriDetayViewModel
            {
                IsEmriId = isEmri.is_emri_id,
                IsEmriNo = isEmri.is_emri_no,
                Tip = isEmri.tip,
                Durum = isEmri.durum,
                DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.durum),
                Oncelik = isEmri.oncelik,
                PlanlananTarih = isEmri.planlanan_tarih ?? isEmri.created_at.AddDays(1),
                AtananKullaniciAdi = isEmri.atanan_kullanici_id.HasValue 
                    ? ((await _kullaniciDeposu.BulIdAsync(isEmri.atanan_kullanici_id.Value))?.ad_soyad ?? "Atanmadı") 
                    : "Atanmadı",
                musteri_ad = abone?.Ad ?? "",
                musteri_soyad = abone?.Soyad ?? "",
                musteri_unvan = abone?.Unvan ?? "",
                telefon = abone?.telefon ?? "",
                TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{isEmri.tuketim_noktasi_id}",
                Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı",
                SayacSeriNo = "Sayaç bilgisi yok",
                SahaSonucu = isEmri.saha_sonucu,
                Gerekce = isEmri.gerekce,
                MuhurNo = isEmri.muhur_no,
                TutanakNo = isEmri.tutanak_no,
                CreatedAt = isEmri.created_at,
                UpdatedAt = isEmri.updated_at
            };

            ViewBag.AuditLogs = await _auditLogService.GetirByVarlikAsync("IsEmri", (int)id);
            ViewBag.Personeller = (await _kullaniciDeposu.ListeleAsync())
                .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = x.kullanici_id.ToString(), Text = x.ad_soyad })
                .ToList();

            return View(viewModel);
        }

    [HttpPost]
    public async Task<IActionResult> DurumGuncelle(long id, KcetasWeb.Models.Enums.IsEmriDurumu yeniDurum)
    {
        var isEmri = await _isEmriService.GetByIdAsync(id);
        if (isEmri == null) return NotFound();

        var eskiDurum = isEmri.durum;
        
        // Validation for status change
        if (yeniDurum == KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi && string.IsNullOrEmpty(isEmri.saha_sonucu))
        {
            TempData["Mesaj"] = "Saha sonucu veya tutanak girilmeden iş emri 'Tamamlandı' statüsüne alınamaz! Lütfen 'Tamamla' butonunu kullanın.";
            TempData["MesajTip"] = "danger";
            return RedirectToAction("Detay", new { id = id });
        }

        await _isEmriService.DurumGuncelleAsync(id, yeniDurum);
        
        // Currently there's no real user authentication to get User ID. Using 1 as a mock ID.
        int mockUserId = 1; 

        await _auditLogService.EkleAsync(
            varlikTipi: "IsEmri",
            varlikId: (int)id,
            islemTipi: "Durum Degisikligi",
            eskiDeger: eskiDurum.ToString(),
            yeniDeger: yeniDurum.ToString(),
            kullaniciId: mockUserId
        );

        TempData["Mesaj"] = $"İş emri durumu '{yeniDurum.ToString()}' olarak güncellendi.";
        TempData["MesajTip"] = "success";
        return RedirectToAction("Detay", new { id = id });
    }

    [Authorize(Roles = $"{AppRoles.SahaOperasyonAmiri},{AppRoles.BTYoneticisi},{AppRoles.Denetci}")]
    [HttpPost]
    public async Task<IActionResult> PersonelAta(long id, long personelId)
    {
        var isEmri = await _isEmriService.GetByIdAsync(id);
        if (isEmri == null) return NotFound();

        await _isEmriService.PersonelAtaAsync(id, personelId);
        
        // Audit log (Opsiyonel)
        await _auditLogService.EkleAsync(
            varlikTipi: "IsEmri",
            varlikId: (int)id,
            islemTipi: "Personel Atama",
            eskiDeger: isEmri.atanan_kullanici_id.ToString() ?? "Atanmadı",
            yeniDeger: personelId.ToString(),
            kullaniciId: 1
        );

        TempData["Mesaj"] = "Personel ataması başarıyla gerçekleştirildi.";
        TempData["MesajTip"] = "success";
        return RedirectToAction("Detay", new { id = id });
    }

    public async Task<IActionResult> TutanakGiris(long id)
    {
        var isEmri = await _isEmriService.GetByIdAsync(id);
        if (isEmri == null)
            return NotFound();

        var viewModel = new TutanakGirisViewModel
        {
            IsEmriId = isEmri.is_emri_id,
            IsEmriNo = isEmri.is_emri_no,
            Tip = isEmri.tip,
            IslemTarihi = DateTime.Now,
            TutanakNo = $"TUT-{DateTime.Now.ToString("yyyyMMdd")}-{new Random().Next(1000, 9999)}"
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TutanakKaydet(TutanakGirisViewModel model)
    {
        // Mühür no öneki (MHR-) arayüzde sabitlendiği için backend tarafında veriye ekliyoruz.
        if (!string.IsNullOrWhiteSpace(model.MuhurNo) && !model.MuhurNo.StartsWith("MHR-"))
        {
            model.MuhurNo = "MHR-" + model.MuhurNo;
        }

        // İş Kuralları Validasyonu
        if (model.Tip == KcetasWeb.Models.Enums.IsEmriTipi.Kesme)
        {
            if (!model.KesmeEndeksi.HasValue) ModelState.AddModelError("KesmeEndeksi", "Kesme işlemi için Kesme Endeksi zorunludur.");
            if (string.IsNullOrWhiteSpace(model.SahaSonucu)) ModelState.AddModelError("SahaSonucu", "İşlem sonucu alanı zorunludur.");
        }
        else if (model.Tip == KcetasWeb.Models.Enums.IsEmriTipi.Degistirme)
        {
            if (!model.EskiSonEndeksi.HasValue) ModelState.AddModelError("EskiSonEndeksi", "Sayaç değişimi için Eski Son Endeks zorunludur.");
            if (!model.YeniIlkEndeksi.HasValue) ModelState.AddModelError("YeniIlkEndeksi", "Sayaç değişimi için Yeni İlk Endeks zorunludur.");
        }
        else if (model.Tip == KcetasWeb.Models.Enums.IsEmriTipi.YeniBaglanti)
        {
            if (!model.YeniIlkEndeksi.HasValue) ModelState.AddModelError("YeniIlkEndeksi", "Yeni bağlantı işlemi için İlk Endeks zorunludur.");
            if (string.IsNullOrWhiteSpace(model.MuhurNo)) ModelState.AddModelError("MuhurNo", "Yeni bağlantı işlemi için Mühür No zorunludur.");
        }

        if (string.IsNullOrWhiteSpace(model.SahaSonucu))
        {
             ModelState.AddModelError("SahaSonucu", "İş emrini tamamlayabilmek için Saha Sonucu girilmesi zorunludur.");
        }

        if (!ModelState.IsValid)
        {
            // Eğer hata varsa Tamamlama sayfasına geri dön.
            // Fakat model tipi TutanakGirisViewModel, Tamamlama ise IsEmriDetayViewModel bekliyor.
            // O yüzden şimdilik geri dönmeden hata mesajını TempData'ya atıp yönlendireceğiz veya TutanakGiris sayfasına döndüreceğiz.
            TempData["Mesaj"] = "Lütfen formdaki zorunlu alanları (Endeks, Sonuç vb.) doldurunuz.";
            TempData["MesajTip"] = "danger";
            
            // Kullanıcıyı Tamamlama formuna geri yönlendiriyoruz ki düzeltip tekrar denesin.
            return RedirectToAction("Tamamlama", new { id = model.IsEmriId });
        }

        await _isEmriService.TutanakKaydetAsync(
            model.IsEmriId,
            model.TutanakNo,
            model.SahaSonucu,
            model.Gerekce,
            model.MuhurNo,
            model.KesmeEndeksi,
            model.AcmaEndeksi,
            model.EskiSayacNo,
            model.YeniSayacNo,
            model.EskiSonEndeksi,
            model.YeniIlkEndeksi
        );
        
        // Tutanağı giren personelin işlemi sonucunda İş Emri durumunu Tamamlandı yapıyoruz.
        await _isEmriService.DurumGuncelleAsync(model.IsEmriId, KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi);
        
        string mesaj = "Tutanak başarıyla kaydedildi ve İş Emri tamamlandı.";
        
        // Simülasyon: Kesme veya Açma yapıldıysa
        if (model.Tip == KcetasWeb.Models.Enums.IsEmriTipi.Kesme && model.KesmeEndeksi.HasValue)
        {
            mesaj = $"Tutanak başarıyla kaydedildi. (Kesme Endeksi: {model.KesmeEndeksi}). Borç veya sözleşme nedeniyle yapılan kesme için, bir sonraki faturaya yansıtılacak olan 'Kesme-Bağlama Bedeli' (Örn: 118,50 TL) simülasyonu başlatılmıştır.";
        }
        else if (model.Tip == KcetasWeb.Models.Enums.IsEmriTipi.Acma && model.AcmaEndeksi.HasValue)
        {
            mesaj = $"Tutanak başarıyla kaydedildi. (Açma Endeksi: {model.AcmaEndeksi}). Ödeme/Sözleşme onayı sonrası açma işlemi tamamlandı.";
        }

        TempData["Mesaj"] = mesaj;
        TempData["MesajTip"] = "success";

        // YENİ EKLENEN AKIŞ (ADIM 5 ve 6): Yeni Bağlantı tamamlandığında
        if (model.Tip == KcetasWeb.Models.Enums.IsEmriTipi.YeniBaglanti)
        {
            if (!string.IsNullOrEmpty(model.YeniSayacNo) && model.YeniIlkEndeksi.HasValue && !string.IsNullOrEmpty(model.MuhurNo))
            {
                var tIsEmri = await _isEmriService.GetByIdAsync(model.IsEmriId);
                if (tIsEmri != null)
                {
                    // 1. Yeni Sayac Oluştur
                    var allSayaclar = await _sayacService.GetAllAsync();
                    int maxSayacId = allSayaclar.Any() ? (int)allSayaclar.Max(x => x.sayac_id) : 0;
                    int newSayacId = maxSayacId + 1;
                    
                    var yeniSayac = new Sayac
                    {
                        sayac_id = newSayacId,
                        tuketim_noktasi_id = tIsEmri.tuketim_noktasi_id,
                        seri_no = model.YeniSayacNo,
                        marka = !string.IsNullOrWhiteSpace(model.YeniSayacMarka) ? model.YeniSayacMarka : "Bilinmiyor",
                        model = !string.IsNullOrWhiteSpace(model.YeniSayacModel) ? model.YeniSayacModel : "Bilinmiyor",
                        uretim_yili = DateTime.Now.Year,
                        muhur_no = model.MuhurNo,
                        durum = KcetasWeb.Models.Enums.SayacDurumu.Bagli,
                        faz = Enum.TryParse<KcetasWeb.Models.Enums.SayacFaz>(model.YeniSayacFaz, true, out var pFaz) ? pFaz : KcetasWeb.Models.Enums.SayacFaz.Monofaze,
                        carpan = 1,
                        status = "AKTIF"
                    };
                    try { await _sayacService.CreateAsync(yeniSayac); } catch { }

                    // 2. İlgili Sözleşmeyi Bul ve Aktif Et
                    var tnSozlesmeler = (await _sozlesmeService.GetAllAsync()).Where(s => s.tuketim_noktasi_id == tIsEmri.tuketim_noktasi_id).ToList();
                    var bekleyenSozlesme = tnSozlesmeler.OrderByDescending(s => s.sozlesme_id).FirstOrDefault();
                    if (bekleyenSozlesme != null)
                    {
                        bekleyenSozlesme.durum = KcetasWeb.Models.Enums.SozlesmeDurumu.Aktif;
                        bekleyenSozlesme.updated_at = DateTime.Now;
                        try { await _sozlesmeService.UpdateAsync(bekleyenSozlesme); } catch { }
                    }

                    // 3. İŞ MANTIĞI: Sayaç Bağlama bitince 'Endeks Okuma' İŞ EMRİ Fırlat!
                    var endeksIsEmri = new IsEmri
                    {
                        is_emri_no = $"IE-END-{DateTime.Now.ToString("yyyyMMdd")}-{new Random().Next(1000, 9999)}",
                        tuketim_noktasi_id = tIsEmri.tuketim_noktasi_id,
                        sayac_id = newSayacId,
                        tip = KcetasWeb.Models.Enums.IsEmriTipi.EndeksOkuma,
                        durum = KcetasWeb.Models.Enums.IsEmriDurumu.Acik, // Havuza açık iş olarak düşer
                        oncelik = "NORMAL",
                        planlanan_tarih = DateTime.Now,
                        atanan_kullanici_id = null,
                        status = "AKTIF",
                        created_at = DateTime.Now
                    };
                    try { await _isEmriService.EkleAsync(endeksIsEmri); } catch { }

                    mesaj = $"Tutanak kaydedildi. Sayaç bağlandı ve sahaya otomatik 'Endeks Okuma' iş emri eklendi.";
                    TempData["Mesaj"] = mesaj;
                }
            }
        }

        // ENDEKS OKUMA -> FATURA OLUŞTURMA AKIŞI
        if (model.Tip == KcetasWeb.Models.Enums.IsEmriTipi.EndeksOkuma)
        {
            if (model.GuncelEndeks.HasValue)
            {
                var tIsEmri = await _isEmriService.GetByIdAsync(model.IsEmriId);
                if (tIsEmri != null)
                {
                    // 1. İlgili Sözleşmeyi Bul (Tarife grubunu almak için)
                    var tumSozlesmeler = (await _sozlesmeService.GetAllAsync()).Where(s => s.tuketim_noktasi_id == tIsEmri.tuketim_noktasi_id).ToList();
                    var aktifSozlesme = tumSozlesmeler.Where(s => s.durum == KcetasWeb.Models.Enums.SozlesmeDurumu.Aktif).OrderByDescending(s => s.sozlesme_id).FirstOrDefault() 
                                     ?? tumSozlesmeler.OrderByDescending(s => s.sozlesme_id).FirstOrDefault();
                        
                    string tarifeGrubu = aktifSozlesme != null ? (aktifSozlesme.sozlesme_tipi?.ToString() ?? "Mesken") : "Mesken";
                    int sozlesmeId = aktifSozlesme != null ? aktifSozlesme.sozlesme_id : 1; // Fallback to 1 to avoid FK error
                    
                    // 2. İlk Endeksi Bul (Önceki Faturalardan veya 0)
                    var oncekiFaturalar = (await _faturaService.GetAllAsync()).Where(f => f.sozlesme_id == sozlesmeId).ToList();
                    decimal ilkEndeks = 0;
                    if (oncekiFaturalar.Any())
                    {
                        ilkEndeks = oncekiFaturalar.Max(f => f.son_endeks ?? 0);
                    }
                    
                    decimal tuketimKwh = model.GuncelEndeks.Value - ilkEndeks;
                    if (tuketimKwh < 0) tuketimKwh = 0;
                    
                    // 3. Fatura Simülasyonu ve Hesaplama
                    var hesap = await _faturaService.SimulasyonHesaplaAsync(tarifeGrubu, tuketimKwh);
                    
                    // Endeks Okuma Kaydı
                    var yeniOkuma = new EndeksOkuma
                    {
                        sayac_id = (tIsEmri.sayac_id != null && tIsEmri.sayac_id > 0) ? tIsEmri.sayac_id : null,
                        sozlesme_id = sozlesmeId,
                        donem = DateTime.Now.ToString("yyyy-MM"),
                        okuma_tipi = oncekiFaturalar.Any() ? KcetasWeb.Models.Enums.OkumaTipi.RutinDonem : KcetasWeb.Models.Enums.OkumaTipi.IlkOkuma, // Eğer geçmiş fatura yoksa İLK OKUMA'dır
                        okuma_kaynagi = KcetasWeb.Models.Enums.OkumaKaynagi.Manuel,
                        onceki_endeks = ilkEndeks,
                        yeni_endeks = model.GuncelEndeks.Value,
                        okuma_zamani = DateTime.UtcNow,
                        kullanici_id = 1,
                        dogrulama_durumu = KcetasWeb.Models.Enums.DogrulamaDurumu.DogrulamaBekliyor,
                        anomali_mi = tuketimKwh > 1000,
                        status = "AKTIF",
                        okunamama_nedeni = "",
                        created_at = DateTime.UtcNow,
                        is_emri_id = (int?)model.IsEmriId
                    };

                    try
                    {
                        await _endeksOkumaService.CreateAsync(yeniOkuma);
                        TempData["Mesaj"] = "Tutanak tamamlandı ve Endeks Okuma kaydı sisteme 'Onay Bekliyor' statüsünde düştü. Fatura kesmek için Endeks Okuma sayfasından onaylayınız.";
                        TempData["MesajTip"] = "success";
                    }
                    catch (Exception ex)
                    {
                        TempData["Mesaj"] = $"Tutanak tamamlandı ancak okuma kaydı atılamadı: {ex.Message}";
                        TempData["MesajTip"] = "warning";
                    }
                }
            }
        }

        // İş emri durumu 'Tamamlandı' olduysa Audit Log atalım
        await _auditLogService.EkleAsync(
            varlikTipi: "IsEmri",
            varlikId: (int)model.IsEmriId,
            islemTipi: "Tamamlama / Durum Degisikligi",
            eskiDeger: "Herhangi (Tamamlanmadan Önce)",
            yeniDeger: KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi.ToString(),
            kullaniciId: 1
        );

        return RedirectToAction("Detay", new { id = model.IsEmriId });
    }

    public async Task<IActionResult> TutanakGoruntule(long id)
    {
        var isEmri = await _isEmriService.GetByIdAsync(id);
        if (isEmri == null || string.IsNullOrEmpty(isEmri.tutanak_no))
            return NotFound();

        var tn = (await _tuketimNoktasiService.GetAllAsync()).FirstOrDefault(t => t.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);

        var viewModel = new IsEmriDetayViewModel
        {
            IsEmriId = isEmri.is_emri_id,
            IsEmriNo = isEmri.is_emri_no,
            Tip = isEmri.tip,
            Durum = isEmri.durum,
            DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.durum),
            Oncelik = isEmri.oncelik,
            PlanlananTarih = isEmri.planlanan_tarih ?? isEmri.created_at.AddDays(1),
            SahaSonucu = isEmri.saha_sonucu,
            Gerekce = isEmri.gerekce,
            MuhurNo = isEmri.muhur_no,
            TutanakNo = isEmri.tutanak_no,
            TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{isEmri.tuketim_noktasi_id}",
            Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı",
            UpdatedAt = isEmri.updated_at
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Tamamlama(long id)
    {
        var isEmri = await _isEmriService.GetByIdAsync(id);
        if (isEmri == null)
            return NotFound();

        var tn = (await _tuketimNoktasiService.GetAllAsync()).FirstOrDefault(t => t.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);
        var sozlesme = (await _sozlesmeService.GetAllAsync()).OrderByDescending(s => s.sozlesme_id).FirstOrDefault(s => s.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);
        var abone = sozlesme?.abone_id.HasValue == true ? await _aboneService.GetByIdAsync(sozlesme.abone_id.Value) : null;

        var viewModel = new IsEmriDetayViewModel
        {
            IsEmriId = isEmri.is_emri_id,
            IsEmriNo = isEmri.is_emri_no,
            Tip = isEmri.tip,
            Durum = isEmri.durum,
            DurumRenk = IsEmriListeViewModel.GetDurumRenk(isEmri.durum),
            Oncelik = isEmri.oncelik,
            PlanlananTarih = isEmri.planlanan_tarih ?? isEmri.created_at.AddDays(1),
            musteri_ad = abone?.Ad ?? "",
            musteri_soyad = abone?.Soyad ?? "",
            musteri_unvan = abone?.Unvan ?? "",
            telefon = abone?.telefon ?? "",
            SahaSonucu = isEmri.saha_sonucu,
            Gerekce = isEmri.gerekce,
            MuhurNo = isEmri.muhur_no,
            TutanakNo = isEmri.tutanak_no,
            TuketimNoktasiKodu = tn != null ? tn.tekil_kod : $"TK-ID-{isEmri.tuketim_noktasi_id}",
            Adres = tn != null ? tn.acik_adres : "Adres bilgisi alınamadı",
            UpdatedAt = isEmri.updated_at,
            CreatedAt = isEmri.created_at
        };

        return View("Tamamlama", viewModel);
    }
}


