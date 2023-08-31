using System;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;
using SustiVest.Data.Security;
using SustiVest.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace SustiVest.Data.Services
{

    public class CompanyServiceDb : ICompanyService
    {
        private readonly DatabaseContext  ctx;
      
        public CompanyServiceDb(DatabaseContext ctx) 
        {
            this.ctx = ctx; 
        }

        public void Initialise()
        {
           ctx.Initialise(); 
        }

    

  // ==================== Company Management =======================


        public IList<Company> GetCompanies()
        {
            return ctx.Companies.ToList();
        }

        public IList<Company> GetCompanies(string order = null, string direction = "asc")
        {
            var results = (order.ToLower(), direction.ToLower()) switch
            {
                ("id", "asc") => ctx.Companies.OrderBy(c => c.Id),
                ("id", "desc") => ctx.Companies.OrderByDescending(c => c.Id),

                ("name", "asc") => ctx.Companies.OrderBy(c => c.CompanyName),
                ("name", "desc") => ctx.Companies.OrderByDescending(c => c.CompanyName),

                ("industry", "asc") => ctx.Companies.OrderBy(c => c.Industry),
                ("industry", "desc") => ctx.Companies.OrderByDescending(c => c.Industry),

                ("riskrating", "asc") => ctx.Companies.OrderBy(c => c.RiskRating),
                ("riskrating", "desc") => ctx.Companies.OrderByDescending(c => c.RiskRating),

                ("tenor", "asc") => ctx.Companies.OrderBy(c => c.Tenor),
                ("tenor", "desc") => ctx.Companies.OrderByDescending(c => c.Tenor),

                ("roidecimal", "asc") => ctx.Companies.OrderBy(c => c.ROIdecimal),
                ("roidecimal", "desc") => ctx.Companies.OrderByDescending(c => c.ROIdecimal),

                _                      => ctx.Companies.OrderBy(c => c.Id)
            };
            return results.ToList();
        }

        // Retrieve Company by Id 
        public Company GetCompany(int id)
        {
            return ctx.Companies.FirstOrDefault(c => c.Id == id);
                    //  .Include(s => s.Tickets)             
        }

        // Add a new company
        public Company AddCompany(Company c)
        {
            // check if company with name exists            
            var exists = GetCompanyByName(c.CompanyName);
            if (exists != null)
            {
                return null;
            }
            // check Tenor is valid
            if (c.Tenor > 7)
            {
                return null;
            }

            // create new company
            var company = new Company
            {
                CompanyName = c.CompanyName,
                Industry = c.Industry,
                RiskRating = c.RiskRating,
                Tenor = c.Tenor,
                ROIdecimal = c.ROIdecimal,
            };

            ctx.Companies.Add(company); // add company to the list
            ctx.SaveChanges();
            return company; // return newly added company
        }

        // Delete the company identified by Id returning true if 
        // deleted and false if not found
        public bool DeleteCompany(int id)
        {
            var c = GetCompany(id);
            if (c == null)
            {
                return false;
            }
            ctx.Companies.Remove(c);
            ctx.SaveChanges();
            return true;
        }

        // Update the company with the details in updated 
        public Company UpdateCompany(Company updated)
        {
            // verify the commpany exists 
            var company = GetCompany(updated.Id);
            if (company == null)
            {
                return null;
            }

            // verify name is still unique
            var exists = GetCompanyByName(updated.CompanyName);
            if (exists != null && exists.Id != updated.Id)
            {
                return null;
            }

            // verify tenor is valid
            if (updated.Tenor > 7)
            {
                return null;
            }

            // update the details of the company retrieved and save
            company.CompanyName = updated.CompanyName;
            company.Industry = updated.Industry;
            company.RiskRating = updated.RiskRating;
            company.Tenor = updated.Tenor;
            company.ROIdecimal = updated.ROIdecimal;

            ctx.SaveChanges();
            return company;
        }

        public Company GetCompanyByName(string name)
        {
            return ctx.Companies.FirstOrDefault(c => c.CompanyName == name);
        }
    }
}