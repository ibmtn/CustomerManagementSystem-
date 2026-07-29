using System.Collections.Generic;
using KcetasWeb.Models;

namespace KcetasWeb.Services.Interfaces
{
    public interface ITuketimNoktasiService
    {
        Task<int> GetTotalCountAsync();
        Task<List<TuketimNoktasi>> GetAllAsync();
        Task<TuketimNoktasi?> GetByIdAsync(string tekilKod);
        Task CreateAsync(TuketimNoktasi tuketimNoktasi);
        Task UpdateAsync(TuketimNoktasi tuketimNoktasi);
        Task DeleteAsync(string tekilKod);
    }
}
