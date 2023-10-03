using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;
using Microsoft.AspNetCore.Authorization;
using SustiVest.Data.Security;
using SustiVest.Web.Models.User;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace SustiVest.Web.Controllers
{
    // [Authorize]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService _svc;
        private readonly Permissions _permissions;

        public CompanyController(ICompanyService svc, Permissions permissions)
        {
            _svc = svc;
            _permissions = permissions;
        }

        // GET /company
        //editted from Index to CompanyIndex
        public IActionResult CompanyIndex(int page = 1, int size = 10, string order = "companyname", string direction = "asc")
        {
            // load Companies using service and pass to view
            var table = _svc.GetCompanies(page, size, order, direction);

            return View(table);
        }

        // GET /company/details/{CR_No}
        public IActionResult CompanyDetails(string crNo)
        {
            var company = _svc.GetCompany(crNo);

            // check if company is null and alert/redirect 
            if (company is null)
            {
                Alert("company not found", AlertType.warning);
                return RedirectToAction(nameof(CompanyIndex));
            }
            return View(company);
        }

        // GET: /company/create
        [Authorize(Roles = "admin, borrower, analyst")]
        public IActionResult Create()
        {
            // display blank form to create company
            return View();
        }

        // POST /company/create
        // [Authorize(Roles="admin,support")]
        [Authorize(Roles = "admin, borrower, analyst")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create([Bind("CRNo, TaxID, CompanyName, Industry, DateOfEstablishment, Activity, Type, ShareholderStructure")] Company c)
        {
            // validate company name is unique
            if (_svc.GetCompanyByName(c.CompanyName) != null)
            {
                ModelState.AddModelError(nameof(c.CompanyName), "Please write unique company name.");
            }

            // complete POST action to add company
            if (ModelState.IsValid)
            {
                // call service AddCompany method using data in s
                var company = _svc.AddCompany(c.CRNo, c.TaxID, c.CompanyName, c.Industry, c.DateOfEstablishment, c.Activity, c.Type, c.ShareholderStructure, c.RepId);
                if (company is null)
                {
                    Alert("Encountered issue creating company profile.", AlertType.warning);
                    return RedirectToAction(nameof(CompanyIndex));
                }
                return RedirectToAction(nameof(CompanyDetails), new { CRNo = company.CRNo });
            }

            // redisplay the form for editing as there are validation errors
            return View(c);
        }

        // GET /company/edit/{id}
        [Authorize(Roles = "admin, borrower, analyst")]
        public IActionResult Edit(string crNo)
        {
            // load the company using the service
            var company = _svc.GetCompany(crNo);

            // check if company is null and Alert/Redirect
            if (company is null)
            {
                Alert("Company not found", AlertType.warning);
                return RedirectToAction(nameof(CompanyIndex));
            }
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);

            if (!_permissions.IsUserAuthorizedToEditCompany(crNo, userId, httpContext: HttpContext))
            {
                Alert("You are not authorized to edit this company profile", AlertType.warning);
                return RedirectToAction(nameof(CompanyDetails), new { crNo = crNo });
            }

            // // Pass the company to the view for editing
            return View(company);
        }

        // POST /company/edit/{id}
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, borrower, analyst")]
        [HttpPost]
        public IActionResult Edit(string crNo, string taxID, [Bind("CompanyName, Industry, DateOfEstablishment, Activity, Type, ShareholderStructure")] Company c)
        {
            // check if company name already exists and is not owned by company being edited 
            var company = _svc.GetCompany(crNo);

            // Check if the ModelState is valid (i.e., no validation errors)
            if (ModelState.IsValid)
            {
                // Complete POST action to save company changes
                var updated = _svc.UpdateCompany(crNo, taxID, c.CompanyName, c.Industry, c.DateOfEstablishment, c.Activity, c.Type, c.ShareholderStructure);

                if (updated is null)
                {
                    Alert("Encountered issue while updating company", AlertType.warning);
                }
                else
                {
                    // Redirect back to view the company details
                    return RedirectToAction(nameof(CompanyDetails), new { crNo = updated.CRNo });
                }
            }

            // Redisplay the form for editing as validation errors
            return View(c);
        }


        // GET / company/delete/{id}
        [Authorize(Roles = "admin, analyst")]
        public IActionResult Delete(string crNo)
        {
            // load the company using the service
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            var company = _svc.GetCompany(crNo);
            // check the returned company is not null and if so return NotFound()
            if (company == null)
            {
                Alert("Company not found", AlertType.warning);
                return RedirectToAction(nameof(CompanyIndex));
            }

            if (!_permissions.IsUserAuthorizedToEditCompany(crNo, userId, httpContext: HttpContext))
            {
                return RedirectToAction(nameof(CompanyDetails), new { crNo = crNo });
            }

            // pass company to view for deletion confirmation
            return View(company);
        }

        // [Authorize(Roles="admin")]
        // POST /company/delete/{id}
        [Authorize(Roles = "admin, analyst")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(string crNo)
        {
            // delete company via service
            var deleted = _svc.DeleteCompany(crNo);
            if (deleted)
            {
                Alert("Company deleted successfully.", AlertType.success);
            }
            else
            {
                Alert("Company could not  be deleted", AlertType.warning);
            }

            // redirect to the index view
            return RedirectToAction(nameof(CompanyIndex));
        }

        // [HttpGet("/Company/Search")]
        // public IActionResult Search(string query, string property)
        // {
        //     Console.WriteLine("=================Company Search being called===================");
        //     var companies = _svc.GetCompanies().Where(c =>
        //     {
        //         switch (property.ToLower()) // Convert property to lowercase for case-insensitive comparison
        //         {
        //             case "crno":
        //                 return c.CRNo.Contains(query, StringComparison.OrdinalIgnoreCase);

        //             case "taxid":
        //                 return c.TaxID.Contains(query, StringComparison.OrdinalIgnoreCase);

        //             case "companyname":
        //                 return c.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase);

        //             case "industry":
        //                 return c.Industry.Contains(query, StringComparison.OrdinalIgnoreCase);
        //             case "type":
        //                 return c.Type.Contains(query, StringComparison.OrdinalIgnoreCase);
        //             // Add more cases for other properties you want to search
        //             default:
        //                 // If an invalid property is selected, search in all properties as a fallback
        //                 return  c.CRNo.Contains(query, StringComparison.OrdinalIgnoreCase) ||
        //                         c.TaxID.Contains(query, StringComparison.OrdinalIgnoreCase) ||
        //                         c.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
        //                         c.Industry.Contains(query, StringComparison.OrdinalIgnoreCase) ||
        //                         c.Type.Contains(query, StringComparison.OrdinalIgnoreCase);
        //         }
        //     }).ToList();

        //     return Json(companies);
        // }


        // [HttpGet("/company/search")]
        // public IActionResult Search(string query)
        // {
        //     var companies = _svc.SearchCompanies(query); // ticket service
        //                                                          // map tickets to list of custom DTO objects
        //     var data = companies.Select(c => new
        //     {
        //         CRNo = c.CRNo,
        //         TaxID = c.TaxID,
        //         CompanyName = c.CompanyName,
        //         Industry = c.Industry,
        //         DateofEstablishment = c.DateOfEstablishment,
        //         Activity = c.Activity,
        //         Type = c.Type,
        //         ShareholderStructure = c.ShareholderStructure,
        //         RepId = c.RepId
        //     });

        //     return Ok(companies); // return json containing custom tickets list
        // }

        // public IActionResult Search()
        // {
        //     return View();

        // }
    }
}



// public bool  IsUserAuthorizedToEditCompany(string crNo, int userId)
// {
//     var company= _svc.GetCompany(crNo);

//     if (userId != company.RepId && !User.IsInRole("admin"))
//     {
//         Alert($"Sorry, you are not authorized to edit this company's profile", AlertType.warning);
//         return false;
//     }

//     return true;
// }




