using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SustiVest.Data.Entities
{
    public class Company
    {
        [Key]
        public string CRNo { get; set; } //Commercial Registration Number
        public string TaxID { get; set; } //Tax ID Number

        public string CompanyName { get; set; }

        public string Industry { get; set; }

        public DateOnly DateOfEstablishment { get; set; }

        public string Activity { get; set; }


        public string Type { get; set; } //Refers to company type SME or Startup
        public string ShareholderStructure { get; set; } //Represents Owner Equity Structure of a company
        public int RepId { get; set; }
        
        public string ProfileStatus { get; set; }//Refers to company status (Active, Inactive, Pending, etc.)
        
        [JsonIgnore]
        public IList<FinanceRequest> FinanceRequests { get; set; } = new List<FinanceRequest>();

       [ForeignKey("RepId")]
        public User Rep { get; set; }

    }
}