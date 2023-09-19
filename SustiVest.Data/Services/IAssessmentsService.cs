using SustiVest.Data.Entities;

namespace SustiVest.Data.Services
{
    public interface IAssessmentsService
    {
        IList<Assessments> GetAssessments();
        Assessments GetAssessment(int requestNo, int analystNo);
        Assessments AddAssessment(Assessments assessment);
        Assessments UpdateAssessment(Assessments updated);
        bool DeleteAssessment(int requestNo, int analystNo);
        Paged<Assessments> GetAssessments(int page = 1, int size = 20, string orderBy = "CompanyName", string direction = "asc");
        IList<Assessments> GetAssessmentByCompanyName(string companyName);
    }


}