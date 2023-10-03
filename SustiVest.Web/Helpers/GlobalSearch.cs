using Microsoft.AspNetCore.Mvc;
using SustiVest.Data.Repositories;
using SustiVest.Web.Controllers;
using SustiVest.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Services;

namespace SustiVest.Web
{
    public class GlobalSearch : BaseController
    {
        private readonly DatabaseContext ctx;

        private readonly ICompanyService _companyService;

        public GlobalSearch(DatabaseContext ctx, ICompanyService companyService)
        {
            _companyService = companyService;
            this.ctx = ctx;
        }

        public IActionResult Search(string query, string property, string entity)
        {
            Console.WriteLine("=================Search  being called===================");
            Console.WriteLine($"=================propertyName={property} and query={query}===================");

            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(property))
            {
                Console.WriteLine("=================String is null/white===================");

                return Json(new List<Company>());
            }

            // Normalize property name to lowercase
            var propertyName = property.ToLower();


            Console.WriteLine($"=================propertyName={propertyName} and query={query}===================");
            if (entity == "company")
            {
                var results = _companyService.GetCompanies()
            .Where(c =>

                (propertyName == "crno" && c.CRNo.Equals(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "taxid" && c.TaxID.Equals(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "companyname" && c.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "industry" && c.Industry.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "type" && c.Type.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();
                Console.WriteLine($"IN COMP IF======={propertyName} and query={query}==========result?={results}");

                return Json(results);
            }

            if (entity == "financerequest")
            {
                var results = _companyService.GetFinanceRequests()
                .Where(fr =>
            (propertyName == "requestno" && fr.RequestNo.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "companyname" && fr.Company.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "amount" && fr.Amount.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "tenor" && fr.Tenor.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "facilitytype" && fr.FacilityType.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "status" && fr.Status.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .ToList();
                Console.WriteLine($"IN FR IF ======={propertyName} and query={query}==========result?={results}");

                return Json(results);
            }


            Console.WriteLine("________did not enter if");
            return View("_GlobalSearch");
        }
    }
}
