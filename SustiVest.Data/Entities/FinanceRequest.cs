using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SustiVest.Data.Entities
{
    public enum FinanceRequestStatus { OPEN, CLOSED, ALL }

    public class FinanceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestNo { get; set; }

        [Required]
        public string Purpose { get; set; }

        [Required]
        public int Amount { get; set; }
        [Required]
        public int Tenor { get; set; }

        public string FacilityType { get; set; }
        public string CRNo { get; set; }
        public string Status { get; set; }

        public DateOnly DateOfRequest { get; set; }

        public bool Assessment { get; set; } 
       
        [ForeignKey("CRNo")]
        public Company Company { get; set; }

        // [NotMapped]
        // public int RepId
        // {
        //     get { return Company != null ? Company.RepId : 0; }
        //     set { if (Company != null) Company.RepId = value; }
        // }
        
        // [JsonIgnore]
        // [NotMapped]
        // public User Rep
        // {
        //     get { return Company.Rep; }
        //     set { Company.Rep = value; }
        // }

        // public FinanceRequest()
        // {
        //     if (Company != null)
        //     {
        //         Rep = Company.Rep;
        //     }
        // }
    }
}           