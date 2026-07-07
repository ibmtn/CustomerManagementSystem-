using KcetasWeb.Models;
using System;
using System.Collections.Generic;

namespace KcetasWeb.Services.Interfaces;

public interface IIsEmriService
{
    List<IsEmri> GetAll();
    IsEmri? GetById(long id);
    List<IsEmri> Filtrele(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama);
    void TutanakKaydet(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks);
}
