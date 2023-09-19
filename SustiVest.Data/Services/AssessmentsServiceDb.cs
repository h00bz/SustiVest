using System;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;
using SustiVest.Data.Security;
using SustiVest.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql.Internal.TypeHandlers.NetworkHandlers;

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
            return ctx.Assessments.ToList();

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

        public Assessments GetAssessment(int requestNo, int analystNo)
        {
            return ctx.Assessments
            .Include(a => a.Company)
            .Include(a => a.Analyst)
            .FirstOrDefault(a => a.RequestNo == requestNo && a.AnalystNo == analystNo);
        }

        public Assessments AddAssessment(Assessments a)
        {
            // Check if an assessment with the same RequestNo and AnalystNo exists
            var exists = GetAssessment(a.RequestNo, a.AnalystNo);
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

        public Assessments UpdateAssessment(Assessments updated)
        {
            var assessment = GetAssessment(updated.RequestNo, updated.AnalystNo);
            if (assessment == null)
            {
                return null;
            }

            // Check if an assessment with the same RequestNo and AnalystNo exists
            var exists = GetAssessment(updated.RequestNo, updated.AnalystNo);
            if (exists != null && exists.RequestNo != updated.RequestNo)
            {
                return null; // Assessment with the same RequestNo and AnalystNo already exists
            }

            // Update the assessment properties
            assessment.Sales = updated.Sales;
            assessment.EBITDA = updated.EBITDA;
            assessment.DSR = updated.DSR;
            assessment.CCC = updated.CCC;
            assessment.RiskRating = updated.RiskRating;
            assessment.MarketPosition = updated.MarketPosition;
            assessment.RepaymentStatus = updated.RepaymentStatus;
            assessment.FinancialLeverage = updated.FinancialLeverage;
            assessment.WorkingCapital = updated.WorkingCapital;
            assessment.OperatingAssets = updated.OperatingAssets;
            assessment.CRNo = updated.CRNo;
            assessment.TotalAssets = updated.TotalAssets;
            assessment.NetEquity = updated.NetEquity;
            // Update other properties as needed

            ctx.SaveChanges();
            return assessment;
        }


        public bool DeleteAssessment(int requestNo, int analystNo)
        {
            var assessment = GetAssessment(requestNo, analystNo);
            if (assessment == null)
            {
                return false;
            }

            ctx.Assessments.Remove(assessment);
            ctx.SaveChanges();
            return true;
        }

        public IList<Assessments> GetAssessmentByCompanyName(string companyName)
        {
            return ctx.Assessments
                .Where(a => a.Company.CompanyName == companyName)
                .ToList();
        }


    }
}