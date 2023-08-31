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

        // public void Initialise()
        // {
        //    ctx.Initialise(); 
        // }

    

  // ==================== Company Management =======================


        public IList<Company> GetCompanies()
        {
            return ctx.Companies.ToList();
        }

        public IList<Company> GetCompanies(string order = null, string direction = "asc")
        {
            var results = (order.ToLower(), direction.ToLower()) switch
            {
                ("cr_no", "asc") => ctx.Companies.OrderBy(c => c.CR_No),
                ("cr_no", "desc") => ctx.Companies.OrderByDescending(c => c.CR_No),

                ("taxID", "asc") => ctx.Companies.OrderBy(c => c.TaxID),
                ("TaxID", "desc") => ctx.Companies.OrderByDescending(c => c.TaxID),
                
                ("companyName", "asc") => ctx.Companies.OrderBy(c => c.CompanyName),
                ("companyName", "desc") => ctx.Companies.OrderByDescending(c => c.CompanyName),

                ("industry", "asc") => ctx.Companies.OrderBy(c => c.Industry),
                ("industry", "desc") => ctx.Companies.OrderByDescending(c => c.Industry),
                
                ("dateofestablishment", "asc") => ctx.Companies.OrderBy(c => c.DateOfEstablishment),
                ("dateofestablishment", "desc") => ctx.Companies.OrderByDescending(c => c.DateOfEstablishment),

                ("activity", "asc") => ctx.Companies.OrderBy(c => c.Activity),
                ("activity", "desc") => ctx.Companies.OrderByDescending(c => c.Activity),

                ("type", "asc") => ctx.Companies.OrderBy(c => c.Type),
                ("type", "desc") => ctx.Companies.OrderByDescending(c => c.Type),

                ("shareholderstructure", "asc") => ctx.Companies.OrderBy(c => c.ShareholderStructure),
                ("shareholderstructure", "desc") => ctx.Companies.OrderByDescending(c => c.ShareholderStructure),
                _                      => ctx.Companies.OrderBy(c => c.CR_No)
            };
            return results.ToList();
        }

        // Retrieve Company by Id 
        public Company GetCompany(string cr_no)
        {
            return ctx.Companies.FirstOrDefault(c => c.CR_No == cr_no);
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

            // create new company
            var company = new Company
            {
                CR_No=c.CR_No,
                TaxID=c.TaxID,
                CompanyName = c.CompanyName,
                Industry = c.Industry,
                DateOfEstablishment = c.DateOfEstablishment,
                Activity = c.Activity,
                Type = c.Type,
                ShareholderStructure = c.ShareholderStructure,
            };

            ctx.Companies.Add(company); // add company to the list
            ctx.SaveChanges();
            return company; // return newly added company
        }

        // Delete the company identified by Id returning true if 
        // deleted and false if not found
        public bool DeleteCompany(string CR_No)
        {
            var c = GetCompany(CR_No);
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
            var company = GetCompany(updated.CR_No);
            if (company == null)
            {
                return null;
            }

            // verify name is still unique
            var exists = GetCompanyByName(updated.CompanyName);
            if (exists != null && exists.CR_No != updated.CR_No)
            {
                return null;
            }

            // update the details of the company retrieved and save
            company.CompanyName = updated.CompanyName;
            company.Industry = updated.Industry;
            company.DateOfEstablishment = updated.DateOfEstablishment;
            company.Activity = updated.Activity;
            company.Type = updated.Type;
            company.ShareholderStructure = updated.ShareholderStructure;

            ctx.SaveChanges();
            return company;
        }

        public Company GetCompanyByName(string name)
        {
            return ctx.Companies.FirstOrDefault(c => c.CompanyName == name);
        }
    }
}