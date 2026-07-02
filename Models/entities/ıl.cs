using KcetasWeb.Models.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace KcetasWeb.Models.entities;

public class ıl
{
    

    [Key]
    public int il_id { get; set; }

    [Required]
    [MaxLength(255)]
    public  string il_adi { get; set; }

    [Required]
    [MaxLength(255)]
    public  string plaka_kodu { get; set; }

    


}

