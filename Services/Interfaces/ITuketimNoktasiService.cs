using System.Collections.Generic;
using KcetasWeb.Models;

namespace KcetasWeb.Services.Interfaces
{
    public interface ITuketimNoktasiService
    {
        Task<int> GetTotalCountAsync();
        Task<PaginatedResponse<TuketimNoktasi>> GetPagedAsync(int page, int pageSize, string? q = null, string? baglantiDurumu = null);
        Task<List<TuketimNoktasi>> GetAllAsync();
        Task<TuketimNoktasi?> GetByIdAsync(string tekilKod);
        Task<TuketimNoktasi?> GetByIdAsync(long id);
        Task CreateAsync(TuketimNoktasi tuketimNoktasi);
        Task UpdateAsync(TuketimNoktasi tuketimNoktasi);
        Task DeleteAsync(string tekilKod);
    }
}
