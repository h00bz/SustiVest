using SustiVest.Data.Entities;

namespace SustiVest.Data.Services
{
   public interface ICompanyService
{
    // Initialise the repository (database)  
    // void Initialise();
 
    IList<Company> GetCompanies();
    Paged<Company> GetCompanies(int page = 1, int size = 10, string orderBy = "companyname", string direction = "asc");
    Company GetCompany(string CR_No);
    Company GetCompanyByName(string CompanyName);
    Company AddCompany(Company c);
    Company UpdateCompany (Company updated);  
    bool DeleteCompany(string CR_No);
    IList <FinanceRequest> GetFinanceRequests();
    IList <FinanceRequest> GetOpenRequests();
    FinanceRequest GetFinanceRequest(int request_No);
    FinanceRequest CreateRequest(string purpose, int amount, int tenor, string facilityType, string companyCR,  string status,  DateOnly dateOfRequest, bool assessment);
    FinanceRequest CloseRequest(int request_No, string status);
    FinanceRequest UpdateRequest(int request_No, string purpose, int amount, int tenor, string facilityType, string status, DateOnly dateOfRequest, bool assessment);
    FinanceRequest ResubmitRequest(int request_No, DateOnly dateOfRequest, bool assessment);
    bool DeleteRequest(int Request_No);

} 
}