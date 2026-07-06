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
        new EndeksOkuma { EndeksOkumaId = 1, TuketimNoktasiId = 1001, SayacId = 5001, OkumaTarihi = new DateTime(2026, 4, 5), IlkEndeks = 15200, SonEndeks = 15520, TuketimMiktari = 320, OkumaTipi = "Manuel", Durum = "Basarili", CreatedAt = new DateTime(2026, 4, 5) },
        new EndeksOkuma { EndeksOkumaId = 2, TuketimNoktasiId = 1002, SayacId = 5002, OkumaTarihi = new DateTime(2026, 4, 6), IlkEndeks = 18400, SonEndeks = 18750, TuketimMiktari = 350, OkumaTipi = "Manuel", Durum = "Basarili", CreatedAt = new DateTime(2026, 4, 6) },
        new EndeksOkuma { EndeksOkumaId = 3, TuketimNoktasiId = 1003, SayacId = 5003, OkumaTarihi = new DateTime(2026, 4, 7), IlkEndeks = 22100, SonEndeks = 22550, TuketimMiktari = 450, OkumaTipi = "Manuel", Durum = "Basarili", CreatedAt = new DateTime(2026, 4, 7) },
        new EndeksOkuma { EndeksOkumaId = 4, TuketimNoktasiId = 1004, SayacId = 5004, OkumaTarihi = new DateTime(2026, 4, 8), IlkEndeks = 16800, SonEndeks = 16800, TuketimMiktari = 0, OkumaTipi = "Manuel", Durum = "Sifir Tuketim", CreatedAt = new DateTime(2026, 4, 8) },
        new EndeksOkuma { EndeksOkumaId = 5, TuketimNoktasiId = 1005, SayacId = 5005, OkumaTarihi = new DateTime(2026, 4, 10), IlkEndeks = 19500, SonEndeks = 19780, TuketimMiktari = 280, OkumaTipi = "Manuel", Durum = "Basarili", CreatedAt = new DateTime(2026, 4, 10) },
        new EndeksOkuma { EndeksOkumaId = 6, TuketimNoktasiId = 1006, SayacId = 5006, OkumaTarihi = new DateTime(2026, 4, 12), IlkEndeks = 20100, SonEndeks = 22650, TuketimMiktari = 2550, OkumaTipi = "Manuel", Durum = "Anormal", CreatedAt = new DateTime(2026, 4, 12) },
        new EndeksOkuma { EndeksOkumaId = 15, TuketimNoktasiId = 1015, SayacId = 5015, OkumaTarihi = new DateTime(2026, 4, 5), IlkEndeks = 18900, SonEndeks = 19250, TuketimMiktari = 350, OkumaTipi = "OSOS", Durum = "Basarili", CreatedAt = new DateTime(2026, 4, 5) },
        new EndeksOkuma { EndeksOkumaId = 16, TuketimNoktasiId = 1016, SayacId = 5016, OkumaTarihi = new DateTime(2026, 4, 5), IlkEndeks = 21500, SonEndeks = 21920, TuketimMiktari = 420, OkumaTipi = "OSOS", Durum = "Basarili", CreatedAt = new DateTime(2026, 4, 5) },
        new EndeksOkuma { EndeksOkumaId = 17, TuketimNoktasiId = 1017, SayacId = 5017, OkumaTarihi = new DateTime(2026, 4, 6), IlkEndeks = 24800, SonEndeks = 25450, TuketimMiktari = 650, OkumaTipi = "OSOS", Durum = "Basarili", CreatedAt = new DateTime(2026, 4, 6) }
    };

    public List<EndeksOkuma> GetAll()
    {
        return _okumalar.OrderByDescending(x => x.OkumaTarihi).ToList();
    }

    public EndeksOkuma? GetById(long id)
    {
        return _okumalar.FirstOrDefault(x => x.EndeksOkumaId == id);
    }

    public List<EndeksOkuma> Filtrele(string? okumaTipi, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
    {
        var query = _okumalar.AsEnumerable();

        if (!string.IsNullOrEmpty(okumaTipi)) query = query.Where(x => x.OkumaTipi == okumaTipi);
        if (!string.IsNullOrEmpty(durum)) query = query.Where(x => x.Durum == durum);
        if (baslangic.HasValue) query = query.Where(x => x.OkumaTarihi >= baslangic.Value);
        if (bitis.HasValue) query = query.Where(x => x.OkumaTarihi <= bitis.Value);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var aramaLower = arama.ToLower();
            query = query.Where(x =>
                x.TuketimNoktasiId.ToString().Contains(aramaLower) ||
                x.SayacId.ToString().Contains(aramaLower)
            );
        }

        return query.OrderByDescending(x => x.OkumaTarihi).ToList();
    }

    public (int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim) GetIstatistikler()
    {
        int toplam = _okumalar.Count;
        int manuel = _okumalar.Count(x => x.OkumaTipi == "Manuel");
        int osos = _okumalar.Count(x => x.OkumaTipi == "OSOS");
        int anomali = _okumalar.Count(x => x.Durum == "Anormal");
        decimal ortalamaTuketim = toplam > 0 ? Math.Round(_okumalar.Average(x => x.TuketimMiktari), 2) : 0;

        return (toplam, manuel, osos, anomali, ortalamaTuketim);
    }
}
