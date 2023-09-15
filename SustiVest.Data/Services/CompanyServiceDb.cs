using System;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;
using SustiVest.Data.Security;
using SustiVest.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql.Internal.TypeHandlers.NetworkHandlers;

namespace SustiVest.Data.Services
{

    public class CompanyServiceDb : ICompanyService
    {
        private readonly DatabaseContext ctx;

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

        public Paged<Company> GetCompanies(int page = 1, int size = 20, string orderBy = "CompanyName", string direction = "asc")
        {
            var results = (orderBy.ToLower(), direction.ToLower()) switch
            {
                ("crNo", "asc") => ctx.Companies.OrderBy(c => c.CRNo),
                ("crNo", "desc") => ctx.Companies.OrderByDescending(c => c.CRNo),

                // ("taxID", "asc") => ctx.Companies.OrderBy(c => c.TaxID),
                // ("taxID", "desc") => ctx.Companies.OrderByDescending(c => c.TaxID),

                ("companyname", "asc") => ctx.Companies.OrderBy(c => c.CompanyName),
                ("companyname", "desc") => ctx.Companies.OrderByDescending(c => c.CompanyName),

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
                _ => ctx.Companies.OrderBy(c => c.CRNo)
            };
            return results.ToPaged(page, size, orderBy, direction);
        }

        // Retrieve Company by Id 
        public Company GetCompany(string crNo)
        {
            return ctx.Companies
            .Include(c => c.FinanceRequests)
            .FirstOrDefault(c => c.CRNo == crNo);
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
                CRNo = c.CRNo,
                TaxID = c.TaxID,
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
        public bool DeleteCompany(string crNo)
        {
            var c = GetCompany(crNo);
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
            var company = GetCompany(updated.CRNo);
            if (company == null)
            {
                return null;
            }

            // verify name is still unique
            var exists = GetCompanyByName(updated.CompanyName);
            if (exists != null && exists.CRNo != updated.CRNo)
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

        // ==================== Finance Request Management =======================

        public IList<FinanceRequest> GetFinanceRequests()
        {
            return ctx.FinanceRequests
            .Include(f => f.Company)
            .ToList();
        }

        public FinanceRequest CreateRequest(string purpose, int amount, int tenor, string facilityType, string crNo, string status, DateOnly dateOfRequest, bool assessment)
        {
            var company = GetCompany(crNo);
            if (company == null) return null;

            var financeRequest = new FinanceRequest
            {
                Purpose = purpose,
                Amount = amount,
                Tenor = tenor,
                FacilityType = facilityType,
                CRNo = crNo,
                Status = status,
                DateOfRequest = dateOfRequest,
                Assessment = false,
            };

            ctx.FinanceRequests.Add(financeRequest);
            ctx.SaveChanges(); // write to database
            return financeRequest;
        }

        public FinanceRequest GetFinanceRequest(int requestNo)
        {
            return ctx.FinanceRequests
            .Include(f => f.Company)
            .FirstOrDefault(f => f.RequestNo == requestNo);
        }


        public FinanceRequest UpdateRequest(int requestNo, string purpose, int amount, int tenor, string facilityType, string status, DateOnly dateOfRequest, bool assessment )
        {
            var financeRequest = GetFinanceRequest(requestNo);
            if (financeRequest == null) return null;

            financeRequest.Purpose = purpose;
            financeRequest.Amount = amount;
            financeRequest.Tenor = tenor;
            financeRequest.FacilityType = facilityType;
            financeRequest.Status = status;
            financeRequest.DateOfRequest = dateOfRequest; 
            financeRequest.Assessment=assessment;

            ctx.SaveChanges(); // write to database
            return financeRequest;
        }
        
        public FinanceRequest ResubmitRequest(int requestNo, DateOnly dateOfRequest, bool assessment)
        {
            var financeRequest = GetFinanceRequest(requestNo);
            if (financeRequest == null) return null;

            financeRequest.Assessment= assessment;
            financeRequest.DateOfRequest = dateOfRequest; 

            ctx.SaveChanges(); // write to database
            return financeRequest;
        }
        
        public FinanceRequest CloseRequest(int requestNo, string status) 
        {
            var financeRequest = GetFinanceRequest(requestNo);
            if (financeRequest == null) return null;

            financeRequest.Assessment= true;
            financeRequest.Status = status;

            ctx.SaveChanges(); // write to database
            return financeRequest;
        }
       
        public bool DeleteRequest(int requestNo)
        {
            var financeRequest = GetFinanceRequest(requestNo);
            if (financeRequest == null) return false;

            ctx.FinanceRequests.Remove(financeRequest);
            ctx.SaveChanges();
            return true;
        }
       
        public IList<FinanceRequest> GetOpenRequests()
        {
            // return open tickets with associated students
            return ctx.FinanceRequests
                     .Include(t => t.Company)
                     .Where(t => t.Assessment == false)
                     .ToList();
        }
    }
}
        // perform a search of the tickets based on a query and
        // an active range 'ALL', 'OPEN', 'CLOSED'
    //     public IList<FinanceRequest> SearchTickets(TicketRange range, string query, string orderBy = "id", string direction = "asc")
    //     {
    //         // ensure query is not null    
    //         query = query == null ? "" : query.ToLower();

    //         // search ticket issue, active status and student name
    //         var search = db.Tickets
    //                         .Include(t => t.Student)
    //                         .OrderBy(t => t.Student.Name)
    //                         .Where(t => (t.Issue.ToLower().Contains(query) ||
    //                                      t.Student.Name.ToLower().Contains(query)
    //                                     ) &&
    //                                     (range == TicketRange.OPEN && t.Active ||
    //                                      range == TicketRange.CLOSED && !t.Active ||
    //                                      range == TicketRange.ALL
    //                                     )

    //                         );
    //         return Ordered(search, orderBy, direction).ToList();
    //     }

    //     private IQueryable<Ticket> Ordered(IQueryable<Ticket> query, string orderby, string direction)
    //     {
    //         query = (orderby, direction) switch
    //         {
    //             ("id", "asc") => query.OrderBy(t => t.Id),
    //             ("id", "desc") => query.OrderByDescending(t => t.Id),
    //             ("name", "asc") => query.OrderBy(t => t.Student.Name),
    //             ("name", "desc") => query.OrderByDescending(t => t.Student.Name),
    //             ("createdon", "asc") => query.OrderBy(t => t.CreatedOn),
    //             ("createdon", "desc") => query.OrderByDescending(t => t.CreatedOn),
    //             _ => query
    //         };
    //         return query;
    //     }


    // }
