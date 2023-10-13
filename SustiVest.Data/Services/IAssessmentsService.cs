using SustiVest.Data.Entities;

namespace SustiVest.Data.Services
{
    public interface IAssessmentsService
    {
        IList<Assessments> GetAssessments();
        Assessments GetAssessment(int assessmentNo);
        Assessments GetAssessmentByRequestNo(int requestNo);
        Assessments AddAssessment(Assessments assessment);
        Assessments PostAssessment(int assessmentNo);
        Assessments UpdateAssessment( int assessmentNo, int requestNo, int analystNo, int sales, int ebitda, double dsr, double ccc, int riskRating, string marketPosition, string repaymentStatus, double financialLeverage, int workingCapital, int operatingAssets, string crNo, int totalAssets, int netEquity);
        bool DeleteAssessment(int assessmentNo);
        Paged<Assessments> GetAssessments(int page = 1, int size = 20, string orderBy = "CompanyName", string direction = "asc");
        IList<Assessments> GetAssessmentsByCompanyName(string companyName);
    }


}