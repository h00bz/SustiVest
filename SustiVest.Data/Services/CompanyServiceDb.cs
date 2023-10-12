using SustiVest.Data.Entities;
using SustiVest.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
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

                ("ProfileStatus", "asc") => ctx.Companies.OrderBy(c => c.ProfileStatus),
                ("ProfileStatus", "desc") => ctx.Companies.OrderByDescending(c => c.ProfileStatus),
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
            // Check if a company with the same CompanyName or CRNo already exists
            var existsByName = GetCompanyByName(c.CompanyName);
            var existsByCRNo = GetCompany(c.CRNo);

            if (existsByName != null || existsByCRNo != null)
            {
                return null; // You might want to handle this case differently, e.g., return an error response.
            }

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
                RepId = c.RepId,
                ProfileStatus = c.ProfileStatus
            };

            ctx.Companies.Add(company);
            ctx.SaveChanges();
            return company;
        }
        public Company Approve(Company c)
        {
            var company = GetCompany(c.CRNo);
            if (company == null) 
            {
                return null;
                }

            company.ProfileStatus = "Approved";
            Console.WriteLine($"SSSScompany: {company} name:{company.CompanyName}");

            ctx.SaveChanges(); // write to database
            return company;
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
        public Company UpdateCompany(Company c)
        {
            // verify the commpany exists 
            var company = GetCompany(c.CRNo);
            if (company == null)
            {
                return null;
            }

            // verify name is still unique
            var exists = GetCompanyByName(company.CompanyName);
            if (exists != null && exists.CRNo != company.CRNo)
            {
                return null;
            }

            // update the details of the company retrieved and save
            company.CompanyName = c.CompanyName;
            company.Industry = c.Industry;
            company.DateOfEstablishment = c.DateOfEstablishment;
            company.Activity = c.Activity;
            company.Type = c.Type;
            company.ShareholderStructure = c.ShareholderStructure;
            company.CRNo = c.CRNo;
            company.TaxID = c.TaxID;
            company.RepId = c.RepId;
            company.ProfileStatus = c.ProfileStatus;


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

        public Paged<FinanceRequest> GetFinanceRequests(int page = 1, int size = 20, string orderBy = "RequestNo", string direction = "asc")
        {
            var results = (orderBy.ToLower(), direction.ToLower()) switch
            {
                ("Company.CompanyName", "asc") => ctx.FinanceRequests.OrderBy(fr => fr.Company.CompanyName),
                ("Company.CompanyName", "desc") => ctx.FinanceRequests.OrderByDescending(fr => fr.Company.CompanyName),

                ("Purpose", "asc") => ctx.FinanceRequests.OrderBy(fr => fr.Purpose),
                ("Purpose", "desc") => ctx.FinanceRequests.OrderByDescending(fr => fr.Purpose),

                ("Amount", "asc") => ctx.FinanceRequests.OrderBy(fr => fr.Amount),
                ("Amount", "desc") => ctx.FinanceRequests.OrderByDescending(fr => fr.Amount),

                ("Tenor", "asc") => ctx.FinanceRequests.OrderBy(fr => fr.Tenor),
                ("Tenor", "desc") => ctx.FinanceRequests.OrderByDescending(fr => fr.Tenor),

                ("FacilityType", "asc") => ctx.FinanceRequests.OrderBy(fr => fr.FacilityType),
                ("FacilityType", "desc") => ctx.FinanceRequests.OrderByDescending(fr => fr.FacilityType),

                ("CRNo", "asc") => ctx.FinanceRequests.OrderBy(fr => fr.CRNo),
                ("CRNo", "desc") => ctx.FinanceRequests.OrderByDescending(fr => fr.CRNo),

                ("Status", "asc") => ctx.FinanceRequests.OrderBy(fr => fr.Status),
                ("Status", "desc") => ctx.FinanceRequests.OrderByDescending(fr => fr.Status),

                ("DateOfRequest", "asc") => ctx.FinanceRequests.OrderBy(fr => fr.DateOfRequest),
                ("DateOfRequest", "desc") => ctx.FinanceRequests.OrderByDescending(fr => fr.DateOfRequest),

                ("Assessment", "asc") => ctx.FinanceRequests.OrderBy(fr => fr.Assessment),
                ("Assessment", "desc") => ctx.FinanceRequests.OrderByDescending(fr => fr.Assessment),
                _ => ctx.FinanceRequests.OrderBy(fr => fr.RequestNo)
            };
            return results.Include(fr => fr.Company).ToPaged(page, size, orderBy, direction);
        }


        public FinanceRequest CreateRequest(FinanceRequest fr)
        {
            var company = GetCompany(fr.CRNo);
            var exists = GetFinanceRequest(fr.RequestNo);
            if (exists != null) return null;

            var financeRequest = new FinanceRequest
            {
                Purpose = fr.Purpose,
                Amount = fr.Amount,
                Tenor = fr.Tenor,
                FacilityType = fr.FacilityType,
                CRNo = fr.CRNo,
                Status = fr.Status,
                DateOfRequest = fr.DateOfRequest,
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


        public FinanceRequest UpdateRequest(FinanceRequest fr)
        {
            var financeRequest = GetFinanceRequest(fr.RequestNo);
            if (financeRequest == null) return null;

            financeRequest.Purpose = fr.Purpose;
            financeRequest.Amount = fr.Amount;
            financeRequest.Tenor = fr.Tenor;
            financeRequest.FacilityType = fr.FacilityType;
            financeRequest.Status = fr.Status;
            financeRequest.DateOfRequest = fr.DateOfRequest;
            financeRequest.Assessment = fr.Assessment;

            ctx.SaveChanges(); // write to database
            return financeRequest;
        }

        public FinanceRequest ResubmitRequest(int requestNo, DateOnly dateOfRequest, bool assessment)
        {
            var financeRequest = GetFinanceRequest(requestNo);
            if (financeRequest == null) return null;

            financeRequest.Assessment = assessment;
            financeRequest.DateOfRequest = dateOfRequest;

            ctx.SaveChanges(); // write to database
            return financeRequest;
        }

        public FinanceRequest CloseRequest(int requestNo, string status)
        {
            var financeRequest = GetFinanceRequest(requestNo);
            if (financeRequest == null) return null;

            financeRequest.Assessment = true;
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

    }
}

