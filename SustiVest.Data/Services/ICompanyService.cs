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
} 
}