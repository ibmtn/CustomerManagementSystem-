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
            Id = 1,
            AggregateType = "Fatura",
            AggregateId = "FT-2026-0001",
            EventType = "FaturaOlusturuldu",
            Payload = "{\"fatura_no\":\"FT-2026-0001\",\"tutar\":1245.80,\"tarife\":\"Mesken\"}",
            Status = "Bekliyor",
            CreatedAt = new DateTime(2026, 6, 28, 10, 30, 0)
        },
        new EntegrasyonOutbox
        {
            Id = 2,
            AggregateType = "EndeksOkuma",
            AggregateId = "EO-2026-0005",
            EventType = "EndeksOkundu",
            Payload = "{\"sayac_id\":5001,\"ilk_endeks\":15200,\"son_endeks\":15520}",
            Status = "Gönderildi",
            CreatedAt = new DateTime(2026, 6, 28, 11, 00, 0),
            ProcessedAt = new DateTime(2026, 6, 28, 11, 05, 0)
        },
        new EntegrasyonOutbox
        {
            Id = 3,
            AggregateType = "Abone",
            AggregateId = "ABN-10045",
            EventType = "AboneDurumDegisti",
            Payload = "{\"abone_id\":1,\"yeni_durum\":\"Pasif\"}",
            Status = "Başarısız",
            CreatedAt = new DateTime(2026, 6, 28, 12, 00, 0)
        }
    };

    public List<EntegrasyonOutbox> GetAll()
    {
        return _outboxKayitlari.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public EntegrasyonOutbox? GetById(long id)
    {
        return _outboxKayitlari.FirstOrDefault(x => x.Id == id);
    }

    public List<EntegrasyonOutbox> Filtrele(string? durum, string? eventType, DateTime? baslangic, DateTime? bitis)
    {
        var query = _outboxKayitlari.AsEnumerable();

        if (!string.IsNullOrEmpty(durum)) query = query.Where(x => x.Status == durum);
        if (!string.IsNullOrEmpty(eventType)) query = query.Where(x => x.EventType != null && x.EventType.Contains(eventType, StringComparison.OrdinalIgnoreCase));
        if (baslangic.HasValue) query = query.Where(x => x.CreatedAt >= baslangic.Value);
        if (bitis.HasValue) query = query.Where(x => x.CreatedAt <= bitis.Value);

        return query.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public (int Toplam, int Bekleyen, int Gonderilmis, int Basarisiz) GetIstatistikler()
    {
        int toplam = _outboxKayitlari.Count;
        int bekleyen = _outboxKayitlari.Count(x => x.Status == "Bekliyor");
        int gonderilmis = _outboxKayitlari.Count(x => x.Status == "Gönderildi");
        int basarisiz = _outboxKayitlari.Count(x => x.Status == "Başarısız");

        return (toplam, bekleyen, gonderilmis, basarisiz);
    }

    public bool YenidenGonder(long id)
    {
        var kayit = GetById(id);
        if (kayit != null && kayit.Status == "Başarısız")
        {
            kayit.Status = "Bekliyor";
            return true;
        }
        return false;
    }
}
