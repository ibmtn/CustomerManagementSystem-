namespace MusteriYonetim.Constants;

public static class IsEmriTipleri
{

    public const string SayacDegistirme = "DEGISTIRME";
    public const string SayacSokme = "SOKME";
    public const string EnerjiKesme = "KESME";
    public const string EnerjiAcma = "ACMA";
    public const string EndeksOkuma = "ENDEKS_OKUMA";
    public const string SayacAriza = "SAYAC_ARIZA";
    public const string Muhurleme = "MUHURLEME";
    public const string KesifInceleme = "KESIF_INCELEME";
    public const string YeniBaglanti = "YENI_BAGLANTI";

    public static string GetUIName(string tip)
    {
        return tip switch
        {
            SayacAriza => "Sayaç Arıza",
            SayacDegistirme => "Sayaç Değiştirme",
            "SAYAC DEGISIMI" => "Sayaç Değiştirme",
            EnerjiKesme => "Enerji Kesme",
            YeniBaglanti => "Yeni Bağlantı",

            SayacSokme => "Sayaç Sökme",
            EndeksOkuma => "Endeks Okuma",
            EnerjiAcma => "Enerji Açma",
            "ENERJI_ACMA" => "Enerji Açma",
            Muhurleme => "Mühürleme",
            KesifInceleme => "Keşif İnceleme",
            _ => string.IsNullOrEmpty(tip) ? "Belirtilmedi" : tip
        };
    }
}
