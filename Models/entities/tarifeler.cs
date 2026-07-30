namespace KcetasWeb.Models;

public class tarifeler
{
    public int tarife_id {get; set;}
    public string tarife_adi {get; set;}
    public string tarife_kodu {get; set;}
    public decimal gunduz_birim_fiyat {get; set;}
    public decimal puant_birim_fiyat {get; set;}
    public decimal gece_birim_fiyat {get; set;}
    public decimal kdv_orani {get; set;}

    public decimal dagitim_bedeli {get; set;}
    public Boolean aktif {get; set;}

    public DateTime created_at {get; set;}
    public DateTime updated_at {get; set;}
    

}
