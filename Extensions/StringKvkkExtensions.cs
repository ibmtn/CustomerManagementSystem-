using System;

namespace MusteriYonetim.Extensions
{
    public static class StringKvkkExtensions
    {
        /// <summary>
        /// Genel verileri (TC, Telefon, Kredi Kartı) modern C# operatörleriyle maskeler.
        /// </summary>
        public static string Maskele(this string? veri, int baslangic = 2, int bitis = 2, char maske = '*')
        {
            // 1. Guard Clause (Koruyucu Yanıt)
            if (string.IsNullOrWhiteSpace(veri))
                return veri ?? string.Empty;

            // 2. Güvenlik Kontrolü
            if (veri.Length <= baslangic + bitis)
                return new string(maske, veri.Length);

            // 3. Modern Range (..) ve Index (^) Operatörleri ile Parçalama
            string basKisim = veri[..baslangic];
            string sonKisim = veri[^bitis..];
            string sansur = new string(maske, veri.Length - (baslangic + bitis));

            // 4. String Interpolation ile Birleştirme
            return $"{basKisim}{sansur}{sonKisim}";
        }

        /// <summary>
        /// E-posta adreslerini modern yaklaşımla maskeler.
        /// </summary>
        public static string EmailMaskele(this string? email, char maske = '*')
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                return email ?? string.Empty;

            int atIndex = email.IndexOf('@');
            
            // @ işaretinden öncesi ve sonrası olarak metni ikiye bölüyoruz
            string kullaniciAdi = email[..atIndex]; 
            string domain = email[atIndex..]; // @ işareti dahil geri kalanı

            if (kullaniciAdi.Length <= 2)
            {
                kullaniciAdi = new string(maske, kullaniciAdi.Length);
            }
            else
            {
                // Kullanıcı adının ilk 2 harfini al, kalanını maskele
                kullaniciAdi = $"{kullaniciAdi[..2]}{new string(maske, kullaniciAdi.Length - 2)}";
            }

            return $"{kullaniciAdi}{domain}";
        }
    }
}