using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Models.enums;

namespace KcetasWeb.Services.Interfaces;

/// <summary>
/// Saha iş emirleri servisi arayüzü.
/// CRUD, filtreleme ve tutanak kayıt işlemlerini tanımlar.
/// </summary>
public interface IIsEmriService
{
    /// <summary>
    /// Tüm iş emirlerini getirir.
    /// </summary>
    List<IsEmri> GetAll();

    /// <summary>
    /// Belirli bir iş emrini ID ile getirir.
    /// </summary>
    IsEmri? GetById(int id);

    /// <summary>
    /// İş emirlerini tip, durum, tarih aralığı ve arama metnine göre filtreler.
    /// </summary>
    List<IsEmri> Filtrele(IsEmriTipi? tip, IsEmriDurumu? durum, DateTime? baslangic, DateTime? bitis, string? arama);

    /// <summary>
    /// Saha tutanak bilgilerini kaydeder ve iş emrini günceller.
    /// </summary>
    void TutanakKaydet(int isEmriId, string tutanakNo, string sahaSonucu, string? gerekce,
        string? eskiSayacNo, decimal? eskiSonEndeksi,
        string? yeniSayacNo, decimal? yeniIlkEndeksi,
        string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi);
}
