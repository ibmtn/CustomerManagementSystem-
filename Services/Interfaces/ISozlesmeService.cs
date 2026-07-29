using System.Collections.Generic;
using System.Threading.Tasks;
using KcetasWeb.Models;

namespace KcetasWeb.Services.Interfaces
{
    public interface ISozlesmeService
    {
        Task<List<Sozlesme>> GetAllAsync();
        Task<int> GetTotalCountAsync();
        Task<PaginatedResponse<Sozlesme>> GetPagedAsync(int page, int pageSize);
        Task<Sozlesme?> GetByIdAsync(string sozlesmeNo);
        Task CreateAsync(Sozlesme sozlesme);
        Task UpdateAsync(Sozlesme sozlesme);
        Task DeleteAsync(string sozlesmeNo);
    }
}
