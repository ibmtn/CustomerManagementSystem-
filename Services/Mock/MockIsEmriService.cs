using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Models.enums;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Mock;

/// <summary>
/// İş emri servisi mock implementasyonu.
/// 18 adet gerçekçi iş emri verisi ile çalışır. Tüm tipleri ve durumları kapsar.
/// </summary>
public class MockIsEmriService : IIsEmriService
{
    private static List<IsEmri> _isEmirleri = new()
    {
        // 1 - SayacSokme - Tamamlandı
        new IsEmri
        {
            IsEmriId = 1,
            IsEmriNo = "IE-2026-0001",
            TuketimNoktasiId = 1001,
            SayacId = 5001,
            Tip = IsEmriTipi.SayacSokme,
            Oncelik = "Yüksek",
            PlanlananTarih = new DateTime(2026, 4, 10),
            AtananKullaniciId = 101,
            AtananKullaniciAdi = "Ahmet Yılmaz",
            Durum = IsEmriDurumu.Tamamlandi,
            SahaSonucu = "Sayaç başarıyla söküldü",
            Gerekce = "Sayaç arızası nedeniyle sökme",
            EskiSayacNo = "SN-38-001234",
            EskiSonEndeksi = 18542.50m,
            TutanakNo = "TT-2026-0001",
            TuketimNoktasiKodu = "TK-2026-001",
            Adres = "Kocasinan Mah. Sivas Cad. No:12/A Kayseri",
            SayacSeriNo = "SN-38-001234",
            CreatedAt = new DateTime(2026, 4, 8),
            UpdatedAt = new DateTime(2026, 4, 10)
        },
        // 2 - SayacTakma - Tamamlandı
        new IsEmri
        {
            IsEmriId = 2,
            IsEmriNo = "IE-2026-0002",
            TuketimNoktasiId = 1001,
            SayacId = 5002,
            Tip = IsEmriTipi.SayacTakma,
            Oncelik = "Yüksek",
            PlanlananTarih = new DateTime(2026, 4, 11),
            AtananKullaniciId = 101,
            AtananKullaniciAdi = "Ahmet Yılmaz",
            Durum = IsEmriDurumu.Tamamlandi,
            SahaSonucu = "Yeni sayaç takıldı",
            YeniSayacNo = "SN-38-005678",
            YeniIlkEndeksi = 0.00m,
            MuhurNo = "MH-2026-0451",
            TutanakNo = "TT-2026-0002",
            TuketimNoktasiKodu = "TK-2026-002",
            Adres = "Kocasinan Mah. Sivas Cad. No:12/A Kayseri",
            SayacSeriNo = "SN-38-005678",
            CreatedAt = new DateTime(2026, 4, 9),
            UpdatedAt = new DateTime(2026, 4, 11)
        },
        // 3 - SayacDegisim - Tamamlandı
        new IsEmri
        {
            IsEmriId = 3,
            IsEmriNo = "IE-2026-0003",
            TuketimNoktasiId = 1002,
            SayacId = 5003,
            Tip = IsEmriTipi.SayacDegisim,
            Oncelik = "Yüksek",
            PlanlananTarih = new DateTime(2026, 4, 15),
            AtananKullaniciId = 102,
            AtananKullaniciAdi = "Mehmet Demir",
            Durum = IsEmriDurumu.Tamamlandi,
            SahaSonucu = "Sayaç değişimi tamamlandı",
            Gerekce = "Periyodik sayaç değişimi",
            EskiSayacNo = "SN-38-002345",
            EskiSonEndeksi = 22150.75m,
            YeniSayacNo = "SN-38-006789",
            YeniIlkEndeksi = 0.00m,
            MuhurNo = "MH-2026-0452",
            TutanakNo = "TT-2026-0003",
            TuketimNoktasiKodu = "TK-2026-003",
            Adres = "Melikgazi Mah. İstasyon Cad. No:45 Kayseri",
            SayacSeriNo = "SN-38-002345",
            CreatedAt = new DateTime(2026, 4, 12),
            UpdatedAt = new DateTime(2026, 4, 15)
        },
        // 4 - Kesme - Tamamlandı
        new IsEmri
        {
            IsEmriId = 4,
            IsEmriNo = "IE-2026-0004",
            TuketimNoktasiId = 1003,
            SayacId = 5004,
            Tip = IsEmriTipi.Kesme,
            Oncelik = "Acil",
            PlanlananTarih = new DateTime(2026, 4, 18),
            AtananKullaniciId = 103,
            AtananKullaniciAdi = "Ayşe Kaya",
            Durum = IsEmriDurumu.Tamamlandi,
            SahaSonucu = "Elektrik kesildi",
            Gerekce = "Borç nedeniyle kesme",
            KesmeEndeksi = 19875.25m,
            TutanakNo = "TT-2026-0004",
            TuketimNoktasiKodu = "TK-2026-004",
            Adres = "Talas Mah. Ali Dağı Blv. No:78/B Kayseri",
            SayacSeriNo = "SN-38-003456",
            CreatedAt = new DateTime(2026, 4, 16),
            UpdatedAt = new DateTime(2026, 4, 18)
        },
        // 5 - Acma - Tamamlandı
        new IsEmri
        {
            IsEmriId = 5,
            IsEmriNo = "IE-2026-0005",
            TuketimNoktasiId = 1003,
            SayacId = 5004,
            Tip = IsEmriTipi.Acma,
            Oncelik = "Normal",
            PlanlananTarih = new DateTime(2026, 5, 2),
            AtananKullaniciId = 103,
            AtananKullaniciAdi = "Ayşe Kaya",
            Durum = IsEmriDurumu.Tamamlandi,
            SahaSonucu = "Elektrik açıldı",
            Gerekce = "Borç ödendi, açma talebi",
            AcmaEndeksi = 19875.25m,
            TutanakNo = "TT-2026-0005",
            TuketimNoktasiKodu = "TK-2026-005",
            Adres = "Talas Mah. Ali Dağı Blv. No:78/B Kayseri",
            SayacSeriNo = "SN-38-003456",
            CreatedAt = new DateTime(2026, 4, 30),
            UpdatedAt = new DateTime(2026, 5, 2)
        },
        // 6 - KontrolMuayene - Tamamlandı
        new IsEmri
        {
            IsEmriId = 6,
            IsEmriNo = "IE-2026-0006",
            TuketimNoktasiId = 1004,
            SayacId = 5005,
            Tip = IsEmriTipi.KontrolMuayene,
            Oncelik = "Normal",
            PlanlananTarih = new DateTime(2026, 5, 5),
            AtananKullaniciId = 104,
            AtananKullaniciAdi = "Fatma Çelik",
            Durum = IsEmriDurumu.Tamamlandi,
            SahaSonucu = "Sayaç ve tesisat kontrolü yapıldı, sorun tespit edilmedi",
            TutanakNo = "TT-2026-0006",
            TuketimNoktasiKodu = "TK-2026-006",
            Adres = "İncesu Mah. Cumhuriyet Cad. No:23 Kayseri",
            SayacSeriNo = "SN-38-004567",
            CreatedAt = new DateTime(2026, 5, 3),
            UpdatedAt = new DateTime(2026, 5, 5)
        },
        // 7 - SayacDegisim - DevamEdiyor
        new IsEmri
        {
            IsEmriId = 7,
            IsEmriNo = "IE-2026-0007",
            TuketimNoktasiId = 1005,
            SayacId = 5006,
            Tip = IsEmriTipi.SayacDegisim,
            Oncelik = "Yüksek",
            PlanlananTarih = new DateTime(2026, 6, 20),
            AtananKullaniciId = 102,
            AtananKullaniciAdi = "Mehmet Demir",
            Durum = IsEmriDurumu.DevamEdiyor,
            TuketimNoktasiKodu = "TK-2026-007",
            Adres = "Hacılar Mah. Erciyes Cad. No:56 Kayseri",
            SayacSeriNo = "SN-38-005678",
            CreatedAt = new DateTime(2026, 6, 18)
        },
        // 8 - Kesme - EkibeAtandı
        new IsEmri
        {
            IsEmriId = 8,
            IsEmriNo = "IE-2026-0008",
            TuketimNoktasiId = 1006,
            SayacId = 5007,
            Tip = IsEmriTipi.Kesme,
            Oncelik = "Acil",
            PlanlananTarih = new DateTime(2026, 6, 25),
            AtananKullaniciId = 105,
            AtananKullaniciAdi = "Mustafa Özkan",
            Durum = IsEmriDurumu.EkibeAtandi,
            Gerekce = "6 aydır ödeme yapılmadı",
            TuketimNoktasiKodu = "TK-2026-008",
            Adres = "Develi Mah. Atatürk Cad. No:89 Kayseri",
            SayacSeriNo = "SN-38-006789",
            CreatedAt = new DateTime(2026, 6, 22)
        },
        // 9 - SayacSokme - Olusturuldu
        new IsEmri
        {
            IsEmriId = 9,
            IsEmriNo = "IE-2026-0009",
            TuketimNoktasiId = 1007,
            SayacId = 5008,
            Tip = IsEmriTipi.SayacSokme,
            Oncelik = "Normal",
            PlanlananTarih = new DateTime(2026, 7, 1),
            Durum = IsEmriDurumu.Olusturuldu,
            Gerekce = "Abone iptal talebi",
            TuketimNoktasiKodu = "TK-2026-009",
            Adres = "Bünyan Mah. Kayseri Cad. No:34 Kayseri",
            SayacSeriNo = "SN-38-007890",
            CreatedAt = new DateTime(2026, 6, 28)
        },
        // 10 - SayacTakma - Olusturuldu
        new IsEmri
        {
            IsEmriId = 10,
            IsEmriNo = "IE-2026-0010",
            TuketimNoktasiId = 1008,
            Tip = IsEmriTipi.SayacTakma,
            Oncelik = "Normal",
            PlanlananTarih = new DateTime(2026, 7, 3),
            Durum = IsEmriDurumu.Olusturuldu,
            Gerekce = "Yeni abonelik başvurusu",
            TuketimNoktasiKodu = "TK-2026-010",
            Adres = "Melikgazi Mah. Sahabiye Cad. No:67/C Kayseri",
            CreatedAt = new DateTime(2026, 6, 29)
        },
        // 11 - KontrolMuayene - EkibeAtandı
        new IsEmri
        {
            IsEmriId = 11,
            IsEmriNo = "IE-2026-0011",
            TuketimNoktasiId = 1009,
            SayacId = 5009,
            Tip = IsEmriTipi.KontrolMuayene,
            Oncelik = "Düşük",
            PlanlananTarih = new DateTime(2026, 6, 28),
            AtananKullaniciId = 104,
            AtananKullaniciAdi = "Fatma Çelik",
            Durum = IsEmriDurumu.EkibeAtandi,
            Gerekce = "Abone şikayeti - yüksek fatura",
            TuketimNoktasiKodu = "TK-2026-011",
            Adres = "Kocasinan Mah. Osman Kavuncu Blv. No:112 Kayseri",
            SayacSeriNo = "SN-38-008901",
            CreatedAt = new DateTime(2026, 6, 25)
        },
        // 12 - SayacDegisim - IptalEdildi
        new IsEmri
        {
            IsEmriId = 12,
            IsEmriNo = "IE-2026-0012",
            TuketimNoktasiId = 1010,
            SayacId = 5010,
            Tip = IsEmriTipi.SayacDegisim,
            Oncelik = "Normal",
            PlanlananTarih = new DateTime(2026, 5, 10),
            AtananKullaniciId = 101,
            AtananKullaniciAdi = "Ahmet Yılmaz",
            Durum = IsEmriDurumu.IptalEdildi,
            Gerekce = "Abone değişim talebinden vazgeçti",
            TuketimNoktasiKodu = "TK-2026-012",
            Adres = "Talas Mah. Yıldırım Beyazıt Cad. No:5 Kayseri",
            SayacSeriNo = "SN-38-009012",
            CreatedAt = new DateTime(2026, 5, 7),
            UpdatedAt = new DateTime(2026, 5, 9)
        },
        // 13 - Acma - DevamEdiyor
        new IsEmri
        {
            IsEmriId = 13,
            IsEmriNo = "IE-2026-0013",
            TuketimNoktasiId = 1011,
            SayacId = 5011,
            Tip = IsEmriTipi.Acma,
            Oncelik = "Yüksek",
            PlanlananTarih = new DateTime(2026, 6, 30),
            AtananKullaniciId = 105,
            AtananKullaniciAdi = "Mustafa Özkan",
            Durum = IsEmriDurumu.DevamEdiyor,
            Gerekce = "Tüm borçlar ödendi",
            TuketimNoktasiKodu = "TK-2026-013",
            Adres = "Pınarbaşı Mah. Gevher Nesibe Cad. No:91 Kayseri",
            SayacSeriNo = "SN-38-010123",
            CreatedAt = new DateTime(2026, 6, 27)
        },
        // 14 - Kesme - Durduruldu
        new IsEmri
        {
            IsEmriId = 14,
            IsEmriNo = "IE-2026-0014",
            TuketimNoktasiId = 1012,
            SayacId = 5012,
            Tip = IsEmriTipi.Kesme,
            Oncelik = "Normal",
            PlanlananTarih = new DateTime(2026, 5, 20),
            AtananKullaniciId = 103,
            AtananKullaniciAdi = "Ayşe Kaya",
            Durum = IsEmriDurumu.Durduruldu,
            Gerekce = "Abone ödeme planı yaptı, kesme durduruldu",
            TuketimNoktasiKodu = "TK-2026-014",
            Adres = "Yeşilhisar Mah. Hürriyet Cad. No:18 Kayseri",
            SayacSeriNo = "SN-38-011234",
            CreatedAt = new DateTime(2026, 5, 17),
            UpdatedAt = new DateTime(2026, 5, 19)
        },
        // 15 - SayacSokme - EkibeAtandı
        new IsEmri
        {
            IsEmriId = 15,
            IsEmriNo = "IE-2026-0015",
            TuketimNoktasiId = 1013,
            SayacId = 5013,
            Tip = IsEmriTipi.SayacSokme,
            Oncelik = "Yüksek",
            PlanlananTarih = new DateTime(2026, 7, 2),
            AtananKullaniciId = 102,
            AtananKullaniciAdi = "Mehmet Demir",
            Durum = IsEmriDurumu.EkibeAtandi,
            Gerekce = "Sayaçta kaçak şüphesi",
            TuketimNoktasiKodu = "TK-2026-015",
            Adres = "Kocasinan Mah. Mimar Sinan Cad. No:42/D Kayseri",
            SayacSeriNo = "SN-38-012345",
            CreatedAt = new DateTime(2026, 6, 30)
        },
        // 16 - SayacTakma - Tamamlandı
        new IsEmri
        {
            IsEmriId = 16,
            IsEmriNo = "IE-2026-0016",
            TuketimNoktasiId = 1014,
            SayacId = 5014,
            Tip = IsEmriTipi.SayacTakma,
            Oncelik = "Normal",
            PlanlananTarih = new DateTime(2026, 5, 25),
            AtananKullaniciId = 101,
            AtananKullaniciAdi = "Ahmet Yılmaz",
            Durum = IsEmriDurumu.Tamamlandi,
            SahaSonucu = "Yeni sayaç takıldı ve mühürlendi",
            YeniSayacNo = "SN-38-013456",
            YeniIlkEndeksi = 0.00m,
            MuhurNo = "MH-2026-0467",
            TutanakNo = "TT-2026-0016",
            TuketimNoktasiKodu = "TK-2026-016",
            Adres = "Melikgazi Mah. Forum Cad. No:100 Kayseri",
            SayacSeriNo = "SN-38-013456",
            CreatedAt = new DateTime(2026, 5, 22),
            UpdatedAt = new DateTime(2026, 5, 25)
        },
        // 17 - KontrolMuayene - Olusturuldu
        new IsEmri
        {
            IsEmriId = 17,
            IsEmriNo = "IE-2026-0017",
            TuketimNoktasiId = 1015,
            SayacId = 5015,
            Tip = IsEmriTipi.KontrolMuayene,
            Oncelik = "Düşük",
            PlanlananTarih = new DateTime(2026, 7, 5),
            Durum = IsEmriDurumu.Olusturuldu,
            Gerekce = "Rutin periyodik kontrol",
            TuketimNoktasiKodu = "TK-2026-017",
            Adres = "Talas Mah. Üniversite Cad. No:200 Kayseri",
            SayacSeriNo = "SN-38-014567",
            CreatedAt = new DateTime(2026, 7, 1)
        },
        // 18 - SayacDegisim - Olusturuldu
        new IsEmri
        {
            IsEmriId = 18,
            IsEmriNo = "IE-2026-0018",
            TuketimNoktasiId = 1016,
            SayacId = 5016,
            Tip = IsEmriTipi.SayacDegisim,
            Oncelik = "Acil",
            PlanlananTarih = new DateTime(2026, 7, 4),
            Durum = IsEmriDurumu.Olusturuldu,
            Gerekce = "Sayaç cam kırık, acil değişim gerekli",
            TuketimNoktasiKodu = "TK-2026-018",
            Adres = "Kocasinan Mah. Gevher Nesibe Cad. No:33/A Kayseri",
            SayacSeriNo = "SN-38-015678",
            CreatedAt = new DateTime(2026, 7, 1)
        }
    };

    /// <inheritdoc />
    public List<IsEmri> GetAll()
    {
        return _isEmirleri.OrderByDescending(x => x.CreatedAt).ToList();
    }

    /// <inheritdoc />
    public IsEmri? GetById(int id)
    {
        return _isEmirleri.FirstOrDefault(x => x.IsEmriId == id);
    }

    /// <inheritdoc />
    public List<IsEmri> Filtrele(IsEmriTipi? tip, IsEmriDurumu? durum, DateTime? baslangic, DateTime? bitis, string? arama)
    {
        var query = _isEmirleri.AsEnumerable();

        if (tip.HasValue)
            query = query.Where(x => x.Tip == tip.Value);

        if (durum.HasValue)
            query = query.Where(x => x.Durum == durum.Value);

        if (baslangic.HasValue)
            query = query.Where(x => x.PlanlananTarih >= baslangic.Value);

        if (bitis.HasValue)
            query = query.Where(x => x.PlanlananTarih <= bitis.Value);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var aramaLower = arama.ToLower();
            query = query.Where(x =>
                (x.IsEmriNo != null && x.IsEmriNo.ToLower().Contains(aramaLower)) ||
                (x.TuketimNoktasiKodu != null && x.TuketimNoktasiKodu.ToLower().Contains(aramaLower)) ||
                (x.Adres != null && x.Adres.ToLower().Contains(aramaLower)) ||
                (x.AtananKullaniciAdi != null && x.AtananKullaniciAdi.ToLower().Contains(aramaLower)) ||
                (x.SayacSeriNo != null && x.SayacSeriNo.ToLower().Contains(aramaLower)) ||
                (x.TutanakNo != null && x.TutanakNo.ToLower().Contains(aramaLower))
            );
        }

        return query.OrderByDescending(x => x.CreatedAt).ToList();
    }

    /// <inheritdoc />
    public void TutanakKaydet(int isEmriId, string tutanakNo, string sahaSonucu, string? gerekce,
        string? eskiSayacNo, decimal? eskiSonEndeksi,
        string? yeniSayacNo, decimal? yeniIlkEndeksi,
        string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi)
    {
        var isEmri = _isEmirleri.FirstOrDefault(x => x.IsEmriId == isEmriId);
        if (isEmri == null)
            throw new InvalidOperationException($"İş emri bulunamadı: {isEmriId}");

        isEmri.TutanakNo = tutanakNo;
        isEmri.SahaSonucu = sahaSonucu;
        isEmri.Gerekce = gerekce ?? isEmri.Gerekce;
        isEmri.EskiSayacNo = eskiSayacNo;
        isEmri.EskiSonEndeksi = eskiSonEndeksi;
        isEmri.YeniSayacNo = yeniSayacNo;
        isEmri.YeniIlkEndeksi = yeniIlkEndeksi;
        isEmri.MuhurNo = muhurNo;
        isEmri.KesmeEndeksi = kesmeEndeksi;
        isEmri.AcmaEndeksi = acmaEndeksi;
        isEmri.Durum = IsEmriDurumu.Tamamlandi;
        isEmri.UpdatedAt = DateTime.Now;
    }
}
