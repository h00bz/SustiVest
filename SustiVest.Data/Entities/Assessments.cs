//using system; commented out 
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustiVest.Data.Entities
{
    public class Assessments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssessmentNo { get; set; }
        public int RequestNo { get; set; } 

        public int AnalystNo { get; set; } 

        public int Sales { get; set; }

        public int EBITDA { get; set; }

        public double DSR { get; set; }

        public double CCC { get; set; }

        public int RiskRating { get; set; }

        public string MarketPosition { get; set; }

        public string RepaymentStatus { get; set; }

        public double FinancialLeverage { get; set; }

        public int WorkingCapital { get; set; }

        public int OperatingAssets { get; set; }

        public string CRNo { get; set; }

        public int TotalAssets { get; set; }

        public int NetEquity { get; set; }

        // Navigation properties for foreign keys
        [ForeignKey("CRNo")]
        public Company Company { get; set; }

        // [ForeignKey("AnalystNo")]
        // public Analysts Analyst { get; set; }

        [ForeignKey("RequestNo")]
        public FinanceRequest FinanceRequest { get; set; }

        public ICollection<Offer> Offers { get; set; }
    }
}
