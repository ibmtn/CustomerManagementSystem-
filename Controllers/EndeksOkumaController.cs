using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Services.Interfaces;
using System;
using System.Linq;

namespace KcetasWeb.Controllers
{
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

        public IActionResult Index(string? kaynak, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var okumalar = _endeksOkumaService.Filtrele(kaynak, durum, baslangic, bitis, arama);
            var sozlesmeler = _sozlesmeService.GetAll();
            var aboneler = _aboneService.GetAll();
            var isEmirleri = _isEmriService.GetAll();
            var tuketimNoktalari = _tuketimNoktasiService.GetAll();
            
            var viewModels = okumalar.Select(o => {
                var sozlesme = sozlesmeler.FirstOrDefault(s => s.sozlesme_id == o.sozlesme_id);

                if (sozlesme == null && o.is_emri_id.HasValue)
                {
                    var isEmri = isEmirleri.FirstOrDefault(ie => ie.is_emri_id == o.is_emri_id.Value);
                    if (isEmri != null)
                    {
                        sozlesme = sozlesmeler.FirstOrDefault(s => s.tuketim_noktasi_id == isEmri.tuketim_noktasi_id);
                    }
                }

                if (sozlesme == null || sozlesme.durum != "Aktif" && o.sayac_id.HasValue)
                {
                    var sayaclar = _sayacService.GetAll();
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
                
                return new KcetasWeb.ViewModels.EndeksOkumaViewModels
                {
                    okuma_id = o.okuma_id,
                    sayac_id = o.sayac_id,
                    is_emri_id = o.is_emri_id,
                    sozlesme_id = o.sozlesme_id,
                    okuma_tipi = o.okuma_tipi switch
                    {
                        "SON_OKUMA"=> "Son Okuma",
                        "KESME_ENDEKSI" => "Kesme Endeksi",
                        "SAYAC_DEGISIM_OKUMASI" => "Sayaç Değişim Okuması",
                        "SAYAC_ARIZA_OKUMASI" => "Sayaç Arıza Okuması",
                        "MUHURLEME_ENDEKSI" => "Mühürleme Endeksi",
                        "RUTIN_DONEM" => "Rutin Dönem",
                        "ILK_OKUMA" => "İlk Okuma",
                        "ACILIS" => "Açılış",
                        "OSOS" => "OSOS",
                        _ => o.okuma_tipi ?? "Normal"
                    },
                    okuma_kaynagi = o.okuma_kaynagi switch
                    {
                        "MANUEL" => "Manuel",
                        "MOBIL" => "Mobil",
                        "OSOS" => "OSOS",
                        _ => o.okuma_kaynagi ?? "Bilinmiyor"
                    },
                    onceki_endeks = o.onceki_endeks,
                    yeni_endeks = o.yeni_endeks,
                    okuma_zamani = o.okuma_zamani,
                    kullanici_id = o.kullanici_id,
                    okunamam_nedeni = o.okunamama_nedeni,
                    dogrulama_durumu = o.dogrulama_durumu,
                    anomali_mi = o.anomali_mi,
                    status = o.status,
                    AnomaliAciklamasi = "",
                    sökme_nedeni = "",
                    aciklama = "",
                    son_endeks = o.yeni_endeks ?? 0m,
                    CreatedAt = o.created_at,
                    abone = aboneBilgisi
                };
            }).ToList();

            ViewBag.Istatistikler = _endeksOkumaService.GetIstatistikler();
            return View(viewModels);
        }

        public IActionResult Detay(long id)
        {
            var okuma = _endeksOkumaService.GetById((int)id);
            if (okuma == null) return NotFound();
            return View(okuma);
        }

        [AllowAnonymous]
        public IActionResult TutanakYazdir(long id)
        {
            var okuma = _endeksOkumaService.GetById((int)id);
            if (okuma == null) return NotFound();
            return View(okuma);
        }

        public IActionResult Yeni()
        {
            ViewBag.TuketimNoktalari = _tuketimNoktasiService.GetAll();
            ViewBag.Sayaclar = _sayacService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Yeni(long TuketimNoktasiId, long SayacId, decimal onceki_endeks, decimal yeni_endeks, string okuma_tipi, string okuma_kaynagi, string aciklama)
        {
            // Tüketim miktarını hesapla
            decimal tuketim = yeni_endeks - onceki_endeks;
            if (tuketim < 0) tuketim = 0; // Eğer negatifse (örneğin hatalı okuma veya sayaç sıfırlanması), şimdilik 0 kabul edelim

            // İlgili tüketim noktasına ait sözleşmeyi bul
            var sozlesmeler = _sozlesmeService.GetAll().Where(s => s.tuketim_noktasi_id == TuketimNoktasiId).ToList();
            var aktifSozlesme = sozlesmeler.FirstOrDefault(s => s.durum != "Feshedildi" && s.durum != "İptal") ?? sozlesmeler.FirstOrDefault();
            
            string tarifeGrubu = aktifSozlesme != null ? 
                (aktifSozlesme.tarife_id == 1 ? "Mesken" : 
                 aktifSozlesme.tarife_id == 2 ? "Sanayi" : 
                 aktifSozlesme.tarife_id == 3 ? "Ticarethane" : 
                 aktifSozlesme.tarife_id == 4 ? "Tarımsal Sulama" : "Aydınlatma") : "Mesken";
            
            // Fatura hesaplamasını yap
            var hesaplama = _faturaService.SimulasyonHesapla(tarifeGrubu, tuketim);

            var yeniFatura = new Fatura
            {
                fatura_no = $"FAT-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}", // Geçici olarak random no ürettik
                sozlesme_id = aktifSozlesme?.sozlesme_id ?? 1000,
                tekil_kod = TuketimNoktasiId.ToString(),
                fatura_tipi = tarifeGrubu,
                fatura_tarihi = DateTime.Now,
                son_odeme_tarihi = DateTime.Now.AddDays(15),
                donem = DateTime.Now.ToString("yyyy-MM"),
                ilk_endeks = onceki_endeks,
                son_endeks = yeni_endeks,
                tuketim_kwh = tuketim,
                carpan = 1,
                enerji_bedeli = hesaplama.EnerjiBedeli,
                dagatim_bedeli = hesaplama.DagitimBedeli,
                vergi_fon_toplam = hesaplama.TrtPayi + hesaplama.EnerjiFonu + hesaplama.KdvTutari,
                toplam_tutar = hesaplama.ToplamTutar,
                durum = "Bekliyor",
                status = "Active",
                created_at = DateTime.Now
            };

            _faturaService.Ekle(yeniFatura);

            var yeniOkuma = new EndeksOkuma
            {
                sayac_id = (int)SayacId,
                sozlesme_id = (aktifSozlesme != null && aktifSozlesme.sozlesme_id > 0) ? (int)aktifSozlesme.sozlesme_id : null,
                donem = DateTime.Now.ToString("yyyy-MM"),
                okuma_tipi = okuma_kaynagi == "Otomatik" ? "OSOS" : "Manuel",
                okuma_kaynagi = okuma_kaynagi == "Otomatik" ? "OSOS" : "Manuel",
                onceki_endeks = onceki_endeks,
                yeni_endeks = yeni_endeks,
                okuma_zamani = DateTime.UtcNow,
                kullanici_id = 1,
                dogrulama_durumu = "DOGRULAMA_BEKLIYOR",
                anomali_mi = tuketim > 1000,
                status = "Basarili",
                okunamama_nedeni = "",
                created_at = DateTime.UtcNow
            };

            _endeksOkumaService.Create(yeniOkuma);

            TempData["OkumaMesaji"] = "Endeks okuması alındı ve otomatik olarak yeni fatura oluşturuldu. (Fatura No: " + yeniFatura.fatura_no + " - Tutar: " + yeniFatura.toplam_tutar?.ToString("C2") + ")";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetSonEndeks(long sayacId)
        {
            var okumalar = _endeksOkumaService.GetAll()
                .Where(x => x.sayac_id == sayacId)
                .OrderByDescending(x => x.okuma_zamani)
                .ToList();

            if (okumalar.Any())
            {
                // En son okunan yeni endeks, sıradaki okumanın "önceki_endeksi" olur.
                return Json(new { basarili = true, endeks = okumalar.First().yeni_endeks });
            }

            return Json(new { basarili = false, endeks = 0 });
        }
    }
}
