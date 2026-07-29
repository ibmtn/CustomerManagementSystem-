using KcetasWeb.Models;
using System.Collections.Generic;

namespace KcetasWeb.Services.Interfaces
{
    public interface IAuditLogService
    {
        System.Threading.Tasks.Task EkleAsync(string varlikTipi, int varlikId, string islemTipi, string eskiDeger, string yeniDeger, int kullaniciId, string islemGerekcesi = null);
        System.Threading.Tasks.Task<List<AuditLog>> GetirByVarlikAsync(string varlikTipi, int varlikId);
        System.Threading.Tasks.Task<PaginatedResponse<AuditLog>> GetAllAsync(int page = 1, int pageSize = 100);
    }
}
