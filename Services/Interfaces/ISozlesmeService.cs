using System.Collections.Generic;
using System.Threading.Tasks;
using MusteriYonetim.Models;

namespace MusteriYonetim.Services.Interfaces
{
    public interface ISozlesmeService
    {
        Task<List<Sozlesme>> GetAllAsync();
        Task<int> GetTotalCountAsync();
        Task<PaginatedResponse<Sozlesme>> GetPagedAsync(int page, int pageSize, string? q = null, string? durum = null, string? tekilKod = null, string? sozlesmeTipi = null);
        Task<Sozlesme?> GetByIdAsync(string sozlesmeNo);
        Task<Sozlesme?> GetByIdAsync(long id);
        Task CreateAsync(Sozlesme sozlesme);
        Task UpdateAsync(Sozlesme sozlesme);
        Task DeleteAsync(string sozlesmeNo);
    }
}
