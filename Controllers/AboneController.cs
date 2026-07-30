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

        public AboneController(IAboneService aboneService, IAuditLogService auditLogService)
        {
            _aboneService = aboneService;
            _auditLogService = auditLogService;
        }

        public async System.Threading.Tasks.Task<IActionResult> Index(AboneListeViewModel filtre)
        {
            filtre.CurrentPage = filtre.CurrentPage > 0 ? filtre.CurrentPage : 1;
            filtre.PageSize = filtre.PageSize > 0 ? filtre.PageSize : 50;

            var pagedResponse = await _aboneService.GetPagedAsync(filtre.CurrentPage, filtre.PageSize);
            var aboneler = pagedResponse.Data.AsQueryable();

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

            var aboneList = aboneler.ToList();
            
            filtre.TotalItems = pagedResponse.TotalCount;
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
            NormalizeAboneModel(model);
            ValidateKimlikAlanlari(model);

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
                    telefon = model.Telefon,
                    e_posta = model.Mail,

                    Durum = "Aktif",
                    CreatedAt = DateTime.Now
                };

                if (model.IsTuzel)
                {
                    yeniAbone.Unvan = model.AdSoyadUnvan;
                    yeniAbone.vkn = model.VKN ?? "";
                    yeniAbone.Ad = "";
                    yeniAbone.Soyad = "";
                    yeniAbone.tckn = "";
                }
                else
                {
                    var nameParts = model.AdSoyadUnvan.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    yeniAbone.Soyad = nameParts.Length > 1 ? nameParts.Last() : "";
                    yeniAbone.Ad = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : model.AdSoyadUnvan;
                    yeniAbone.tckn = model.TCKN ?? "";
                    yeniAbone.vkn = "";
                    yeniAbone.Unvan = "";
                }

                try
                {
                    await _aboneService.CreateAsync(yeniAbone);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Abone kaydedilemedi. API detayı: {ex.Message}");
                    return View(model);
                }

                // API BUG WORKAROUND: Harici API, yeni kayıt (POST) sırasında e-posta adresini yoksayıyor.
                // Ancak güncelleme (PUT) işleminde e-postayı başarıyla kaydediyor.
                // Bu yüzden yeni eklenen aboneyi bulup hemen arkasından bir Update (PUT) isteği atarak e-postayı zorla kaydediyoruz.
                var createdAbone = (await _aboneService.GetAllAsync()).OrderByDescending(a => a.abone_id).FirstOrDefault(a => a.telefon == model.Telefon);
                int yeniId = 1;
                string emailUpdateWarning = null;
                if (createdAbone != null)
                {
                    yeniId = createdAbone.abone_id;
                    if (!string.IsNullOrEmpty(model.Mail))
                    {
                        try
                        {
                            createdAbone.e_posta = model.Mail;
                            await _aboneService.UpdateAsync(createdAbone);
                        }
                        catch (Exception ex)
                        {
                            emailUpdateWarning = $"Abone oluşturuldu fakat e-posta güncellenemedi. API detayı: {ex.Message}";
                        }
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

                TempData["Mesaj"] = emailUpdateWarning ?? "Müşteri / Abone başarıyla eklendi!";
                TempData["MesajTip"] = emailUpdateWarning == null ? "success" : "warning";
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
                KimlikNoMaskeli = (abone.abone_tipi == KcetasWeb.Models.Enums.AboneTipi.Bireysel) ? Maskele(abone.tckn) : Maskele(abone.vkn)
                // Diğer sekmelerdeki Sözleşmeler, İş Emirleri, Tüketim Noktaları mocklanmış şekilde sayfa içinde foreach döngüsü boş kalacak şekilde bırakıldı, istendiğinde servisten çekilecek.
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
            NormalizeAboneModel(model);
            ValidateKimlikAlanlari(model);

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

                try
                {
                    await _aboneService.UpdateAsync(abone);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Abone güncellenemedi. API detayı: {ex.Message}");
                    ViewBag.AboneId = id;
                    return View("Duzenle", model);
                }

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
            await _aboneService.DeleteAsync(id);

            await _auditLogService.EkleAsync(
                varlikTipi: "Abone",
                varlikId: id,
                islemTipi: "DELETE",
                eskiDeger: "Sistemde Kayıtlı Abone",
                yeniDeger: "SİLİNDİ",
                kullaniciId: 1, // Mock User ID
                islemGerekcesi: "Abone sistemden silindi."
            );

            TempData["Mesaj"] = "Abone başarıyla silindi.";
            TempData["MesajTip"] = "success";
            return RedirectToAction("Index");
        }

        private string Maskele(string deger)
        {
            if (string.IsNullOrWhiteSpace(deger) || deger.Length < 5) return "***";
            return deger.Substring(0, 3) + new string('*', deger.Length - 5) + deger.Substring(deger.Length - 2);
        }

        private void NormalizeAboneModel(AboneEkleViewModel model)
        {
            model.AdSoyadUnvan = model.AdSoyadUnvan?.Trim();
            model.Telefon = model.Telefon?.Trim();
            model.Mail = model.Mail?.Trim();
            model.TCKN = DigitsOnly(model.TCKN);
            model.VKN = DigitsOnly(model.VKN);

            if (model.IsTuzel)
            {
                model.TCKN = string.Empty;
            }
            else
            {
                model.VKN = string.Empty;
            }
        }

        private void ValidateKimlikAlanlari(AboneEkleViewModel model)
        {
            if (model.IsTuzel)
            {
                if (string.IsNullOrWhiteSpace(model.VKN))
                    ModelState.AddModelError("VKN", "Vergi Kimlik Numarası zorunludur.");
                else if (model.VKN.Length != 10)
                    ModelState.AddModelError("VKN", "Vergi Kimlik Numarası 10 haneli olmalıdır.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.TCKN))
                    ModelState.AddModelError("TCKN", "TC Kimlik Numarası zorunludur.");
                else if (model.TCKN.Length != 11)
                    ModelState.AddModelError("TCKN", "TC Kimlik Numarası 11 haneli olmalıdır.");
            }
        }

        private static string DigitsOnly(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return new string(value.Where(char.IsDigit).ToArray());
        }
    }
}
