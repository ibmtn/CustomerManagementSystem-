using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KcetasWeb.ViewModels
{
    public class TutanakGirisViewModel : IValidatableObject
    {
        public long IsEmriId { get; set; }
        public string IsEmriNo { get; set; } = null!;
        public KcetasWeb.Models.Enums.IsEmriTipi Tip { get; set; }

        [Required(ErrorMessage = "Tutanak numarası zorunludur")]
        public string TutanakNo { get; set; } = null!;

        [Required(ErrorMessage = "Saha sonucu zorunludur")]
        public string SahaSonucu { get; set; } = null!;

        public string? Gerekce { get; set; }

        [Required]
        public DateTime IslemTarihi { get; set; } = DateTime.Now;

        public string? MuhurNo { get; set; }

        // Sökme/Takma Endeksleri
        public string? EskiSayacNo { get; set; }
        public string? YeniSayacNo { get; set; }
        public string? YeniSayacMarka { get; set; }
        public string? YeniSayacModel { get; set; }
        public string? YeniSayacFaz { get; set; }
        public decimal? EskiSonEndeksi { get; set; }
        public decimal? YeniIlkEndeksi { get; set; }

        // Açma/Kesme Endeksleri
        public decimal? KesmeEndeksi { get; set; }
        public decimal? AcmaEndeksi { get; set; }

        // Periyodik Okuma Endeksi
        public decimal? GuncelEndeks { get; set; }

        public bool IsSokmeTakma => Tip == KcetasWeb.Models.Enums.IsEmriTipi.Sokme || Tip == KcetasWeb.Models.Enums.IsEmriTipi.YeniBaglanti || Tip == KcetasWeb.Models.Enums.IsEmriTipi.Degistirme;
        public bool IsAcmaKesme => Tip == KcetasWeb.Models.Enums.IsEmriTipi.Kesme || Tip == KcetasWeb.Models.Enums.IsEmriTipi.Acma;

        // Belgedeki kural: "Seçilen iş emri tipine göre farklı alanlar zorunlu olmalı"
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (Tip)
            {
                case KcetasWeb.Models.Enums.IsEmriTipi.Kesme:
                    if (!KesmeEndeksi.HasValue)
                        yield return new ValidationResult("Kesme işleminde Kesme Endeksi zorunludur.", new[] { nameof(KesmeEndeksi) });
                    break;

                case KcetasWeb.Models.Enums.IsEmriTipi.Acma:
                    if (!AcmaEndeksi.HasValue)
                        yield return new ValidationResult("Açma işleminde Açma Endeksi zorunludur.", new[] { nameof(AcmaEndeksi) });
                    break;

                case KcetasWeb.Models.Enums.IsEmriTipi.EndeksOkuma:
                    if (!GuncelEndeks.HasValue)
                        yield return new ValidationResult("Endeks okuma işleminde Güncel Endeks zorunludur.", new[] { nameof(GuncelEndeks) });
                    break;

                case KcetasWeb.Models.Enums.IsEmriTipi.Sokme:
                    if (string.IsNullOrWhiteSpace(EskiSayacNo))
                        yield return new ValidationResult("Sayaç sökme işleminde Eski Sayaç Seri No zorunludur.", new[] { nameof(EskiSayacNo) });
                    if (!EskiSonEndeksi.HasValue)
                        yield return new ValidationResult("Sayaç sökme işleminde Eski Sayaç Son Endeks zorunludur.", new[] { nameof(EskiSonEndeksi) });
                    if (string.IsNullOrWhiteSpace(MuhurNo))
                        yield return new ValidationResult("Sayaç sökme işleminde Mühür No zorunludur.", new[] { nameof(MuhurNo) });
                    break;

                case KcetasWeb.Models.Enums.IsEmriTipi.YeniBaglanti:
                    if (string.IsNullOrWhiteSpace(YeniSayacNo))
                        yield return new ValidationResult("Sayaç takma işleminde Yeni Sayaç Seri No zorunludur.", new[] { nameof(YeniSayacNo) });
                    if (!YeniIlkEndeksi.HasValue)
                        yield return new ValidationResult("Sayaç takma işleminde Yeni Sayaç İlk Endeks zorunludur.", new[] { nameof(YeniIlkEndeksi) });
                    if (string.IsNullOrWhiteSpace(MuhurNo))
                        yield return new ValidationResult("Sayaç takma işleminde Mühür No zorunludur.", new[] { nameof(MuhurNo) });
                    break;

                case KcetasWeb.Models.Enums.IsEmriTipi.Degistirme:
                    if (string.IsNullOrWhiteSpace(EskiSayacNo))
                        yield return new ValidationResult("Sayaç değişiminde Eski Sayaç Seri No zorunludur.", new[] { nameof(EskiSayacNo) });
                    if (!EskiSonEndeksi.HasValue)
                        yield return new ValidationResult("Sayaç değişiminde Eski Sayaç Son Endeks zorunludur.", new[] { nameof(EskiSonEndeksi) });
                    if (string.IsNullOrWhiteSpace(YeniSayacNo))
                        yield return new ValidationResult("Sayaç değişiminde Yeni Sayaç Seri No zorunludur.", new[] { nameof(YeniSayacNo) });
                    if (!YeniIlkEndeksi.HasValue)
                        yield return new ValidationResult("Sayaç değişiminde Yeni Sayaç İlk Endeks zorunludur.", new[] { nameof(YeniIlkEndeksi) });
                    if (string.IsNullOrWhiteSpace(MuhurNo))
                        yield return new ValidationResult("Sayaç değişiminde Mühür No zorunludur.", new[] { nameof(MuhurNo) });
                    break;
            }
        }
    }
}