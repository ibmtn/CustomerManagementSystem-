using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock;

public class MockEndeksOkumaService : IEndeksOkumaService
{
    private static readonly List<EndeksOkuma> _okumalar = new()
    {
        new EndeksOkuma { okuma_id = 1, sayac_id = 5001, okuma_zamani = new DateTime(2026, 4, 5), onceki_endeks = 15200, yeni_endeks = 15520, okuma_tipi = "Manuel", status = "Basarili", CreatedAt = new DateTime(2026, 4, 5) },
        new EndeksOkuma { okuma_id = 2, sayac_id = 5002, okuma_zamani = new DateTime(2026, 4, 6), onceki_endeks = 18400, yeni_endeks = 18750, okuma_tipi = "Manuel", status = "Basarili", CreatedAt = new DateTime(2026, 4, 6) },
        new EndeksOkuma { okuma_id = 3, sayac_id = 5003, okuma_zamani = new DateTime(2026, 4, 7), onceki_endeks = 22100, yeni_endeks = 22550, okuma_tipi = "Manuel", status = "Basarili", CreatedAt = new DateTime(2026, 4, 7) },
        new EndeksOkuma { okuma_id = 4, sayac_id = 5004, okuma_zamani = new DateTime(2026, 4, 8), onceki_endeks = 16800, yeni_endeks = 16800, okuma_tipi = "Manuel", status = "Sifir Tuketim", okunamam_nedeni = 1, CreatedAt = new DateTime(2026, 4, 8) },
        new EndeksOkuma { okuma_id = 5, sayac_id = 5005, okuma_zamani = new DateTime(2026, 4, 10), onceki_endeks = 19500, yeni_endeks = 19780, okuma_tipi = "Manuel", status = "Basarili", CreatedAt = new DateTime(2026, 4, 10) },
        new EndeksOkuma { okuma_id = 6, sayac_id = 5006, okuma_zamani = new DateTime(2026, 4, 12), onceki_endeks = 20100, yeni_endeks = 22650, okuma_tipi = "Manuel", status = "Anormal", anomali_mi = true, AnomaliAciklamasi = "Aşırı Tüketim Artışı", CreatedAt = new DateTime(2026, 4, 12) },
        new EndeksOkuma { okuma_id = 15, sayac_id = 5015, okuma_zamani = new DateTime(2026, 4, 5), onceki_endeks = 18900, yeni_endeks = 19250, okuma_tipi = "OSOS", status = "Basarili", CreatedAt = new DateTime(2026, 4, 5) },
        new EndeksOkuma { okuma_id = 16, sayac_id = 5016, okuma_zamani = new DateTime(2026, 4, 5), onceki_endeks = 21500, yeni_endeks = 21920, okuma_tipi = "OSOS", status = "Basarili", CreatedAt = new DateTime(2026, 4, 5) },
        new EndeksOkuma { okuma_id = 17, sayac_id = 5017, okuma_zamani = new DateTime(2026, 4, 6), onceki_endeks = 24800, yeni_endeks = 25450, okuma_tipi = "OSOS", status = "Basarili", CreatedAt = new DateTime(2026, 4, 6) }
    };

    public List<EndeksOkuma> GetAll()
    {
        return _okumalar.OrderByDescending(x => x.okuma_zamani).ToList();
    }

    public EndeksOkuma? GetById(long id)
    {
        return _okumalar.FirstOrDefault(x => x.okuma_id == id);
    }

    public List<EndeksOkuma> Filtrele(string? okumaTipi, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
    {
        var query = _okumalar.AsEnumerable();

        if (!string.IsNullOrEmpty(okumaTipi)) query = query.Where(x => x.okuma_tipi == okumaTipi);
        if (!string.IsNullOrEmpty(durum)) query = query.Where(x => x.status == durum);
        if (baslangic.HasValue) query = query.Where(x => x.okuma_zamani >= baslangic.Value);
        if (bitis.HasValue) query = query.Where(x => x.okuma_zamani <= bitis.Value);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var aramaLower = arama.ToLower();
            query = query.Where(x =>
                x.sayac_id.ToString().Contains(aramaLower)
            );
        }

        return query.OrderByDescending(x => x.okuma_zamani).ToList();
    }

    public (int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim) GetIstatistikler()
    {
        int toplam = _okumalar.Count;
        int manuel = _okumalar.Count(x => x.okuma_tipi == "Manuel");
        int osos = _okumalar.Count(x => x.okuma_tipi == "OSOS");
        int anomali = _okumalar.Count(x => x.status == "Anormal");
        decimal ortalamaTuketim = toplam > 0 ? Math.Round(_okumalar.Average(x => x.yeni_endeks - x.onceki_endeks), 2) : 0;

        return (toplam, manuel, osos, anomali, ortalamaTuketim);
    }
}
