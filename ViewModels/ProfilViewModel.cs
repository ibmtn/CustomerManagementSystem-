using System.ComponentModel.DataAnnotations;

namespace MusteriYonetim.ViewModels
{
    public class ProfilViewModel
    {
        // Profil Bilgileri
        public string? AdSoyad { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? EPosta { get; set; }

        // Şifre Değiştirme
        [DataType(DataType.Password)]
        public string? EskiSifre { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        public string? YeniSifre { get; set; }

        [DataType(DataType.Password)]
        [Compare("YeniSifre", ErrorMessage = "Yeni şifreler eşleşmiyor.")]
        public string? YeniSifreTekrar { get; set; }
    }
}
