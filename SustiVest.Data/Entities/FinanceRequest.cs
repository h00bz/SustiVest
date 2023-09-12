//using system; commented out 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
namespace SustiVest.Data.Entities
{
    public enum FinanceRequestStatus { ASSESSED, PENDING, ALL }

    public class FinanceRequest
    {

        public int Request_No { get; set; }

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
        public string CR_No { get; set; }
        public string Status { get; set; }

        public DateOnly DateOfRequest { get; set; }

        public bool Assessment { get; set; }

        [ForeignKey("CR_No")]
        public Company Company { get; set; }
    }
}