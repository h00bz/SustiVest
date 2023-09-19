using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using SustiVest.Data.Entities;
using SustiVest.Data.Services;
using Microsoft.AspNetCore.Authorization;
using SustiVest.Data.Security;
// using SustiVest.Web.Models.User;

namespace SustiVest.Web.Controllers
{
    // [Authorize]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService _svc;

        public CompanyController(ICompanyService svc)
        {
            _svc = svc;
        }

        // GET /company
        //editted from Index to CompanyIndex
        public ActionResult CompanyIndex(int page = 1, int size = 10, string order = "companyname", string direction = "asc")
        {
            // load Companies using service and pass to view
            var table = _svc.GetCompanies(page, size, order, direction);

            return View(table);
        }

        // GET /company/details/{CR_No}
        public IActionResult CompanyDetails(String crNo)
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
        // [Authorize(Roles="admin,support")]
        public IActionResult Create()
        {
            // display blank form to create company
            return View();
        }

        // POST /company/create
        // [Authorize(Roles="admin,support")]
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
                var company = _svc.AddCompany(c);
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
        // [Authorize(Roles="admin,support")]
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

            // pass company to view for editing
            return View(company);
        }

        // POST /company/edit/{id}
        [ValidateAntiForgeryToken]
        // [Authorize(Roles="admin,support")]
        [HttpPost]
        public IActionResult Edit(int id, [Bind("CompanyName, Industry, DateOfEstablishment, Activity, Type, ShareholderStructure")] Company c)
        {
            // check if email exists and is not owned by company being edited 
            var exists = _svc.GetCompanyByName(c.CompanyName);
            if (exists != null && c.CRNo != exists.CRNo)
            {
                ModelState.AddModelError(nameof(c.CompanyName), "The company name entered is not unique.");
            }

            // complete POST action to save company changes
            if (ModelState.IsValid)
            {
                var company = _svc.UpdateCompany(c);
                if (company is null)
                {
                    Alert("Encountered issue while updating company", AlertType.warning);
                }

                // redirect back to view the company details
                return RedirectToAction(nameof(CompanyDetails), new { CRNo = c.CRNo });
            }

            // redisplay the form for editing as validation errors
            return View(c);
        }

        // GET / company/delete/{id}
        // [Authorize(Roles="admin")]      
        public IActionResult Delete(string crNo)
        {
            // load the company using the service
            var company = _svc.GetCompany(crNo);
            // check the returned company is not null and if so return NotFound()
            if (company == null)
            {
                Alert("Company not found", AlertType.warning);
                return RedirectToAction(nameof(CompanyIndex));
            }

            // pass company to view for deletion confirmation
            return View(company);
        }

        // [Authorize(Roles="admin")]
        // POST /company/delete/{id}
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