using KcetasWeb.Models;
using System;
using System.Collections.Generic;

namespace KcetasWeb.Services.Interfaces;

public interface IOutboxService
{
    List<EntegrasyonOutbox> GetAll();
    EntegrasyonOutbox? GetById(long id);
    List<EntegrasyonOutbox> Filtrele(string? durum, string? eventType, DateTime? baslangic, DateTime? bitis);
    (int Toplam, int Bekleyen, int Gonderilmis, int Basarisiz) GetIstatistikler();
    bool YenidenGonder(long id);
}
