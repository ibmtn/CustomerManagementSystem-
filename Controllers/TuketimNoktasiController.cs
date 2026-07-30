using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.ViewModels;
using System.Threading.Tasks;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,Denetci ")]    
    public class TuketimNoktasiController : Controller
    {
        private readonly ITuketimNoktasiService _tuketimNoktasiService;
        private readonly ISayacService _sayacService;
        private readonly ISozlesmeService _sozlesmeService;
        private readonly IIsEmriService _isEmriService;
        private readonly IEndeksOkumaService _endeksOkumaService;
        private readonly IAboneService _aboneService;
        private readonly IAuditLogService _auditLogService;

        public TuketimNoktasiController(
            ITuketimNoktasiService tuketimNoktasiService,
            ISayacService sayacService,
            ISozlesmeService sozlesmeService,
            IIsEmriService isEmriService,
            IEndeksOkumaService endeksOkumaService,
            IAboneService aboneService,
            IAuditLogService auditLogService)
        {
            _tuketimNoktasiService = tuketimNoktasiService;
            _sayacService = sayacService;
            _sozlesmeService = sozlesmeService;
            _isEmriService = isEmriService;
            _endeksOkumaService = endeksOkumaService;
            _aboneService = aboneService;
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index(TuketimNoktasiListeViewModel filtre)
        {
            var data = (await _tuketimNoktasiService.GetAllAsync()).AsQueryable();

            if (!string.IsNullOrEmpty(filtre.FiltreTekilKod))
                data = data.Where(x => x.tekil_kod != null && x.tekil_kod.Contains(filtre.FiltreTekilKod, StringComparison.OrdinalIgnoreCase));

            if (filtre.FiltreIlceId.HasValue)
                data = data.Where(x => x.ilce_id == filtre.FiltreIlceId.Value);

            if (!string.IsNullOrEmpty(filtre.FiltreTuketiciGrubu))
                data = data.Where(x => x.tuketici_grubu != null && x.tuketici_grubu.Equals(filtre.FiltreTuketiciGrubu, StringComparison.OrdinalIgnoreCase));

            var dataList = data.OrderByDescending(x => x.tuketim_noktasi_id).ToList();
            int totalItems = dataList.Count;
            
            filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
            filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;
            
            var pagedData = dataList.Skip((filtre.CurrentPage - 1) * filtre.PageSize).Take(filtre.PageSize).ToList();

            var viewModels = pagedData.Select(item => new KcetasWeb.ViewModels.TuketimNoktasiViewModels
            {
                TuketimNoktasiId = item.tuketim_noktasi_id,
                tekil_kod = item.tekil_kod,
                baglanti_gucu_kw = item.baglanti_gucu_kw,
                ilce_id = item.ilce_id,
                ilce_adi = item.ilce_id switch
                {
                    1 => "Melikgazi", 2 => "Kocasinan", 3 => "Talas", 4 => "Akkışla", 5 => "Bünyan",
                    6 => "Develi", 7 => "Felahiye", 8 => "Hacılar", 9 => "İncesu", 10 => "Özvatan",
                    11 => "Pınarbaşı", 12 => "Sarıoğlan", 13 => "Sarız", 14 => "Tomarza", 15 => "Yahyalı",
                    16 => "Yeşilhisar", 99 => "Merkez İlçe", _ => "Bilinmeyen İlçe"
                },
                bina_no = item.bina_no,
                bagimsiz_bolum_no = item.bagimsiz_bolum_no,
                tuketici_grubu = item.tuketici_grubu,
                status = item.status
            }).ToList();

            filtre.TotalItems = totalItems;
            filtre.TuketimNoktalari = viewModels;

            return View(filtre);
        }

        public IActionResult Yeni()
        {
            return View(new TuketimNoktasiViewModels
            {
                tuketici_grubu = "MESKEN",
                baglanti_durumu = KcetasWeb.Models.Enums.BaglantiDurumu.Pasif
            });
        }

        [HttpPost]
        public async Task<IActionResult> Yeni(KcetasWeb.ViewModels.TuketimNoktasiViewModels model)
        {
            model.mahalle = model.mahalle?.Trim();
            model.bina_no = model.bina_no?.Trim();
            model.bagimsiz_bolum_no = model.bagimsiz_bolum_no?.Trim();
            model.acik_adres = model.acik_adres?.Trim();
            model.tuketici_grubu = NormalizeTuketiciGrubu(model.tuketici_grubu);

            if (model.ilce_id <= 0)
                ModelState.AddModelError(nameof(model.ilce_id), "Lütfen ilçe seçiniz.");
            if (string.IsNullOrWhiteSpace(model.mahalle))
                ModelState.AddModelError(nameof(model.mahalle), "Mahalle alanı zorunludur.");
            if (string.IsNullOrWhiteSpace(model.acik_adres))
                ModelState.AddModelError(nameof(model.acik_adres), "Açık adres alanı zorunludur.");
            if (model.baglanti_gucu_kw <= 0)
                ModelState.AddModelError(nameof(model.baglanti_gucu_kw), "Bağlantı gücü 0'dan büyük olmalıdır.");
            if (string.IsNullOrWhiteSpace(model.tuketici_grubu))
                ModelState.AddModelError(nameof(model.tuketici_grubu), "Lütfen tüketici grubu seçiniz.");

            var allData = await _tuketimNoktasiService.GetAllAsync();
            
            if (model.koordinat_lat != 0 && model.koordinat_lot != 0 &&
                allData.Any(x => x.koordinat_lat == model.koordinat_lat && x.koordinat_lot == model.koordinat_lot))
            {
                ModelState.AddModelError("koordinat_lat", "HATA: Bu koordinatlara sahip başka bir tüketim noktası sistemde zaten mevcut! Lütfen farklı koordinatlar girin.");
            }

            var aboneler = await _aboneService.GetAllAsync();
            if (!string.IsNullOrEmpty(model.tckn) && aboneler.Any(a => a.tckn == model.tckn))
                ModelState.AddModelError("tckn", "HATA: Bu TC Kimlik Numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.vkn) && aboneler.Any(a => a.vkn == model.vkn))
                ModelState.AddModelError("vkn", "HATA: Bu Vergi Kimlik Numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.telefon) && aboneler.Any(a => a.telefon == model.telefon))
                ModelState.AddModelError("telefon", "HATA: Bu telefon numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.e_posta) && aboneler.Any(a => a.e_posta == model.e_posta))
                ModelState.AddModelError("e_posta", "HATA: Bu e-posta adresi sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int maxId = allData.Any() ? (int)allData.Max(x => x.tuketim_noktasi_id) : 0;
            
            var yeniNokta = new TuketimNoktasi
            {
                tuketim_noktasi_id = maxId + 1,
                tekil_kod = $"TK-2026-{(maxId + 1).ToString().PadLeft(3, '0')}",
                ilce_id = model.ilce_id,
                mahalle = model.mahalle,
                bina_no = model.bina_no,
                bagimsiz_bolum_no = model.bagimsiz_bolum_no,
                acik_adres = model.acik_adres,
                baglanti_gucu_kw = model.baglanti_gucu_kw,
                koordinat_lat = model.koordinat_lat == 0 ? null : model.koordinat_lat,
                koordinat_lot = model.koordinat_lot == 0 ? null : model.koordinat_lot,
                tuketici_grubu = model.tuketici_grubu,
                baglanti_durumu = model.baglanti_durumu ?? KcetasWeb.Models.Enums.BaglantiDurumu.Pasif,
                status = "Pasif"
            };

            try
            {
                await _tuketimNoktasiService.CreateAsync(yeniNokta);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Tüketim noktası kayıt API hatası: " + ex);
                ModelState.AddModelError(string.Empty, "Tüketim noktası API tarafında kaydedilemedi. API/veritabanı kayıt işlemindeki inner exception incelenmelidir.");
                return View(model);
            }

            await _auditLogService.EkleAsync(
                varlikTipi: "TuketimNoktasi",
                varlikId: yeniNokta.tuketim_noktasi_id,
                islemTipi: "INSERT",
                eskiDeger: "Yok",
                yeniDeger: yeniNokta.tekil_kod,
                kullaniciId: 1, // Mock
                islemGerekcesi: "Yeni tüketim noktası oluşturuldu."
            );

            // Abone bilgilerini ayır ve API'ye gönder
            if (!string.IsNullOrEmpty(model.tckn) || !string.IsNullOrEmpty(model.vkn))
            {
                var abone = new Abone
                {
                    tckn = model.tckn,
                    vkn = model.vkn,
                    telefon = model.telefon ?? "0000000000",
                    e_posta = model.e_posta,
                    abone_tipi = !string.IsNullOrEmpty(model.vkn) ? KcetasWeb.Models.Enums.AboneTipi.Kurumsal : KcetasWeb.Models.Enums.AboneTipi.Bireysel,
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    Unvan = model.Unvan
                };
                
                try 
                {
                    await _aboneService.CreateAsync(abone);
                    
                    // Eklenen abonenin ID'sini bulmak için listeyi çekiyoruz
                    var guncelAboneler = await _aboneService.GetAllAsync();
                    var eklenenAbone = guncelAboneler.OrderByDescending(a => a.abone_id).FirstOrDefault(a => (a.tckn == model.tckn && !string.IsNullOrEmpty(model.tckn)) || (a.vkn == model.vkn && !string.IsNullOrEmpty(model.vkn)));

                    if (eklenenAbone != null)
                    {
                        await _auditLogService.EkleAsync(
                            varlikTipi: "Abone",
                            varlikId: eklenenAbone.abone_id,
                            islemTipi: "INSERT",
                            eskiDeger: "Yok",
                            yeniDeger: $"{model.Ad} {model.Soyad} {model.Unvan}".Trim(),
                            kullaniciId: 1, // Mock
                            islemGerekcesi: "Tüketim noktası oluşturulurken otomatik yeni abone kaydı oluşturuldu."
                        );
                        
                        // Müşteri isteği: Tüketim noktası ile birlikte otomatik sözleşme oluşmayacak.
                        // Sözleşme işlemi Sözleşmeler sayfasından manuel yapılacaktır.
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Abone/Sözleşme kaydı API hatası: " + ex.Message);
                    // Tüketim noktası kaydedildiği için devam ediyoruz
                }
            }

            TempData["BasariMesaji"] = "Harika! Yeni tüketim noktası ve abone başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        private static string NormalizeTuketiciGrubu(string tuketiciGrubu)
        {
            return tuketiciGrubu?.Trim() switch
            {
                "Mesken" or "MESKEN" => "MESKEN",
                "Ticarethane" or "TICARETHANE" or "TİCARETHANE" => "TICARETHANE",
                "Sanayi" or "SANAYI" or "SANAYİ" => "SANAYI",
                "TarimsalSulama" or "Tarımsal Sulama" or "TARIMSAL SULAMA" or "TARIMSAL_SULAMA" => "TARIMSAL SULAMA",
                "Aydinlatma" or "Aydınlatma" or "AYDINLATMA" => "AYDINLATMA",
                var value => value ?? string.Empty
            };
        }

        public async Task<IActionResult> Detay(string id)
        {
            var item = await _tuketimNoktasiService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new KcetasWeb.ViewModels.TuketimNoktasiViewModels
            {
                TuketimNoktasiId = item.tuketim_noktasi_id,
                tekil_kod = item.tekil_kod,
                ilce_id = item.ilce_id,
                mahalle = item.mahalle,
                bina_no = item.bina_no,
                bagimsiz_bolum_no = item.bagimsiz_bolum_no,
                acik_adres = item.acik_adres,
                baglanti_gucu_kw = item.baglanti_gucu_kw,
                koordinat_lat = item.koordinat_lat ?? 0m,
                koordinat_lot = item.koordinat_lot ?? 0m,
                tuketici_grubu = item.tuketici_grubu,
                baglanti_durumu = item.baglanti_durumu,
                status = item.status
            };

            // İlişkili verilerin çekilmesi
            var sayaclar = (await _sayacService.GetAllAsync()).Where(s => s.tuketim_noktasi_id == item.tuketim_noktasi_id).ToList();
            ViewBag.Sayaclar = sayaclar;
            var sozlesmeler = (await _sozlesmeService.GetAllAsync()).Where(s => s.tuketim_noktasi_id == item.tuketim_noktasi_id).ToList();
            ViewBag.Sozlesmeler = sozlesmeler;
            ViewBag.IsEmirleri = (await _isEmriService.GetAllAsync()).Where(i => i.tuketim_noktasi_id == item.tuketim_noktasi_id).OrderByDescending(i => i.created_at).ToList();

            var aktifSozlesme = sozlesmeler.OrderByDescending(s => s.baslangic_tarihi).FirstOrDefault(s => s.durum == KcetasWeb.Models.Enums.SozlesmeDurumu.Aktif);
            if (aktifSozlesme != null)
            {
                var abone = await _aboneService.GetByIdAsync((int)aktifSozlesme.abone_id);
                if (abone != null)
                {
                    viewModel.Ad = abone.Ad;
                    viewModel.Soyad = abone.Soyad;
                    viewModel.Unvan = abone.Unvan;
                    viewModel.tckn = abone.tckn;
                    viewModel.vkn = abone.vkn;
                    viewModel.telefon = abone.telefon;
                    viewModel.e_posta = abone.e_posta;
                }
            }
            
            var sayacIds = sayaclar.Select(s => s.sayac_id).ToList();
            ViewBag.Endeksler = (await _endeksOkumaService.GetAllAsync())
                .Where(e => e.sayac_id.HasValue && sayacIds.Contains(e.sayac_id.Value))
                .OrderByDescending(e => e.okuma_zamani)
                .ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Duzenle(string id)
        {
            var item = await _tuketimNoktasiService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Duzenle(TuketimNoktasi model)
        {
            var allData = await _tuketimNoktasiService.GetAllAsync();
            if (model.koordinat_lat.HasValue && model.koordinat_lot.HasValue &&
                allData.Any(x => x.koordinat_lat == model.koordinat_lat && x.koordinat_lot == model.koordinat_lot && x.tekil_kod != model.tekil_kod))
            {
                ModelState.AddModelError("koordinat_lat", "HATA: Bu koordinatlara sahip başka bir tüketim noktası sistemde zaten mevcut! Lütfen farklı koordinatlar girin.");
                return View(model);
            }

            var item = await _tuketimNoktasiService.GetByIdAsync(model.tekil_kod);
            if (item != null)
            {
                // Abone bilgileri UI'dan kaldırıldığı için burada güncellenmemeli.
                item.ilce_id = model.ilce_id;
                
                item.mahalle = string.IsNullOrWhiteSpace(model.mahalle) ? "Bilinmiyor" : model.mahalle;
                item.bina_no = model.bina_no;
                item.bagimsiz_bolum_no = model.bagimsiz_bolum_no;
                item.acik_adres = string.IsNullOrWhiteSpace(model.acik_adres) ? "Belirtilmemiş" : model.acik_adres;
                item.baglanti_gucu_kw = model.baglanti_gucu_kw;
                item.koordinat_lat = model.koordinat_lat;
                item.koordinat_lot = model.koordinat_lot;
                item.tuketici_grubu = string.IsNullOrWhiteSpace(model.tuketici_grubu) ? "Mesken" : model.tuketici_grubu;
                item.baglanti_durumu = model.baglanti_durumu ?? KcetasWeb.Models.Enums.BaglantiDurumu.Pasif;
                item.status = string.IsNullOrWhiteSpace(model.status) ? "Pasif" : model.status;
                item.updated_at = DateTime.Now;

                await _tuketimNoktasiService.UpdateAsync(item);
                
                await _auditLogService.EkleAsync(
                    varlikTipi: "TuketimNoktasi",
                    varlikId: item.tuketim_noktasi_id,
                    islemTipi: "UPDATE",
                    eskiDeger: "Eski Bilgiler",
                    yeniDeger: "Güncellenmiş Bilgiler",
                    kullaniciId: 1, // Mock
                    islemGerekcesi: "Tüketim noktası bilgileri güncellendi."
                );
            }
            TempData["BasariMesaji"] = model.tekil_kod + " kodlu nokta başarıyla güncellendi.";
            return RedirectToAction("Detay", new { id = model.tekil_kod });
        }
    }
}
