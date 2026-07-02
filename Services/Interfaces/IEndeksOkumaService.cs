using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Models.enums;

namespace KcetasWeb.Services.Interfaces;

/// <summary>
/// Endeks okuma servisi arayüzü.
/// Sayaç okuma verileri üzerinde CRUD, filtreleme ve istatistik işlemlerini tanımlar.
/// </summary>
public interface IEndeksOkumaService
{
    /// <summary>
    /// Tüm endeks okumalarını getirir.
    /// </summary>
    List<EndeksOkuma> GetAll();

    /// <summary>
    /// Belirli bir okumayı ID ile getirir.
    /// </summary>
    EndeksOkuma? GetById(int id);

    /// <summary>
    /// Okumaları kaynak, durum, tarih aralığı ve arama metnine göre filtreler.
    /// </summary>
    List<EndeksOkuma> Filtrele(OkumaKaynagi? kaynak, OkumaDurumu? durum, DateTime? baslangic, DateTime? bitis, string? arama);

    /// <summary>
    /// Okuma istatistiklerini döner: Toplam, Manuel sayısı, OSOS sayısı, Anomali sayısı, Ortalama Tüketim.
    /// </summary>
    (int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim) GetIstatistikler();
}
