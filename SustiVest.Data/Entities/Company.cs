using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SustiVest.Data.Entities
{
    public class Company
    {
        [Required (ErrorMessage = "Field cannot be left empty.")]
        [Key]
        public string CRNo { get; set; } //Commercial Registration Number
        [Required (ErrorMessage = "Field cannot be left empty.")]
        public string TaxID { get; set; } //Tax ID Number

        [Required (ErrorMessage = "Field cannot be left empty.")]
        public string CompanyName { get; set; }

        [Required (ErrorMessage = "Field cannot be left empty.")]
        public string Industry { get; set; }

        [Required (ErrorMessage = "Field cannot be left empty.")]
        public DateOnly DateOfEstablishment { get; set; }

        [Required (ErrorMessage = "Field cannot be left empty.")]
        public string Activity { get; set; }


        [Required (ErrorMessage = "Field cannot be left empty.")]
        public string Type { get; set; } //Refers to company type SME or Startup
        public string ShareholderStructure { get; set; } //Represents Owner Equity Structure of a company
        [Required (ErrorMessage = "Field cannot be left empty.")]
        public int RepId { get; set; }
        
        [JsonIgnore]
        public IList<FinanceRequest> FinanceRequests { get; set; } = new List<FinanceRequest>();

       [ForeignKey("RepId")]
        public User Rep { get; set; }

    }
}