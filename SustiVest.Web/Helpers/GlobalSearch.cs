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
            Console.WriteLine("=================Search for Company being called===================");

            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(property))
            {
                return View("_GlobalSearch", new List<Company>());
            }

            // Normalize property name to lowercase
            var propertyName = property.ToLower();
            

            Console.WriteLine($"=================propertyName={propertyName} and query={query}===================");
            if (entity == "company")
            {
                var results = _companyService.GetCompanies()
            .Where(c =>

                (propertyName == "crno" && c.CRNo.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "taxid" && c.TaxID.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "companyname" && c.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "industry" && c.Industry.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "type" && c.Type.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();
                Console.WriteLine($"=================result?={results}");

                return Json(results);
            }
            Console.WriteLine("________did not enter if");
            return View("_GlobalSearch");
        }
            // }

            // if (entity == "financerequest")
            // {
            //     var results = _companyService.GetFinanceRequests()
            // // Convert to a list to enable client-side evaluation
            // .Where(fr =>

            //     (propertyName == "requestno" && fr.RequestNo.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
            //     (propertyName == "companyname" && fr.Company.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            //     (propertyName == "amount" && fr.Amount.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
            //     (propertyName == "tenor" && fr.Tenor.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
            //     (propertyName == "facilitytype" && fr.FacilityType.Equals(query, StringComparison.OrdinalIgnoreCase)))
            //     .ToList();
            //     Console.WriteLine($"=================result?={results}");

            //     return Json(results);
            // }
        

        // [HttpGet("/GlobalSearch/Search")]
        // public IActionResult Search(string entity, string query, string property)
        // {
        //     Console.WriteLine("=================Search being called===================");
        //     if (string.IsNullOrWhiteSpace(entity) || string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(property))
        //     {
        //         return View(new List<object>()); // Display an empty result when the parameters are empty
        //     }

        //     var entities = GetEntities(entity);
        //     Console.WriteLine($"=================entities: {entity}===================");


        //     if (!entities.Any())
        //     {
        //         // Invalid entity name, handle it as needed (e.g., show an error message)
        //         return View(new List<object>());
        //     }

        //     var entityType = entities.First().GetType();
        //     var properties = entityType.GetProperties();
        //     Console.WriteLine($"=================entityType: {entityType}===================");
        //     Console.WriteLine($"=================properties: {properties}===================");

        //     // Use reflection to dynamically query the entities
        //     var results = entities.Where(e =>
        //     {
        //         var selectedProperty = properties.FirstOrDefault(p => string.Equals(p.Name, property, StringComparison.OrdinalIgnoreCase));
        //         Console.WriteLine($"=================selected: {selectedProperty}===================");

        //         if (selectedProperty == null)
        //         {

        //             // Invalid property name, handle it as needed (e.g., show an error message)
        //             return false;
        //         }

        //         var propertyValue = selectedProperty.GetValue(e)?.ToString();
        //      Console.WriteLine($"=================propertyValue: {propertyValue}===================");

        //         return propertyValue != null && propertyValue.Contains(query, StringComparison.OrdinalIgnoreCase);
        //     }).ToList();

        //     return PartialView("_GlobalSearch", results);
        // }
        // [HttpGet("/GlobalSearch/Search")]
        // public IActionResult Search(string entity, string query, string property)
        // {
        //     Console.WriteLine("=================Search being called===================");
        //     if (string.IsNullOrWhiteSpace(entity) || string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(property))
        //     {
        //         return View(new List<object>()); // Display an empty result when the parameters are empty
        //     }

        //     var entities = GetEntities(entity);
        //     Console.WriteLine($"=================entities: {entity}===================");


        //     if (!entities.Any())
        //     {
        //         // Invalid entity name, handle it as needed (e.g., show an error message)
        //         return View(new List<object>());
        //     }

        //     var entityType = entities.First().GetType();
        //     var properties = entityType.GetProperties();
        //     Console.WriteLine($"=================entityType: {entityType}===================");
        //     Console.WriteLine($"=================properties: {properties}===================");

        //     // Use reflection to dynamically query the entities
        //     var results = entities.Where(e =>
        //     {
        //         var selectedProperty = properties.FirstOrDefault(p => string.Equals(p.Name, property, StringComparison.OrdinalIgnoreCase));
        //         Console.WriteLine($"=================selected: {selectedProperty}===================");

        //         if (selectedProperty == null)
        //         {

        //             // Invalid property name, handle it as needed (e.g., show an error message)
        //             return false;
        //         }

        //         var propertyValue = selectedProperty.GetValue(e)?.ToString();
        //         Console.WriteLine($"=================propertyValue: {propertyValue}===================");

        //         return propertyValue != null && propertyValue.Contains(query, StringComparison.OrdinalIgnoreCase);
        //     }).ToList();

        //     return PartialView("_GlobalSearch", results);
        // }



        private IEnumerable<object> GetEntities(string entity)
        {
            switch (entity.ToLower())
            {
                case "company":
                    return ctx.Companies.ToList(); // Replace with your actual entity name
                case "financerequest":
                    return ctx.FinanceRequests.ToList(); // Add more cases for other entities
                default:
                    return new List<object>();
            }
        }
    }
}

