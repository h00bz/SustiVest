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
        Company AddCompany(string crNo, string taxID, string companyName, string industry, DateOnly dateOfEstablishment, string activity, string type, string shareholderStructure);
        Company UpdateCompany(string crNo, string taxID, string companyName, string industry, DateOnly dateOfEstablishment, string activity, string type, string shareholderStructure);
        bool DeleteCompany(string CRNo);
        IList<FinanceRequest> GetFinanceRequests();
        IList<FinanceRequest> GetOpenRequests();
        FinanceRequest GetFinanceRequest(int requestNo);
        FinanceRequest CreateRequest(FinanceRequest fr);
        FinanceRequest CloseRequest(int requestNo, string status);
        FinanceRequest UpdateRequest(int requestNo, string purpose, int amount, int tenor, string facilityType, string status, DateOnly dateOfRequest, bool assessment);
        FinanceRequest ResubmitRequest(int requestNo, DateOnly dateOfRequest, bool assessment);
        bool DeleteRequest(int RequestNo);
        public bool IsUserAuthorizedToEditCompanyProfile(string crNo, int userId);

    }
}