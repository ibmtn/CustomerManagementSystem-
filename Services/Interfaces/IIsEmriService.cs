using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KcetasWeb.Services.Interfaces;

public interface IIsEmriService
{
    Task<int> GetTotalCountAsync();
    Task<List<IsEmri>> GetAllAsync();
    Task<IsEmri?> GetByIdAsync(long id);
    Task<List<IsEmri>> FiltreleAsync(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama);
    Task TutanakKaydetAsync(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks);
    Task<IsEmri> EkleAsync(IsEmri isEmri);
    Task DurumGuncelleAsync(long id, KcetasWeb.Models.Enums.IsEmriDurumu yeniDurum);
    Task PersonelAtaAsync(long id, long personelId);
}
