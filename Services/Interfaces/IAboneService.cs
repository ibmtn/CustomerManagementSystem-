using System.Collections.Generic;
using MusteriYonetim.Models;

namespace MusteriYonetim.Services.Interfaces
{
    public interface IAboneService
    {
        System.Threading.Tasks.Task<List<Abone>> GetAllAsync();
        System.Threading.Tasks.Task<PaginatedResponse<Abone>> GetPagedAsync(int page, int pageSize);
        System.Threading.Tasks.Task<MusteriYonetim.Models.Dtos.PagedResultDto<MusteriYonetim.Models.Dtos.AboneListDto>> GetPagedCursorAsync(long? lastId, int limit);
        System.Threading.Tasks.Task<Abone> GetByIdAsync(int id);
        System.Threading.Tasks.Task CreateAsync(Abone abone);
        System.Threading.Tasks.Task UpdateAsync(Abone abone);
        System.Threading.Tasks.Task DeleteAsync(int id);
    }
}
