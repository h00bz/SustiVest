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
        Company UpdateCompany(Company updated);
        bool DeleteCompany(string CRNo);
        IList<FinanceRequest> GetFinanceRequests();
        IList<FinanceRequest> GetOpenRequests();
        FinanceRequest GetFinanceRequest(int requestNo);
        FinanceRequest CreateRequest(string purpose, int amount, int tenor, string facilityType, string crNo, string status, DateOnly dateOfRequest, bool assessment);
        FinanceRequest CloseRequest(int requestNo, string status);
        FinanceRequest UpdateRequest(int requestNo, string purpose, int amount, int tenor, string facilityType, string status, DateOnly dateOfRequest, bool assessment);
        FinanceRequest ResubmitRequest(int requestNo, DateOnly dateOfRequest, bool assessment);
        bool DeleteRequest(int RequestNo);

    }
}