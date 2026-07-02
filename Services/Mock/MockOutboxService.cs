using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Models.enums;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Mock;

/// <summary>
/// Entegrasyon Outbox servisi mock implementasyonu.
/// 25 adet gerçekçi entegrasyon mesajı ile çalışır.
/// Farklı işlem tipleri, hedef sistemler ve durumlar içerir.
/// </summary>
public class MockOutboxService : IOutboxService
{
    private static List<EntegrasyonOutbox> _outboxKayitlari = new()
    {
        // Bekleyen kayıtlar (7 adet)
        new EntegrasyonOutbox
        {
            outbox_id = 1,
            islem_tipi = "Fatura",
            referans_id = 4001,
            referans_no = "FT-2026-0001",
            hedef_sistem = "SAP",
            payload = "{\"fatura_no\":\"FT-2026-0001\",\"tutar\":1245.80,\"tarife\":\"Mesken\"}",
            durum = OutboxDurumu.Bekliyor,
            deneme_sayisi = 0,
            olusturulma_zamani = new DateTime(2026, 6, 28, 10, 30, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 28, 10, 35, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 2,
            islem_tipi = "Okuma",
            referans_id = 1,
            referans_no = "OK-2026-0001",
            hedef_sistem = "MDMS",
            payload = "{\"okuma_id\":1,\"endeks\":15520,\"tuketim\":320,\"kaynak\":\"Manuel\"}",
            durum = OutboxDurumu.Bekliyor,
            deneme_sayisi = 0,
            olusturulma_zamani = new DateTime(2026, 6, 28, 11, 0, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 28, 11, 5, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 3,
            islem_tipi = "İş Emri",
            referans_id = 7,
            referans_no = "IE-2026-0007",
            hedef_sistem = "GIS",
            payload = "{\"is_emri_no\":\"IE-2026-0007\",\"tip\":\"SayacDegisim\",\"durum\":\"DevamEdiyor\"}",
            durum = OutboxDurumu.Bekliyor,
            deneme_sayisi = 0,
            olusturulma_zamani = new DateTime(2026, 6, 29, 9, 15, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 29, 9, 20, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 4,
            islem_tipi = "Fatura",
            referans_id = 4002,
            referans_no = "FT-2026-0002",
            hedef_sistem = "SAP",
            payload = "{\"fatura_no\":\"FT-2026-0002\",\"tutar\":2380.50,\"tarife\":\"Ticarethane\"}",
            durum = OutboxDurumu.Bekliyor,
            deneme_sayisi = 0,
            olusturulma_zamani = new DateTime(2026, 6, 29, 14, 0, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 29, 14, 5, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 5,
            islem_tipi = "Okuma",
            referans_id = 35,
            referans_no = "OK-2026-0035",
            hedef_sistem = "MDMS",
            payload = "{\"okuma_id\":35,\"endeks\":18980,\"tuketim\":280,\"kaynak\":\"OSOS\"}",
            durum = OutboxDurumu.Bekliyor,
            deneme_sayisi = 0,
            olusturulma_zamani = new DateTime(2026, 6, 30, 8, 45, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 30, 8, 50, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 6,
            islem_tipi = "İş Emri",
            referans_id = 13,
            referans_no = "IE-2026-0013",
            hedef_sistem = "CRM",
            payload = "{\"is_emri_no\":\"IE-2026-0013\",\"tip\":\"Acma\",\"durum\":\"DevamEdiyor\"}",
            durum = OutboxDurumu.Bekliyor,
            deneme_sayisi = 0,
            olusturulma_zamani = new DateTime(2026, 6, 30, 10, 20, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 30, 10, 25, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 7,
            islem_tipi = "Fatura",
            referans_id = 4003,
            referans_no = "FT-2026-0003",
            hedef_sistem = "SAP",
            payload = "{\"fatura_no\":\"FT-2026-0003\",\"tutar\":985.20,\"tarife\":\"Mesken\"}",
            durum = OutboxDurumu.Bekliyor,
            deneme_sayisi = 0,
            olusturulma_zamani = new DateTime(2026, 7, 1, 9, 0, 0),
            sonraki_deneme_zamani = new DateTime(2026, 7, 1, 9, 5, 0)
        },

        // Gönderilmiş kayıtlar (10 adet)
        new EntegrasyonOutbox
        {
            outbox_id = 8,
            islem_tipi = "Fatura",
            referans_id = 4004,
            referans_no = "FT-2026-0004",
            hedef_sistem = "SAP",
            payload = "{\"fatura_no\":\"FT-2026-0004\",\"tutar\":1567.90,\"tarife\":\"Mesken\"}",
            durum = OutboxDurumu.Gonderildi,
            deneme_sayisi = 1,
            olusturulma_zamani = new DateTime(2026, 4, 10, 8, 0, 0),
            gonderim_zamani = new DateTime(2026, 4, 10, 8, 2, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 9,
            islem_tipi = "Okuma",
            referans_id = 3,
            referans_no = "OK-2026-0003",
            hedef_sistem = "MDMS",
            payload = "{\"okuma_id\":3,\"endeks\":22550,\"tuketim\":450,\"kaynak\":\"Manuel\"}",
            durum = OutboxDurumu.Gonderildi,
            deneme_sayisi = 1,
            olusturulma_zamani = new DateTime(2026, 4, 12, 14, 30, 0),
            gonderim_zamani = new DateTime(2026, 4, 12, 14, 31, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 10,
            islem_tipi = "İş Emri",
            referans_id = 1,
            referans_no = "IE-2026-0001",
            hedef_sistem = "GIS",
            payload = "{\"is_emri_no\":\"IE-2026-0001\",\"tip\":\"SayacSokme\",\"durum\":\"Tamamlandi\"}",
            durum = OutboxDurumu.Gonderildi,
            deneme_sayisi = 1,
            olusturulma_zamani = new DateTime(2026, 4, 15, 9, 0, 0),
            gonderim_zamani = new DateTime(2026, 4, 15, 9, 1, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 11,
            islem_tipi = "Fatura",
            referans_id = 4005,
            referans_no = "FT-2026-0005",
            hedef_sistem = "SAP",
            payload = "{\"fatura_no\":\"FT-2026-0005\",\"tutar\":3245.00,\"tarife\":\"Sanayi\"}",
            durum = OutboxDurumu.Gonderildi,
            deneme_sayisi = 2,
            olusturulma_zamani = new DateTime(2026, 5, 3, 10, 15, 0),
            gonderim_zamani = new DateTime(2026, 5, 3, 10, 25, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 12,
            islem_tipi = "Okuma",
            referans_id = 15,
            referans_no = "OK-2026-0015",
            hedef_sistem = "MDMS",
            payload = "{\"okuma_id\":15,\"endeks\":19250,\"tuketim\":350,\"kaynak\":\"OSOS\"}",
            durum = OutboxDurumu.Gonderildi,
            deneme_sayisi = 1,
            olusturulma_zamani = new DateTime(2026, 5, 5, 11, 0, 0),
            gonderim_zamani = new DateTime(2026, 5, 5, 11, 1, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 13,
            islem_tipi = "İş Emri",
            referans_id = 4,
            referans_no = "IE-2026-0004",
            hedef_sistem = "CRM",
            payload = "{\"is_emri_no\":\"IE-2026-0004\",\"tip\":\"Kesme\",\"durum\":\"Tamamlandi\"}",
            durum = OutboxDurumu.Gonderildi,
            deneme_sayisi = 1,
            olusturulma_zamani = new DateTime(2026, 5, 10, 15, 30, 0),
            gonderim_zamani = new DateTime(2026, 5, 10, 15, 31, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 14,
            islem_tipi = "Fatura",
            referans_id = 4006,
            referans_no = "FT-2026-0006",
            hedef_sistem = "SAP",
            payload = "{\"fatura_no\":\"FT-2026-0006\",\"tutar\":890.40,\"tarife\":\"Mesken\"}",
            durum = OutboxDurumu.Gonderildi,
            deneme_sayisi = 1,
            olusturulma_zamani = new DateTime(2026, 5, 20, 9, 45, 0),
            gonderim_zamani = new DateTime(2026, 5, 20, 9, 46, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 15,
            islem_tipi = "Okuma",
            referans_id = 21,
            referans_no = "OK-2026-0021",
            hedef_sistem = "MDMS",
            payload = "{\"okuma_id\":21,\"endeks\":18150,\"tuketim\":350,\"kaynak\":\"OSOS\"}",
            durum = OutboxDurumu.Gonderildi,
            deneme_sayisi = 1,
            olusturulma_zamani = new DateTime(2026, 6, 2, 8, 30, 0),
            gonderim_zamani = new DateTime(2026, 6, 2, 8, 31, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 16,
            islem_tipi = "İş Emri",
            referans_id = 5,
            referans_no = "IE-2026-0005",
            hedef_sistem = "GIS",
            payload = "{\"is_emri_no\":\"IE-2026-0005\",\"tip\":\"Acma\",\"durum\":\"Tamamlandi\"}",
            durum = OutboxDurumu.Gonderildi,
            deneme_sayisi = 1,
            olusturulma_zamani = new DateTime(2026, 6, 5, 13, 0, 0),
            gonderim_zamani = new DateTime(2026, 6, 5, 13, 1, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 17,
            islem_tipi = "Fatura",
            referans_id = 4007,
            referans_no = "FT-2026-0007",
            hedef_sistem = "SAP",
            payload = "{\"fatura_no\":\"FT-2026-0007\",\"tutar\":1750.60,\"tarife\":\"Ticarethane\"}",
            durum = OutboxDurumu.Gonderildi,
            deneme_sayisi = 1,
            olusturulma_zamani = new DateTime(2026, 6, 15, 10, 0, 0),
            gonderim_zamani = new DateTime(2026, 6, 15, 10, 2, 0)
        },

        // Başarısız kayıtlar (5 adet)
        new EntegrasyonOutbox
        {
            outbox_id = 18,
            islem_tipi = "Fatura",
            referans_id = 4008,
            referans_no = "FT-2026-0008",
            hedef_sistem = "SAP",
            payload = "{\"fatura_no\":\"FT-2026-0008\",\"tutar\":2100.30,\"tarife\":\"Sanayi\"}",
            durum = OutboxDurumu.Basarisiz,
            deneme_sayisi = 3,
            son_hata_mesaji = "Bağlantı zaman aşımı",
            olusturulma_zamani = new DateTime(2026, 6, 10, 14, 0, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 10, 15, 0, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 19,
            islem_tipi = "Okuma",
            referans_id = 20,
            referans_no = "OK-2026-0020",
            hedef_sistem = "MDMS",
            payload = "{\"okuma_id\":20,\"endeks\":18200,\"tuketim\":2600,\"kaynak\":\"OSOS\"}",
            durum = OutboxDurumu.Basarisiz,
            deneme_sayisi = 5,
            son_hata_mesaji = "HTTP 500 Internal Server Error",
            olusturulma_zamani = new DateTime(2026, 6, 12, 9, 30, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 12, 12, 30, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 20,
            islem_tipi = "İş Emri",
            referans_id = 8,
            referans_no = "IE-2026-0008",
            hedef_sistem = "GIS",
            payload = "{\"is_emri_no\":\"IE-2026-0008\",\"tip\":\"Kesme\",\"durum\":\"EkibeAtandi\"}",
            durum = OutboxDurumu.Basarisiz,
            deneme_sayisi = 4,
            son_hata_mesaji = "Geçersiz payload formatı",
            olusturulma_zamani = new DateTime(2026, 6, 22, 16, 0, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 22, 18, 0, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 21,
            islem_tipi = "Fatura",
            referans_id = 4009,
            referans_no = "FT-2026-0009",
            hedef_sistem = "CRM",
            payload = "{\"fatura_no\":\"FT-2026-0009\",\"tutar\":1480.75,\"tarife\":\"Mesken\"}",
            durum = OutboxDurumu.Basarisiz,
            deneme_sayisi = 3,
            son_hata_mesaji = "Hedef sistem yanıt vermedi",
            olusturulma_zamani = new DateTime(2026, 6, 25, 11, 15, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 25, 13, 15, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 22,
            islem_tipi = "Okuma",
            referans_id = 29,
            referans_no = "OK-2026-0029",
            hedef_sistem = "MDMS",
            payload = "{\"okuma_id\":29,\"endeks\":23100,\"tuketim\":2500,\"kaynak\":\"OSOS\"}",
            durum = OutboxDurumu.Basarisiz,
            deneme_sayisi = 2,
            son_hata_mesaji = "Bağlantı zaman aşımı",
            olusturulma_zamani = new DateTime(2026, 6, 27, 8, 0, 0),
            sonraki_deneme_zamani = new DateTime(2026, 6, 27, 9, 0, 0)
        },

        // İptal edilmiş kayıtlar (3 adet)
        new EntegrasyonOutbox
        {
            outbox_id = 23,
            islem_tipi = "İş Emri",
            referans_id = 12,
            referans_no = "IE-2026-0012",
            hedef_sistem = "GIS",
            payload = "{\"is_emri_no\":\"IE-2026-0012\",\"tip\":\"SayacDegisim\",\"durum\":\"IptalEdildi\"}",
            durum = OutboxDurumu.IptalEdildi,
            deneme_sayisi = 0,
            olusturulma_zamani = new DateTime(2026, 5, 9, 10, 0, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 24,
            islem_tipi = "Fatura",
            referans_id = 4010,
            referans_no = "FT-2026-0010",
            hedef_sistem = "SAP",
            payload = "{\"fatura_no\":\"FT-2026-0010\",\"tutar\":0,\"tarife\":\"Mesken\",\"durum\":\"IptalEdildi\"}",
            durum = OutboxDurumu.IptalEdildi,
            deneme_sayisi = 1,
            son_hata_mesaji = "Fatura iptal edildi, gönderim durduruldu",
            olusturulma_zamani = new DateTime(2026, 5, 15, 14, 30, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 25,
            islem_tipi = "Okuma",
            referans_id = 11,
            referans_no = "OK-2026-0011",
            hedef_sistem = "MDMS",
            payload = "{\"okuma_id\":11,\"endeks\":19200,\"tuketim\":0,\"kaynak\":\"Manuel\",\"durum\":\"SifirTuketim\"}",
            durum = OutboxDurumu.IptalEdildi,
            deneme_sayisi = 2,
            son_hata_mesaji = "Sıfır tüketim kaydı doğrulanmadı, gönderim iptal edildi",
            olusturulma_zamani = new DateTime(2026, 5, 20, 16, 0, 0)
        }
    };

    /// <inheritdoc />
    public List<EntegrasyonOutbox> GetAll()
    {
        return _outboxKayitlari.OrderByDescending(x => x.olusturulma_zamani).ToList();
    }

    /// <inheritdoc />
    public EntegrasyonOutbox? GetById(int id)
    {
        return _outboxKayitlari.FirstOrDefault(x => x.outbox_id == id);
    }

    /// <inheritdoc />
    public List<EntegrasyonOutbox> Filtrele(OutboxDurumu? durum, string? islemTipi, DateTime? baslangic, DateTime? bitis)
    {
        var query = _outboxKayitlari.AsEnumerable();

        if (durum.HasValue)
            query = query.Where(x => x.durum == durum.Value);

        if (!string.IsNullOrWhiteSpace(islemTipi))
            query = query.Where(x => x.islem_tipi.Equals(islemTipi, StringComparison.OrdinalIgnoreCase));

        if (baslangic.HasValue)
            query = query.Where(x => x.olusturulma_zamani >= baslangic.Value);

        if (bitis.HasValue)
            query = query.Where(x => x.olusturulma_zamani <= bitis.Value);

        return query.OrderByDescending(x => x.olusturulma_zamani).ToList();
    }

    /// <inheritdoc />
    public (int Toplam, int Bekleyen, int Gonderilmis, int Basarisiz) GetIstatistikler()
    {
        int toplam = _outboxKayitlari.Count;
        int bekleyen = _outboxKayitlari.Count(x => x.durum == OutboxDurumu.Bekliyor);
        int gonderilmis = _outboxKayitlari.Count(x => x.durum == OutboxDurumu.Gonderildi);
        int basarisiz = _outboxKayitlari.Count(x => x.durum == OutboxDurumu.Basarisiz);

        return (toplam, bekleyen, gonderilmis, basarisiz);
    }

    /// <inheritdoc />
    public bool YenidenGonder(int id)
    {
        var kayit = _outboxKayitlari.FirstOrDefault(x => x.outbox_id == id);
        if (kayit == null)
            return false;

        // Sadece başarısız veya iptal edilmiş kayıtlar yeniden gönderilebilir
        if (kayit.durum != OutboxDurumu.Basarisiz && kayit.durum != OutboxDurumu.IptalEdildi)
            return false;

        kayit.durum = OutboxDurumu.Bekliyor;
        kayit.deneme_sayisi = 0;
        kayit.son_hata_mesaji = null;
        kayit.sonraki_deneme_zamani = DateTime.Now.AddMinutes(5);

        return true;
    }
}
