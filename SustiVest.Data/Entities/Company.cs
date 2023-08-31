//using system; commented out 
using System.ComponentModel.DataAnnotations;
namespace SustiVest.Data.Entities
{
    public class Company
{
    public string CR_No { get; set; } //Commercial Registration Number

    public string TaxID { get; set; } //Tax ID Number

    // [Required]
    public string CompanyName { get; set; }

    // [Required]
    public string Industry { get; set; }

    // [Required]
    public DateOnly DateOfEstablishment { get; set; }

    public string Activity { get; set; }

    public enum CompanyType
    {
        SME,
        Startup
    }

    public CompanyType Type { get; set; } //Refers to company type SME or Startup
    public string ShareholderStructure {get; set;} //Represents Owner Equity Structure of a company
    
}
}