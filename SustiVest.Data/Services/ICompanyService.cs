using SustiVest.Data.Entities;

namespace SustiVest.Data.Services
{
   public interface ICompanyService
{
    // Initialise the repository (database)  
    void Initialise();
 
    IList<Company> GetCompanies();
    IList<Company> GetCompanies(string order = null, string direction="asc");
    Company GetCompany(int id);
    Company GetCompanyByName(string companyName);
    Company AddCompany(Company c);
    Company UpdateCompany (Company updated);  
    bool DeleteCompany(int id);
} 
}