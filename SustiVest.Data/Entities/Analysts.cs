    //using system; commented out 
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;
    using SustiVest.Data.Entities;
    using System.ComponentModel.DataAnnotations.Schema;

namespace SustiVest.Data.Entities // Replace with your actual namespace
{
    public class Analysts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnalystNo { get; set; } // PRIMARY KEY

        public string Email { get; set; }

        public string Name { get; set; }

        public decimal PhoneNo { get; set; }

        public IList<Assessments> Assessments { get; set; } = new List<Assessments>();
    }
}
