using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        [HttpGet]
        public IActionResult CompanyIndex(int page = 1, int size = 20, string order = "companyname", string direction = "asc")
        {
            // load Companies using service and pass to view

            var table = _svc.GetCompanies(page, size, order, direction);
            if (!User.IsInRole("admin") && !User.IsInRole("analyst"))
            {
                table.Data = table.Data.Where(c => c.ProfileStatus == "Approved").ToList();

                return View(table);
            }
            return View(table);
        }

        // GET /company/details/{CR_No}
        [HttpGet]
        public IActionResult CompanyDetails(string crNo)
        {
            var company = _svc.GetCompany(crNo);
            // check if company is null and alert/redirect 
            if (company is null)
            {
                Alert("company not found", AlertType.warning);
                return RedirectToAction(nameof(CompanyIndex));
            }

            if (!User.IsInRole("admin") && !User.IsInRole("analyst"))
            {
                if (company.ProfileStatus != "Approved")
                {
                    Alert("company not found", AlertType.warning);
                    return RedirectToAction(nameof(CompanyIndex));
                }
            }

            return View(company);
        }

        // GET: /company/create
        [Authorize(Roles = "admin, borrower, analyst")]
        [HttpGet]
        public IActionResult Create()
        {
            // display blank form to create company
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);

            // Create a new Company instance and set the RepId property
            var company = new Company
            {
                RepId = currentUserId
            };
            return View(company);
        }

        // POST /company/create
        // [Authorize(Roles="admin,support")]
        [Authorize(Roles = "admin, borrower, analyst")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create([Bind("CRNo, TaxID, CompanyName, Industry, DateOfEstablishment, Activity, Type, ShareholderStructure, RepId, ProfileStatus")] Company c)
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
                Console.WriteLine($"company:2222222 {c} NAME {c.CompanyName}");
                var company = _svc.AddCompany(c);
                if (company is null)
                {
                    Alert("Encountered issue creating company profile.", AlertType.warning);
                    return RedirectToAction(nameof(CompanyIndex));
                }
                Alert("Thank you for creating a SustiVest profile. Your Request submitted for approval.", AlertType.success);
                return RedirectToAction(nameof(CompanyIndex));
            }
            // redisplay the form for editing as there are validation errors
            return View(c);
        }


        [Authorize(Roles = "admin, analyst")]
        [HttpGet]
        public IActionResult ApproveCompany(string crNo)
        {
            var company = _svc.GetCompany(crNo);
            Console.WriteLine($"company: {company} name:{company.CompanyName}");
            if (company == null)
            {
                Alert("Company not found", AlertType.warning);
                return RedirectToAction(nameof(CompanyIndex));
            }
            return View("CompanyDetails", company);
        }


        [Authorize(Roles = "admin, analyst")]
        [HttpPost]
        public IActionResult ApproveCompany(Company c)
        {
            var company = _svc.GetCompany(c.CRNo);
            Console.WriteLine($"2company: {company} name:{company.CompanyName}");

            if (company == null)
            {
                Console.WriteLine($"company null {company} name:{company.CompanyName}");
                Alert("Company not found", AlertType.warning);
                return RedirectToAction(nameof(CompanyIndex));
            }
            if (ModelState.IsValid)
            {
                var approved = _svc.Approve(company);
                Console.WriteLine($"3company: {approved} name:{approved.CompanyName}");

                if (approved == null)
                {
                    Alert("Could not post company profile", AlertType.warning);
                    return RedirectToAction(nameof(CompanyIndex));
                }
                // Redirect back to view the company details
                Alert("Company profile posted successfully", AlertType.warning);
                return RedirectToAction(nameof(CompanyDetails), new { crNo = approved.CRNo });

            }
          Console.WriteLine($"4company: {c} name:{c.CompanyName}");

            return View("CompanyDetails", c);
        }

        // GET /company/edit/{id}
        [Authorize(Roles = "admin, borrower, analyst")]
        [HttpGet]
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
            return View("Edit", company);  
        }

        // POST /company/edit/{id}
        [Authorize(Roles = "admin, borrower, analyst")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(Company c)
        {
            // check if company name already exists and is not owned by company being edited 

            // Check if the ModelState is valid (i.e., no validation errors)
                Console.WriteLine($"company: {c} name:{c.CompanyName}");

            if (ModelState.IsValid)
            {
                // Complete POST action to save company changes
                var updated = _svc.UpdateCompany(c);
                Console.WriteLine($"company: {updated} name:{updated.CompanyName}");

                if (updated == null)
                {
                    Alert("Encountered issue while updating company", AlertType.warning);
                    return RedirectToAction(nameof(CompanyIndex));
                }
                else
                {
                    // Redirect back to view the company details
                    return RedirectToAction(nameof(CompanyDetails), new { crNo = updated.CRNo });
                }
            }
            Console.WriteLine($"Model Invalid!!");

            // Redisplay the form for editing as validation errors
            return View(c);
        }


        // GET / company/delete/{id}
        [Authorize(Roles = "admin, analyst")]
        [HttpGet]
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
    }
}






