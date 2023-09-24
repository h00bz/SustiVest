//using system; commented out 
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
namespace SustiVest.Data.Entities
{
    public class Company
    {
        public string CRNo { get; set; } //Commercial Registration Number

        public string TaxID { get; set; } //Tax ID Number

        // [Required]
        public string CompanyName { get; set; }

        // [Required]
        public string Industry { get; set; }

        // [Required]
        public DateOnly DateOfEstablishment { get; set; }

        public string Activity { get; set; }

        // public enum CompanyType
        // {
        //     SME,
        //     Startup
        // }

        public string Type { get; set; } //Refers to company type SME or Startup
        public string ShareholderStructure { get; set; } //Represents Owner Equity Structure of a company
        public int RepId { get; set; }

        public IList<FinanceRequest> FinanceRequests { get; set; } = new List<FinanceRequest>();

       [ForeignKey("RepId")]
        public User Rep { get; set; }

    }
}