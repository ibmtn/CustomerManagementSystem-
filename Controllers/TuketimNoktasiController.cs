using Microsoft.AspNetCore.Mvc;

namespace KcetasWeb.Controllers
{
    public class TuketimNoktasiController : Controller
    {
        // 1. Liste Ekranı (Sayfa ilk açıldığında çalışır)
        public IActionResult Index()
        {
            return View();
        }

        // 2. Yeni Kayıt Formu (Sayfa ilk açıldığında GET olarak çalışır)
        public IActionResult Yeni()
        {
            return View();
        }

        // 3. FORMDAN GELEN VERİYİ YAKALAYAN METOT (POST işlemi)
        [HttpPost]
        public IActionResult Yeni(string TuketiciGrubu, string BaglantiGucu, string AcikAdres)
        {
            // İleride veritabanı (backend API) ekibi işi bitirdiğinde 
            // bu yakaladığımız verileri onların veritabanına göndereceğiz.
            
            // Şimdilik işlemi başarılı sayıp, ekrana JavaScript KULLANMADAN
            // mesaj basmak için TempData kullanıyoruz:
            TempData["BasariMesaji"] = "Harika! " + TuketiciGrubu + " grubundaki yeni kayıt başarıyla oluşturuldu.";

            // Kayıt bitince kullanıcıyı tekrar listeye (Index'e) geri yolla:
            return RedirectToAction("Index");
        }
    }
}