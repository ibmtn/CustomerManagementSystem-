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
            is_emri_id = 1,
            is_emri_no = "IE-2026-0001",
            tuketim_noktasi_id = 1001,
            sayac_id = 5001,
            tip = "Sayaç Sökme",
            oncelik = "Yüksek",
            planlanan_tarih = new DateTime(2026, 4, 10),
            atanan_kullanici_id = 101,
            durum = "Tamamlandı",
            saha_sonucu = "Sayaç başarıyla söküldü",
            gerekce = "Sayaç arızası nedeniyle sökme",
            tutanak_no = "TT-2026-0001",
            status = "Active",
            eski_sayac_no = "S-1001",
            eski_son_endeksi = 15200.5m,
            CreatedAt = DateTime.Now.AddDays(-20)
        },
        new IsEmri
        {
            is_emri_id = 2,
            is_emri_no = "IE-2026-0002",
            tuketim_noktasi_id = 1002,
            sayac_id = 5002,
            tip = "Kesme",
            oncelik = "Normal",
            planlanan_tarih = new DateTime(2026, 4, 15),
            atanan_kullanici_id = 102,
            durum = "Devam Ediyor",
            status = "Active",
            CreatedAt = DateTime.Now.AddDays(-15)
        }
    };

    public List<IsEmri> GetAll()
    {
        return _isEmirleri.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public IsEmri? GetById(long id)
    {
        return _isEmirleri.FirstOrDefault(x => x.is_emri_id == id);
    }

    public List<IsEmri> Filtrele(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
    {
        var query = _isEmirleri.AsEnumerable();

        if (!string.IsNullOrEmpty(tip)) query = query.Where(x => x.tip == tip);
        if (!string.IsNullOrEmpty(durum)) query = query.Where(x => x.durum == durum);
        if (baslangic.HasValue) query = query.Where(x => x.planlanan_tarih >= baslangic.Value);
        if (bitis.HasValue) query = query.Where(x => x.planlanan_tarih <= bitis.Value);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var aramaLower = arama.ToLower();
            query = query.Where(x =>
                (x.is_emri_no != null && x.is_emri_no.ToLower().Contains(aramaLower)) ||
                x.tuketim_noktasi_id.ToString().Contains(aramaLower)
            );
        }

        return query.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public void TutanakKaydet(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks)
    {
        var isEmri = _isEmirleri.FirstOrDefault(x => x.is_emri_id == isEmriId);
        if (isEmri != null)
        {
            isEmri.tutanak_no = tutanakNo;
            isEmri.saha_sonucu = sahaSonucu;
            isEmri.gerekce = gerekce ?? "";
            isEmri.muhur_no = muhurNo ?? "";
            
            isEmri.kesme_endeksi = kesmeEndeksi;
            isEmri.acma_endeksi = acmaEndeksi;
            isEmri.eski_sayac_no = eskiSayacNo;
            isEmri.yeni_sayac_no = yeniSayacNo;
            isEmri.eski_son_endeksi = eskiSonEndeks;
            isEmri.yeni_ilk_endeksi = yeniIlkEndeks;

            isEmri.durum = "Tamamlandı";
            isEmri.UpdatedAt = DateTime.Now;
        }
    }
}
