using SustiVest.Data.Entities;
using SustiVest.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SustiVest.Data.Services
{
    public class AssessmentServiceDb : IAssessmentsService
    {
        private readonly DatabaseContext ctx;

        public AssessmentServiceDb(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }

        public IList<Assessments> GetAssessments()
        {
            return ctx.Assessments
            .Include(a=>a.Company)
            .ToList();

        }

        public Paged<Assessments> GetAssessments(int page = 1, int size = 20, string orderBy = "CompanyName", string direction = "asc")
        {
            var results = (orderBy.ToLower(), direction.ToLower()) switch
            {
                ("companyname", "asc") => ctx.Assessments.OrderBy(a => a.Company.CompanyName),
                ("companyname", "desc") => ctx.Assessments.OrderByDescending(a => a.Company.CompanyName),
                ("requestno", "asc") => ctx.Assessments.OrderBy(a => a.RequestNo),
                ("requestno", "desc") => ctx.Assessments.OrderByDescending(a => a.RequestNo),
                ("sales", "asc") => ctx.Assessments.OrderBy(a => a.Sales),
                ("sales", "desc") => ctx.Assessments.OrderByDescending(a => a.Sales),
                ("riskrating", "asc") => ctx.Assessments.OrderBy(a => a.RiskRating),
                ("riskrating", "desc") => ctx.Assessments.OrderByDescending(a => a.RiskRating),
                ("repaymentstatus", "asc") => ctx.Assessments.OrderBy(a => a.RepaymentStatus),
                ("repaymentstatus", "desc") => ctx.Assessments.OrderByDescending(a => a.RepaymentStatus),
                ("totalassets", "asc") => ctx.Assessments.OrderBy(a => a.TotalAssets),
                ("totalassets", "desc") => ctx.Assessments.OrderByDescending(a => a.TotalAssets),
                ("netequity", "asc") => ctx.Assessments.OrderBy(a => a.NetEquity),
                ("netequity", "desc") => ctx.Assessments.OrderByDescending(a => a.NetEquity),
                _ => ctx.Assessments.OrderBy(a => a.RequestNo)
            };

            return results.Include(a => a.Company).ToPaged(page, size, orderBy, direction);
        }

        public Assessments GetAssessment(int assessmentNo)
        {
            return ctx.Assessments
            .Include(a => a.Company)
    
            .FirstOrDefault(a => a.AssessmentNo == assessmentNo);
        }

        public Assessments GetAssessmentByRequestNo(int requestNo)
        {
            return ctx.Assessments
            .Include(a => a.FinanceRequest)
            .FirstOrDefault(a => a.RequestNo == requestNo);
        }

        public Assessments AddAssessment(Assessments a)
        {
            // Check if an assessment with the same RequestNo and AnalystNo exists
            var exists = GetAssessment(a.AssessmentNo);
            if (exists != null)
            {
                return null; // Assessment with the same RequestNo and AnalystNo already exists
            }

            var assessment = new Assessments
            {
                RequestNo = a.RequestNo,
                AnalystNo = a.AnalystNo,
                Sales = a.Sales,
                EBITDA = a.EBITDA,
                DSR = a.DSR,
                CCC = a.CCC,
                RiskRating = a.RiskRating,
                MarketPosition = a.MarketPosition,
                RepaymentStatus = a.RepaymentStatus,
                FinancialLeverage = a.FinancialLeverage,
                WorkingCapital = a.WorkingCapital,
                OperatingAssets = a.OperatingAssets,
                CRNo = a.CRNo,
                TotalAssets = a.TotalAssets,
                NetEquity = a.NetEquity
                // Populate other properties as needed
            };

            ctx.Assessments.Add(assessment);
            ctx.SaveChanges();
            return assessment;
        }

        public Assessments UpdateAssessment(int assessmentNo, int requestNo, int analystNo, int sales, int ebitda, double dsr, double ccc, int riskRating, string marketPosition, string repaymentStatus, double financialLeverage, int workingCapital, int operatingAssets, string crNo, int totalAssets, int netEquity)
        {
            var assessment = GetAssessment(assessmentNo);
            if (assessment == null)
            {
                return null;
            }

            // Check if an assessment with the same RequestNo and AnalystNo exists
            var exists = GetAssessment(assessment.AssessmentNo);
            if (exists != null && exists.AssessmentNo != assessmentNo)
            {
                return null; // Assessment with the same RequestNo and AnalystNo already exists
            }
      

            // Update the assessment properties
            assessment.Sales = sales;
            assessment.EBITDA = ebitda;
            assessment.DSR = dsr;
            assessment.CCC = ccc;
            assessment.RiskRating = riskRating;
            assessment.MarketPosition = marketPosition;
            assessment.RepaymentStatus = repaymentStatus;
            assessment.FinancialLeverage = financialLeverage;
            assessment.WorkingCapital = workingCapital;
            assessment.OperatingAssets = operatingAssets;
            assessment.CRNo = crNo;
            assessment.TotalAssets = totalAssets;
            assessment.NetEquity = netEquity;
            // Update other properties as needed
    

            ctx.SaveChanges();
            return assessment;
        }


        public bool DeleteAssessment(int assessmentNo)
        {
            var assessment = GetAssessment(assessmentNo);
            if (assessment == null)
            {
                return false;
            }

            ctx.Assessments.Remove(assessment);
            ctx.SaveChanges();
            return true;
        }

        public IList<Assessments> GetAssessmentsByCompanyName(string companyName)
        {
            return ctx.Assessments
                .Where(a => a.Company.CompanyName == companyName)
                .ToList();
        }




    }
}