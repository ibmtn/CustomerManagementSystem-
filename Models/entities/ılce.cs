using KcetasWeb.Models.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace KcetasWeb.Models.entities;

public class ılce
{
    

    [Key]
    public int ilce_id { get; set; }

    [Required]
    [MaxLength(255)]
    public  string il_id { get; set; }

    [Required]
    [MaxLength(255)]
    public  string ilce_adi { get; set; }

    


}
