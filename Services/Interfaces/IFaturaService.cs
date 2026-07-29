namespace KcetasWeb.Services.Interfaces;

/// <summary>
/// Fatura simülasyon kalemi DTO'su.
/// Her bir fatura kaleminin adı, miktarı, birim fiyatı ve tutarını içerir.
/// </summary>
public record SimulasyonKalemDto(string KalemAdi, decimal Miktar, decimal BirimFiyat, decimal Tutar);

/// <summary>
/// Fatura servisi arayüzü.
/// Tarife grubuna ve tüketim miktarına göre fatura simülasyonu hesaplar.
/// </summary>
public interface IFaturaService
{
    /// <summary>
    /// Verilen tarife grubu ve tüketim miktarına göre fatura simülasyonu hesaplar.
    /// Tüm kalem detaylarını ve toplam tutarı döner.
    /// </summary>
    /// <param name="tarifeGrubu">Tarife grubu: Mesken, Ticarethane, Sanayi</param>
    /// <param name="tuketimMiktari">Tüketim miktarı (kWh)</param>
    /// <returns>Hesaplanan fatura kalemleri ve toplam tutarlar</returns>
    System.Threading.Tasks.Task<(decimal BirimFiyat, decimal EnerjiBedeli, decimal DagitimBedeli, decimal TrtPayi,
     decimal EnerjiFonu, decimal KdvTutari, decimal ToplamTutar,
     System.Collections.Generic.List<SimulasyonKalemDto> Kalemler)> SimulasyonHesaplaAsync(string tarifeGrubu, decimal tuketimMiktari);

    System.Threading.Tasks.Task<System.Collections.Generic.List<KcetasWeb.Models.Fatura>> GetAllAsync();
    
    System.Threading.Tasks.Task<PagedResponse<KcetasWeb.Models.Fatura>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? faturaNo = null, 
        int? durum = null, 
        long? sozlesmeId = null);

    System.Threading.Tasks.Task<KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.FaturaListDto>> GetPagedCursorAsync(long? lastId, int limit);
    
    System.Threading.Tasks.Task<KcetasWeb.Models.Fatura?> GetByIdAsync(int id);
    
    System.Threading.Tasks.Task<KcetasWeb.Models.Fatura> EkleAsync(KcetasWeb.Models.Fatura fatura);
    
    System.Threading.Tasks.Task GuncelleAsync(KcetasWeb.Models.Fatura fatura);
    
    System.Threading.Tasks.Task SilAsync(int id);
}
