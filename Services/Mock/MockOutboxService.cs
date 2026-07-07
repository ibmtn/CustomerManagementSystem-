using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock;

public class MockOutboxService : IOutboxService
{
    private static List<EntegrasyonOutbox> _outboxKayitlari = new()
    {
        new EntegrasyonOutbox
        {
            outbox_id = 1,
            fatura_id = 1,
            hedef_sistem = "SAP",
            corrolation_id = "FT-2026-0001",
            paload = "{\"fatura_no\":\"FT-2026-0001\",\"tutar\":1245.80,\"tarife\":\"Mesken\"}",
            durum = "Bekliyor",
            created_at = new DateTime(2026, 6, 28, 10, 30, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 2,
            fatura_id = 2,
            hedef_sistem = "e-Devlet",
            corrolation_id = "EO-2026-0005",
            paload = "{\"sayac_id\":5001,\"ilk_endeks\":15200,\"son_endeks\":15520}",
            durum = "Gönderildi",
            created_at = new DateTime(2026, 6, 28, 11, 00, 0),
            gonderim_zamani = new DateTime(2026, 6, 28, 11, 05, 0)
        },
        new EntegrasyonOutbox
        {
            outbox_id = 3,
            fatura_id = 3,
            hedef_sistem = "Maliye",
            corrolation_id = "ABN-10045",
            paload = "{\"abone_id\":1,\"yeni_durum\":\"Pasif\"}",
            durum = "Başarısız",
            created_at = new DateTime(2026, 6, 28, 12, 00, 0)
        }
    };

    public List<EntegrasyonOutbox> GetAll()
    {
        return _outboxKayitlari.OrderByDescending(x => x.created_at).ToList();
    }

    public EntegrasyonOutbox? GetById(long id)
    {
        return _outboxKayitlari.FirstOrDefault(x => x.outbox_id == id);
    }

    public List<EntegrasyonOutbox> Filtrele(string? durum, string? eventType, DateTime? baslangic, DateTime? bitis)
    {
        var query = _outboxKayitlari.AsEnumerable();

        if (!string.IsNullOrEmpty(durum)) query = query.Where(x => x.durum == durum);
        if (!string.IsNullOrEmpty(eventType)) query = query.Where(x => x.hedef_sistem != null && x.hedef_sistem.Contains(eventType, StringComparison.OrdinalIgnoreCase));
        if (baslangic.HasValue) query = query.Where(x => x.created_at >= baslangic.Value);
        if (bitis.HasValue) query = query.Where(x => x.created_at <= bitis.Value);

        return query.OrderByDescending(x => x.created_at).ToList();
    }

    public (int Toplam, int Bekleyen, int Gonderilmis, int Basarisiz) GetIstatistikler()
    {
        int toplam = _outboxKayitlari.Count;
        int bekleyen = _outboxKayitlari.Count(x => x.durum == "Bekliyor");
        int gonderilmis = _outboxKayitlari.Count(x => x.durum == "Gönderildi");
        int basarisiz = _outboxKayitlari.Count(x => x.durum == "Başarısız");

        return (toplam, bekleyen, gonderilmis, basarisiz);
    }

    public bool YenidenGonder(long id)
    {
        var kayit = GetById(id);
        if (kayit != null && kayit.durum == "Başarısız")
        {
            kayit.durum = "Bekliyor";
            return true;
        }
        return false;
    }
}
