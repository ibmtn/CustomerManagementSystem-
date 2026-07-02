namespace KcetasWeb.Models.enums;

/// <summary>
/// Entegrasyon outbox durumları
/// </summary>
public enum OutboxDurumu
{
    Bekliyor = 1,
    Gonderildi = 2,
    Basarisiz = 3,
    IptalEdildi = 4
}
