using Microsoft.AspNetCore.Mvc;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using KcetasWeb.ViewModels;
using System;

namespace KcetasWeb.Controllers
{
    [Authorize(Roles = "BTYoneticisi,MusteriTemsilcisi,SozlesmeYetkilisi,Denetci")]
    public class AboneController : Controller
    {
        private readonly IAboneService _aboneService;
        private readonly IAuditLogService _auditLogService;
        private readonly ISozlesmeService _sozlesmeService;

        public AboneController(IAboneService aboneService, IAuditLogService auditLogService, ISozlesmeService sozlesmeService)
        {
            _aboneService = aboneService;
            _auditLogService = auditLogService;
            _sozlesmeService = sozlesmeService;
        }

        public async System.Threading.Tasks.Task<IActionResult> Index(AboneListeViewModel filtre)
        {
            filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
            filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;

            var tumAboneler = await _aboneService.GetAllAsync();
            
            // Son eklenenlerin en üstte gözükmesi için id'ye göre ters sıralayalım
            var aboneler = tumAboneler.OrderByDescending(x => x.abone_id).AsQueryable();

            if (!string.IsNullOrEmpty(filtre.FiltreTCKNVKN))
                aboneler = aboneler.Where(x => (x.tckn != null && x.tckn.Contains(filtre.FiltreTCKNVKN)) || (x.vkn != null && x.vkn.Contains(filtre.FiltreTCKNVKN)));

            if (!string.IsNullOrEmpty(filtre.FiltreAdSoyadUnvan))
                aboneler = aboneler.Where(x => 
                    ($"{x.Ad} {x.Soyad}").Contains(filtre.FiltreAdSoyadUnvan, StringComparison.CurrentCultureIgnoreCase) || 
                    (x.Unvan != null && x.Unvan.Contains(filtre.FiltreAdSoyadUnvan, StringComparison.CurrentCultureIgnoreCase)));

            if (!string.IsNullOrEmpty(filtre.FiltreAboneTipi))
            {
                var queryTipi = filtre.FiltreAboneTipi.ToUpper();
                if (queryTipi == "BIREYSEL" || queryTipi == "GERÇEK")
                    aboneler = aboneler.Where(x => x.abone_tipi == KcetasWeb.Models.Enums.AboneTipi.Bireysel);
                else if (queryTipi == "KURUMSAL" || queryTipi == "TÜZEL")
                    aboneler = aboneler.Where(x => x.abone_tipi == KcetasWeb.Models.Enums.AboneTipi.Kurumsal);
            }

            filtre.TotalItems = aboneler.Count();

            var aboneList = aboneler
                .Skip((filtre.CurrentPage - 1) * filtre.PageSize)
                .Take(filtre.PageSize)
                .ToList();
            
            filtre.Aboneler = aboneList.Select(a => new AboneSatirViewModel
            {
                AboneId = a.abone_id,
                AboneNo = a.abone_no,
                AdSoyad = (a.abone_tipi == KcetasWeb.Models.Enums.AboneTipi.Bireysel) ? $"{a.Ad} {a.Soyad}" : a.Unvan ?? "",
                KimlikNoMaskeli = (a.abone_tipi == KcetasWeb.Models.Enums.AboneTipi.Bireysel) ? Maskele(a.tckn) : Maskele(a.vkn),
                Telefon = a.telefon,
                Mail = string.IsNullOrWhiteSpace(a.e_posta) ? "(Belirtilmemiş)" : a.e_posta,
                EPostaApi = string.IsNullOrWhiteSpace(a.e_posta) ? "(Belirtilmemiş)" : a.e_posta,
                AboneTipi = a.abone_tipi,
                Durum = a.Durum
            }).ToList();

            return View(filtre);
        }



        [HttpGet]
        public IActionResult Yeni()
        {
            return View(new AboneEkleViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Yeni(AboneEkleViewModel model)
        {
            var tumAboneler = await _aboneService.GetAllAsync();

            if (!string.IsNullOrEmpty(model.TCKN) && tumAboneler.Any(a => a.tckn == model.TCKN))
                ModelState.AddModelError("TCKN", "HATA: Bu TC Kimlik Numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.VKN) && tumAboneler.Any(a => a.vkn == model.VKN))
                ModelState.AddModelError("VKN", "HATA: Bu Vergi Kimlik Numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.Telefon) && tumAboneler.Any(a => a.telefon == model.Telefon))
                ModelState.AddModelError("Telefon", "HATA: Bu telefon numarası sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.Mail) && tumAboneler.Any(a => a.e_posta == model.Mail))
                ModelState.AddModelError("Mail", "HATA: Bu e-posta adresi sistemde zaten kayıtlı! Lütfen farklı bir değer girin.");
            
            // Abone isminin aynısı var mı kontrolü (Küçük-büyük harf duyarsız)
            var temizAdSoyad = (model.AdSoyadUnvan ?? "").Trim().ToLower();
            if (tumAboneler.Any(a => (a.Ad + " " + a.Soyad).Trim().ToLower() == temizAdSoyad || (a.Unvan ?? "").Trim().ToLower() == temizAdSoyad))
            {
                ModelState.AddModelError("AdSoyadUnvan", "HATA: Bu Abone Adı/Ünvanı sistemde zaten kayıtlı! Lütfen farklı bir isim girin veya mevcut abone kaydını kullanın.");
            }

            if (ModelState.IsValid)
            {
                var yeniAbone = new Abone
                {
                    abone_tipi = model.IsTuzel ? KcetasWeb.Models.Enums.AboneTipi.Kurumsal : KcetasWeb.Models.Enums.AboneTipi.Bireysel,
                    telefon = model.Telefon ?? "",
                    e_posta = model.Mail ?? "",

                    Durum = "AKTIF",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    abone_no = "ABN-" + DateTime.Now.ToString("yyyyMMddHHmmss")
                };

                if (model.IsTuzel)
                {
                    yeniAbone.Unvan = model.AdSoyadUnvan;
                    yeniAbone.vkn = string.IsNullOrWhiteSpace(model.VKN) ? null : model.VKN;
                    yeniAbone.Ad = "";
                    yeniAbone.Soyad = "";
                    yeniAbone.tckn = null;
                }
                else
                {
                    var nameParts = model.AdSoyadUnvan.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    yeniAbone.Soyad = nameParts.Length > 1 ? nameParts.Last() : "";
                    yeniAbone.Ad = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : model.AdSoyadUnvan;
                    yeniAbone.tckn = string.IsNullOrWhiteSpace(model.TCKN) ? null : model.TCKN;
                    yeniAbone.vkn = null;
                    yeniAbone.Unvan = "";
                }

                await _aboneService.CreateAsync(yeniAbone);

                // API BUG WORKAROUND: Harici API, yeni kayıt (POST) sırasında e-posta adresini yoksayıyor.
                // Ancak güncelleme (PUT) işleminde e-postayı başarıyla kaydediyor.
                // Bu yüzden yeni eklenen aboneyi bulup hemen arkasından bir Update (PUT) isteği atarak e-postayı zorla kaydediyoruz.
                var createdAbone = (await _aboneService.GetAllAsync()).OrderByDescending(a => a.abone_id).FirstOrDefault(a => a.telefon == model.Telefon);
                int yeniId = 1;
                if (createdAbone != null)
                {
                    yeniId = createdAbone.abone_id;
                    if (!string.IsNullOrEmpty(model.Mail))
                    {
                        createdAbone.e_posta = model.Mail;
                        await _aboneService.UpdateAsync(createdAbone);
                    }
                }
                
                await _auditLogService.EkleAsync(
                    varlikTipi: "Abone",
                    varlikId: yeniId,
                    islemTipi: "INSERT",
                    eskiDeger: "Yok",
                    yeniDeger: model.AdSoyadUnvan,
                    kullaniciId: 1, // Mock User ID
                    islemGerekcesi: "Yeni abone kaydı oluşturuldu."
                );

                TempData["Mesaj"] = "Müşteri / Abone başarıyla eklendi!";
                TempData["MesajTip"] = "success";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var abone = await _aboneService.GetByIdAsync(id);
            if (abone == null) return NotFound();

            var viewModel = new AboneDetayViewModel
            {
                Abone = abone,
                KimlikNoMaskeli = (abone.abone_tipi == KcetasWeb.Models.Enums.AboneTipi.Bireysel) ? Maskele(abone.tckn) : Maskele(abone.vkn),
                Sozlesmeler = (await _sozlesmeService.GetAllAsync()).Where(s => s.abone_id == id).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Duzenle(int id)
        {
            var abone = await _aboneService.GetByIdAsync(id);
            if (abone == null) return NotFound();

            var model = new AboneEkleViewModel
            {
                IsTuzel = abone.abone_tipi == KcetasWeb.Models.Enums.AboneTipi.Kurumsal,
                AdSoyadUnvan = abone.abone_tipi == KcetasWeb.Models.Enums.AboneTipi.Kurumsal ? abone.Unvan : $"{abone.Ad} {abone.Soyad}",
                TCKN = abone.tckn,
                VKN = abone.vkn,
                Telefon = abone.telefon,
                Mail = abone.e_posta,
                KvkkOnayi = true
            };
            ViewBag.AboneId = id;

            return View("Duzenle", model);
        }

        [HttpPost]
        public async Task<IActionResult> Duzenle(int id, AboneEkleViewModel model)
        {
            var tumAboneler = await _aboneService.GetAllAsync();

            if (!string.IsNullOrEmpty(model.TCKN) && tumAboneler.Any(a => a.tckn == model.TCKN && a.abone_id != id))
                ModelState.AddModelError("TCKN", "HATA: Bu TC Kimlik Numarası sistemde başka bir abonede kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.VKN) && tumAboneler.Any(a => a.vkn == model.VKN && a.abone_id != id))
                ModelState.AddModelError("VKN", "HATA: Bu Vergi Kimlik Numarası sistemde başka bir abonede kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.Telefon) && tumAboneler.Any(a => a.telefon == model.Telefon && a.abone_id != id))
                ModelState.AddModelError("Telefon", "HATA: Bu telefon numarası sistemde başka bir abonede kayıtlı! Lütfen farklı bir değer girin.");
            if (!string.IsNullOrEmpty(model.Mail) && tumAboneler.Any(a => a.e_posta == model.Mail && a.abone_id != id))
                ModelState.AddModelError("Mail", "HATA: Bu e-posta adresi sistemde başka bir abonede kayıtlı! Lütfen farklı bir değer girin.");

            if (ModelState.IsValid)
            {
                var abone = await _aboneService.GetByIdAsync(id);
                if (abone == null) return NotFound();

                abone.abone_tipi = model.IsTuzel ? KcetasWeb.Models.Enums.AboneTipi.Kurumsal : KcetasWeb.Models.Enums.AboneTipi.Bireysel;
                abone.telefon = model.Telefon;
                abone.e_posta = model.Mail;


                if (model.IsTuzel)
                {
                    abone.Unvan = model.AdSoyadUnvan;
                    abone.vkn = model.VKN ?? "";
                    abone.Ad = "";
                    abone.Soyad = "";
                    abone.tckn = "";
                }
                else
                {
                    var nameParts = model.AdSoyadUnvan.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    abone.Soyad = nameParts.Length > 1 ? nameParts.Last() : "";
                    abone.Ad = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : model.AdSoyadUnvan;
                    abone.tckn = model.TCKN ?? "";
                    abone.vkn = "";
                    abone.Unvan = "";
                }

                await _aboneService.UpdateAsync(abone);

                await _auditLogService.EkleAsync(
                    varlikTipi: "Abone",
                    varlikId: id,
                    islemTipi: "UPDATE",
                    eskiDeger: "Bilinmiyor",
                    yeniDeger: model.AdSoyadUnvan,
                    kullaniciId: 1, // Mock User ID
                    islemGerekcesi: "Abone bilgileri güncellendi."
                );

                TempData["Mesaj"] = "Müşteri / Abone başarıyla güncellendi!";
                TempData["MesajTip"] = "success";
                return RedirectToAction("Index");
            }
            ViewBag.AboneId = id;
            return View("Duzenle", model);
        }

        [HttpPost]
        public async Task<IActionResult> Sil(int id)
        {
            try 
            {
                await _aboneService.DeleteAsync(id);

                await _auditLogService.EkleAsync(
                    varlikTipi: "Abone",
                    varlikId: id,
                    islemTipi: "DELETE",
                    eskiDeger: "Sistemde Kayıtlı Abone",
                    yeniDeger: "SİLİNDİ/PASİF",
                    kullaniciId: 1, // Mock User ID
                    islemGerekcesi: "Abone sistemden silindi (pasife alındı)."
                );

                TempData["Mesaj"] = "Abone başarıyla pasife alındı (silindi).";
                TempData["MesajTip"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Mesaj"] = "Hata: Abone silinemedi. " + (ex.Message.Contains("sözleşme") ? "Bu aboneye ait sözleşme bulunduğu için silinemez!" : ex.Message);
                TempData["MesajTip"] = "danger";
                var referer = Request.Headers["Referer"].ToString();
                return Redirect(string.IsNullOrEmpty(referer) ? "/Abone/Index" : referer);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DurumGuncelle(int id, string yeniDurum)
        {
            try
            {
                var abone = await _aboneService.GetByIdAsync(id);
                if (abone == null) return NotFound();

                abone.Durum = yeniDurum;
                abone.UpdatedAt = DateTime.Now;

                await _aboneService.UpdateAsync(abone);
                
                TempData["Mesaj"] = $"Abone durumu başarıyla '{yeniDurum}' olarak güncellendi.";
                TempData["MesajTip"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mesaj"] = $"Hata: Durum güncellenemedi. Detay: {ex.Message}";
                TempData["MesajTip"] = "danger";
            }
            
            return RedirectToAction("Detay", new { id });
        }

        private string Maskele(string deger)
        {
            if (string.IsNullOrWhiteSpace(deger) || deger.Length < 5) return "***";
            return deger.Substring(0, 3) + new string('*', deger.Length - 5) + deger.Substring(deger.Length - 2);
        }
    }
}