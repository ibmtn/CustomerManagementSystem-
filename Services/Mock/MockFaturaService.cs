using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Mock;

/// <summary>
/// Fatura simülasyon servisi mock implementasyonu.
/// Türkiye elektrik tarife yapısına uygun hesaplama yapar.
/// Tarife grupları: Mesken (2.85 TL/kWh), Ticarethane (3.45 TL/kWh), Sanayi (2.65 TL/kWh)
/// </summary>
public class MockFaturaService : IFaturaService
{
    // Tarife birim fiyatları (TL/kWh)
    private static readonly Dictionary<string, decimal> _tarifeFiyatlari = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Mesken", 2.85m },
        { "Ticarethane", 3.45m },
        { "Sanayi", 2.65m }
    };

    // Dağıtım bedeli birim fiyatı (TL/kWh)
    private const decimal DagitimBirimFiyat = 0.65m;

    // TRT payı oranı (enerji bedeli üzerinden %2)
    private const decimal TrtOrani = 0.02m;

    // Enerji fonu oranı (enerji bedeli üzerinden %1)
    private const decimal EnerjiFonuOrani = 0.01m;

    // KDV oranı (%20)
    private const decimal KdvOrani = 0.20m;

    /// <inheritdoc />
    public (decimal BirimFiyat, decimal EnerjiBedeli, decimal DagitimBedeli, decimal TrtPayi,
            decimal EnerjiFonu, decimal KdvTutari, decimal ToplamTutar,
            List<SimulasyonKalemDto> Kalemler) SimulasyonHesapla(string tarifeGrubu, decimal tuketimMiktari)
    {
        // Birim fiyat belirleme
        if (!_tarifeFiyatlari.TryGetValue(tarifeGrubu, out decimal birimFiyat))
        {
            birimFiyat = _tarifeFiyatlari["Mesken"]; // Varsayılan olarak mesken tarifesi
        }

        // Enerji bedeli = tüketim x birim fiyat
        decimal enerjiBedeli = Math.Round(tuketimMiktari * birimFiyat, 2);

        // Dağıtım bedeli = tüketim x dağıtım birim fiyatı
        decimal dagitimBedeli = Math.Round(tuketimMiktari * DagitimBirimFiyat, 2);

        // TRT payı = enerji bedeli x %2
        decimal trtPayi = Math.Round(enerjiBedeli * TrtOrani, 2);

        // Enerji fonu = enerji bedeli x %1
        decimal enerjiFonu = Math.Round(enerjiBedeli * EnerjiFonuOrani, 2);

        // KDV matrahı = enerji bedeli + dağıtım bedeli + TRT payı + enerji fonu
        decimal kdvMatrahi = enerjiBedeli + dagitimBedeli + trtPayi + enerjiFonu;

        // KDV tutarı = KDV matrahı x %20
        decimal kdvTutari = Math.Round(kdvMatrahi * KdvOrani, 2);

        // Toplam tutar
        decimal toplamTutar = Math.Round(kdvMatrahi + kdvTutari, 2);

        // Kalem detayları
        var kalemler = new List<SimulasyonKalemDto>
        {
            new SimulasyonKalemDto("Enerji Bedeli", tuketimMiktari, birimFiyat, enerjiBedeli),
            new SimulasyonKalemDto("Dağıtım Bedeli", tuketimMiktari, DagitimBirimFiyat, dagitimBedeli),
            new SimulasyonKalemDto("TRT Payı (%2)", enerjiBedeli, TrtOrani, trtPayi),
            new SimulasyonKalemDto("Enerji Fonu (%1)", enerjiBedeli, EnerjiFonuOrani, enerjiFonu),
            new SimulasyonKalemDto("KDV (%20)", kdvMatrahi, KdvOrani, kdvTutari)
        };

        return (birimFiyat, enerjiBedeli, dagitimBedeli, trtPayi, enerjiFonu, kdvTutari, toplamTutar, kalemler);
    }
}
