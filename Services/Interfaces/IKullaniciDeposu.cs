using KcetasWeb.Models.entities;

namespace KcetasWeb.Services.Interfaces
{
    public interface IKullaniciDeposu
    {
        Task<bool> KullaniciAdiVarMiAsync(string kullaniciAdi);
        Task<bool> EPostaVarMiAsync(string ePosta);
        Task<Kullanici> EkleAsync(Kullanici kullanici);
        Task<bool> GirisKontrolAsync(string kullaniciAdi, string sifre);
        Task<Kullanici?> BulKullaniciAdiIleAsync(string kullaniciAdi);
        Task<List<Kullanici>> ListeleAsync();
        Task<Kullanici?> BulIdAsync(long id);
        Task<bool> GuncelleAsync(Kullanici kullanici);
        Task<bool> SilAsync(long id);
    }
}