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
            int? durum);

        Task<Sayac?> GetByIdAsync(long id);
        Task CreateAsync(Sayac sayac);
        Task UpdateAsync(Sayac sayac);
        Task DeleteAsync(long id);
    }
}
