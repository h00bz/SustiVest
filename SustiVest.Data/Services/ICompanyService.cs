using SustiVest.Data.Entities;

namespace SustiVest.Data.Services
{
   public interface ICompanyService
{
    // Initialise the repository (database)  
    // void Initialise();
 
    IList<Company> GetCompanies();
    IList<Company> GetCompanies(string order = null, string direction="asc");
    Company GetCompany(string CR_No);
    Company GetCompanyByName(string companyName);
    Company AddCompany(Company c);
    Company UpdateCompany (Company updated);  
    bool DeleteCompany(string CR_No);
} 
}