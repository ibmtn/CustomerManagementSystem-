using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Models.enums;

namespace KcetasWeb.Services.Interfaces;

/// <summary>
/// Entegrasyon Outbox servisi arayüzü.
/// Dış sistem entegrasyonu mesaj kuyruğu üzerinde CRUD, filtreleme, istatistik ve yeniden gönderim işlemlerini tanımlar.
/// </summary>
public interface IOutboxService
{
    /// <summary>
    /// Tüm outbox kayıtlarını getirir.
    /// </summary>
    List<EntegrasyonOutbox> GetAll();

    /// <summary>
    /// Belirli bir outbox kaydını ID ile getirir.
    /// </summary>
    EntegrasyonOutbox? GetById(int id);

    /// <summary>
    /// Outbox kayıtlarını durum, işlem tipi ve tarih aralığına göre filtreler.
    /// </summary>
    List<EntegrasyonOutbox> Filtrele(OutboxDurumu? durum, string? islemTipi, DateTime? baslangic, DateTime? bitis);

    /// <summary>
    /// Outbox istatistiklerini döner: Toplam, Bekleyen, Gönderilmiş, Başarısız.
    /// </summary>
    (int Toplam, int Bekleyen, int Gonderilmis, int Basarisiz) GetIstatistikler();

    /// <summary>
    /// Başarısız bir kaydı yeniden gönderim için kuyruğa alır.
    /// </summary>
    bool YenidenGonder(int id);
}
