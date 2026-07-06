using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock;

public class MockIsEmriService : IIsEmriService
{
    private static List<IsEmri> _isEmirleri = new()
    {
        new IsEmri
        {
            IsEmriId = 1,
            IsEmriNo = "IE-2026-0001",
            TuketimNoktasiId = 1001,
            SayacId = 5001,
            Tip = "Sayaç Sökme",
            Oncelik = "Yüksek",
            PlanlananTarih = new DateTime(2026, 4, 10),
            AtananKullaniciId = 101,
            Durum = "Tamamlandı",
            SahaSonucu = "Sayaç başarıyla söküldü",
            Gerekce = "Sayaç arızası nedeniyle sökme",
            TutanakNo = "TT-2026-0001",
            Status = "Active",
            CreatedAt = DateTime.Now.AddDays(-20)
        },
        new IsEmri
        {
            IsEmriId = 2,
            IsEmriNo = "IE-2026-0002",
            TuketimNoktasiId = 1002,
            SayacId = 5002,
            Tip = "Sayaç Takma",
            Oncelik = "Normal",
            PlanlananTarih = new DateTime(2026, 4, 15),
            AtananKullaniciId = 102,
            Durum = "Devam Ediyor",
            Status = "Active",
            CreatedAt = DateTime.Now.AddDays(-15)
        }
    };

    public List<IsEmri> GetAll()
    {
        return _isEmirleri.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public IsEmri? GetById(long id)
    {
        return _isEmirleri.FirstOrDefault(x => x.IsEmriId == id);
    }

    public List<IsEmri> Filtrele(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
    {
        var query = _isEmirleri.AsEnumerable();

        if (!string.IsNullOrEmpty(tip)) query = query.Where(x => x.Tip == tip);
        if (!string.IsNullOrEmpty(durum)) query = query.Where(x => x.Durum == durum);
        if (baslangic.HasValue) query = query.Where(x => x.PlanlananTarih >= baslangic.Value);
        if (bitis.HasValue) query = query.Where(x => x.PlanlananTarih <= bitis.Value);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var aramaLower = arama.ToLower();
            query = query.Where(x =>
                (x.IsEmriNo != null && x.IsEmriNo.ToLower().Contains(aramaLower)) ||
                x.TuketimNoktasiId.ToString().Contains(aramaLower)
            );
        }

        return query.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public void TutanakKaydet(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo)
    {
        var isEmri = _isEmirleri.FirstOrDefault(x => x.IsEmriId == isEmriId);
        if (isEmri != null)
        {
            isEmri.TutanakNo = tutanakNo;
            isEmri.SahaSonucu = sahaSonucu;
            isEmri.Gerekce = gerekce ?? "";
            isEmri.MuhurNo = muhurNo ?? "";
            isEmri.Durum = "Tamamlandı";
            isEmri.UpdatedAt = DateTime.Now;
        }
    }
}
