//using system; commented out 
using System.ComponentModel.DataAnnotations;
namespace SustiVest.Data.Entities
{
    public class Company
{
    public int Id { get; set; }

    // [Required]
    public string CompanyName { get; set; }

    // [Required]
    public string Industry { get; set; }

    // [Required]
    public int RiskRating { get; set; }

    
    // [Required]
    public int Tenor { get; set; }

    // [Required]
    public double ROIdecimal { get; set; }
    // [Url] [UrlResource]
    // public string PhotoUrl { get; set; }
}
}