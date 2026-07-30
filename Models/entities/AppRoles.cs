namespace KcetasWeb.Models.entities
{
    // Rol adlarını tek bir yerden yönetmek için sabitler.
    // [Authorize(Roles = "...")] attribute'larında ve Claim atarken bunlar kullanılır.
    public static class AppRoles
    {
        public const string BTYoneticisi = "BTYoneticisi";
        public const string MusteriTemsilcisi = "MusteriTemsilcisi";
        public const string SozlesmeYetkilisi = "SozlesmeYetkilisi";
        public const string SayacOkumaPersoneli = "SayacOkumaPersoneli";
        public const string SahaOperasyonAmiri = "SahaOperasyonAmiri";
        public const string FaturalamaUzmani = "FaturalamaUzmani";
        public const string Denetci = "Denetci";
    }
}
