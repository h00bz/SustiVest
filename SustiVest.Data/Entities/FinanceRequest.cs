//using system; commented out 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace SustiVest.Data.Entities
{
    public enum FinanceRequestStatus { OPEN, CLOSED, ALL }

    public class FinanceRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestNo { get; set; }

        // [Required]
        public string Purpose { get; set; }

        // [Required]
        public int Amount { get; set; }
        // [Required]
        public int Tenor { get; set; }

        // public enum FaciltyTypes
        // {
        //     OVD,
        //     STL,
        //     MTL,
        //     Discounting,
        //     Bonds,
        //     IPO,
        // }

        public string FacilityType { get; set; }   
        public string CRNo { get; set; }
        public string Status { get; set; }

        public DateOnly DateOfRequest { get; set; }

        public bool Assessment { get; set; }

        [ForeignKey("CRNo")]
        public Company Company { get; set; }
    }
}