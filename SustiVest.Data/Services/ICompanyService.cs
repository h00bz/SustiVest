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
        Company AddCompany(string crNo, string taxID, string companyName, string industry, DateOnly dateOfEstablishment, string activity, string type, string shareholderStructure, int repId);
        Company UpdateCompany(string crNo, string taxID, string companyName, string industry, DateOnly dateOfEstablishment, string activity, string type, string shareholderStructure, int repId);
        bool DeleteCompany(string CRNo);
        IList<FinanceRequest> GetFinanceRequests();

        Paged<FinanceRequest> GetFinanceRequests(int page = 1, int size = 20, string orderBy = "Purpose", string direction = "asc");
        // IList<FinanceRequest> GetOpenRequests();
        FinanceRequest GetFinanceRequest(int requestNo);
        FinanceRequest CreateRequest(FinanceRequest fr);
        FinanceRequest CloseRequest(int requestNo, string status);
        FinanceRequest UpdateRequest(int requestNo, string purpose, int amount, int tenor, string facilityType, string status, DateOnly dateOfRequest, bool assessment);
        FinanceRequest ResubmitRequest(int requestNo, DateOnly dateOfRequest, bool assessment);
        bool DeleteRequest(int RequestNo);
        // IList<Company> SearchCompanies(string query, string orderBy = "companyname", string direction = "asc");
        // public bool IsUserAuthorizedToEditCompanyProfile(string crNo, int userId);

    }
}