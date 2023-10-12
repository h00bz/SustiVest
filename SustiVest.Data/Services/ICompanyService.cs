using SustiVest.Data.Entities;

namespace SustiVest.Data.Services
{
    public interface ICompanyService
    {
        // Initialise the repository (database)  
        // void Initialise();

        IList<Company> GetCompanies();
        Paged<Company> GetCompanies(int page = 1, int size = 10, string orderBy = "companyname", string direction = "asc");
        Company GetCompany(string CRNo);
        Company GetCompanyByName(string CompanyName);
        Company AddCompany(Company c);
        Company Approve(Company c);
        Company UpdateCompany(Company c);
        bool DeleteCompany(string CRNo);
        IList<FinanceRequest> GetFinanceRequests();
        Paged<FinanceRequest> GetFinanceRequests(int page = 1, int size = 20, string orderBy = "Purpose", string direction = "asc");
        FinanceRequest GetFinanceRequest(int requestNo);
        FinanceRequest CreateRequest(FinanceRequest fr);
        FinanceRequest CloseRequest(int requestNo, string status);
        FinanceRequest UpdateRequest(FinanceRequest fr);
        FinanceRequest ResubmitRequest(int requestNo, DateOnly dateOfRequest, bool assessment);
        bool DeleteRequest(int RequestNo);


    }
}