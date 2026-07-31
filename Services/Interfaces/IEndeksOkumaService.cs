using KcetasWeb.Models;
using KcetasWeb.Models.Dtos;
using System;
using System.Collections.Generic;

namespace KcetasWeb.Services.Interfaces;

public class PagedResponse<T>
{
    [System.Text.Json.Serialization.JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("page")]
    public int Page { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();
}

public interface IEndeksOkumaService
{
    System.Threading.Tasks.Task<List<EndeksOkuma>> GetAllAsync();

    System.Threading.Tasks.Task<EndeksOkuma?> GetByIdAsync(long id);

    System.Threading.Tasks.Task<List<EndeksOkuma>> FiltreleAsync(
        string? okumaTipi,
        string? durum,
        DateTime? baslangic,
        DateTime? bitis,
        string? arama);

    System.Threading.Tasks.Task<PagedResponse<EndeksOkuma>> GetPagedAsync(
        int page, 
        int pageSize,
        string? okumaTipi,
        string? durum,
        DateTime? baslangic,
        DateTime? bitis,
        string? aramaMetni,
        string? sayacId,
        string? donem,
        string? dogrulamaDurumu,
        string? tuketimNoktasi,
        string? abone,
        long? okumaNo);

    System.Threading.Tasks.Task<List<YeniOkumaSecimDto>> YeniOkumaSecimAraAsync(string? q, int page = 1, int pageSize = 20);

    System.Threading.Tasks.Task<(int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim)>
        GetIstatistiklerAsync(string? donem = null);

    System.Threading.Tasks.Task CreateAsync(EndeksOkuma model);
    
    System.Threading.Tasks.Task UpdateAsync(EndeksOkuma model);
    
    System.Threading.Tasks.Task DeleteAsync(long id);
}
