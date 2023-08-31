using SustiVest.Data.Entities;
using SustiVest.Data.Services;



namespace SustiVest.Data.Services
{
    
    public static class Seeder
    {
        // use this class to seed the database with dummy test data using an IUserService and ICompanyService
        public static void Seed(IUserService userSVC, ICompanyService companySVC)
        {
            // seeder destroys and recreates the database - NOT to be called in production!!!
            userSVC.Initialise();

            // add users
            userSVC.AddUser("Administrator", "admin@mail.com", "admin", Role.admin);
            userSVC.AddUser("Manager", "manager@mail.com", "manager", Role.manager);
            userSVC.AddUser("Guest", "guest@mail.com", "guest", Role.guest);

            // add companies
            companySVC.AddCompany(new Company
            {
                CompanyName = "KaramSolar",
                Industry = "Solar Energy",
                RiskRating = 3,
                Tenor = 3,
                ROIdecimal = 0.12
            });

            companySVC.AddCompany(new Company
            {
                CompanyName = "Envirofone",
                Industry = "Retail Electronics",
                RiskRating = 4,
                Tenor = 1,
                ROIdecimal = 0.6
            });

            companySVC.AddCompany(new Company
            {
                CompanyName = "NileOptics",
                Industry = "Retail Optics",
                RiskRating = 5,
                Tenor = 2,
                ROIdecimal = 0.9
            });

        }
    }
}

