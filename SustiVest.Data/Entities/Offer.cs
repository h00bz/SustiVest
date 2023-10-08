using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage;


namespace SustiVest.Data.Entities
{
    public class Offer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OfferId { get; set; }
        public int RequestNo { get; set; }

        public string CRNo { get; set; }

        public int Amount { get; set; }

        public int Tenor { get; set; }

        public string Payback { get; set; }

        public string Linens { get; set; }

        public string Undertakings { get; set; }

        public string Covenants { get; set; }

        public double ROR { get; set; }

        public string FacilityType { get; set; }

        public string UtilizationMechanism { get; set; }

        public int AnalystNo { get; set; }

        public int AssessmentNo { get; set; }
        [ForeignKey("CRNo")]
        public Company Company { get; set; }
        [ForeignKey("AnalystNo")]
        public FinanceRequest FinanceRequest { get; set; }
        [ForeignKey("AssessmentNo")]
        public Assessments Assessments { get; set; }
    }
}
