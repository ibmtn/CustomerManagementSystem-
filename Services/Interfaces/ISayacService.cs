using System.Collections.Generic;
using KcetasWeb.Models;

namespace KcetasWeb.Services.Interfaces
{
    public interface ISayacService
    {
        Task<List<Sayac>> GetAllAsync();
        
        Task<PagedResponse<Sayac>> GetPagedAsync(
            int page, 
            int pageSize, 
            string? seriNo, 
            int? durum,
            long? tuketimNoktasiId = null);

        Task<Sayac?> GetByIdAsync(long id);
        Task<Sayac?> GetByTuketimNoktasiIdAsync(long tuketimNoktasiId);
        Task CreateAsync(Sayac sayac);
        Task UpdateAsync(Sayac sayac);
        Task DeleteAsync(long id);
    }
}
