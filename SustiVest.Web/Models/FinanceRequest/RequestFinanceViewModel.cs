using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SustiVest.Web.Models
{
    public class RequestFinanceViewModel
    {
        // selectlist of students (id, name)       
        public SelectList Companies { get; set; }

        // Collecting StudentId and Issue in Form
        [Required(ErrorMessage = "Please select a company")]
        [Display(Name = "Select Company")]
        public string CompanyCR_No { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Purpose { get; set; }

        [Required]
        public int Amount{ get; set; }

        [Required]
        public string FacilityType { get; set; }   
        public string Status { get; set; }
        public DateOnly DateOfRequest {get; set;} = DateOnly.FromDateTime(DateTime.Now);

        public bool Assessment { get; set; }= false;

        public int Tenor { get; set; }

    }
}

