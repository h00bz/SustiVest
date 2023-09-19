using SustiVest.Data.Entities;

namespace SustiVest.Data.Services
{
    public interface IAssessmentsService
    {
        IList<Assessments> GetAssessments();
        Assessments GetAssessment(int requestNo, int analystNo);
        Assessments AddAssessment(Assessments assessment);
        Assessments UpdateAssessment(int requestNo, int analystNo, int sales, int ebitda, double dsr, double ccc, int riskRating, string marketPosition, string repaymentStatus, double financialLeverage, int workingCapital, int operatingAssets, string crNo, int totalAssets, int netEquity);
        bool DeleteAssessment(int requestNo, int analystNo);
        Paged<Assessments> GetAssessments(int page = 1, int size = 20, string orderBy = "CompanyName", string direction = "asc");
        IList<Assessments> GetAssessmentByCompanyName(string companyName);
    }


}