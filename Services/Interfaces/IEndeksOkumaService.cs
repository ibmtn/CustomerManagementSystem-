using KcetasWeb.Models;
using System;
using System.Collections.Generic;

namespace KcetasWeb.Services.Interfaces;

public interface IEndeksOkumaService
{
    List<EndeksOkuma> GetAll();
    EndeksOkuma? GetById(long id);
    List<EndeksOkuma> Filtrele(string? okumaTipi, string? durum, DateTime? baslangic, DateTime? bitis, string? arama);
    (int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim) GetIstatistikler();
}
