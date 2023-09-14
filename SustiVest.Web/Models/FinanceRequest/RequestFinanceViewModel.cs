using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SustiVest.Data.Services;
using SustiVest.Data.Repositories;
using SustiVest.Data.Entities;

namespace SustiVest.Web.Models
{
    public class RequestFinanceViewModel
    {
        // selectlist of students (id, name)       
        public SelectList Companies { get; set; }

        // // Collecting StudentId and Issue in Form
        // [Required(ErrorMessage = "Please select a company")]
        // [Display(Name = "Select Company")]
        public string Request_No { get; set; }
        public string CR_No { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Purpose { get; set; }

        [Required]
        public int Amount{ get; set; }

         public int Tenor { get; set; }

        [Required]
        public string FacilityType { get; set; }   
        public string Status { get; set; }
        public DateOnly DateOfRequest {get; set;} = DateOnly.FromDateTime(DateTime.Now);

        public bool Assessment { get; set; }= false;

 

    }
}

