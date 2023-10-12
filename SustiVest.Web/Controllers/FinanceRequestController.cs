using Microsoft.AspNetCore.Mvc;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SustiVest.Web.Controllers
{

   [Authorize (Roles = "admin, analyst, borrower")]
    public class FinanceRequestController : BaseController
    {
        private readonly ICompanyService svc;

        private readonly Permissions _permissions;

        public FinanceRequestController(ICompanyService _svc, Permissions permissions)
        {
            svc = _svc;
            _permissions = permissions;

        }

        
        [Authorize(Roles = "admin, analyst, borrower")]
        public IActionResult Details(int requestNo)
        {

            var financeRequest = svc.GetFinanceRequest(requestNo);
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            
            if (!_permissions.IsUserAuthorizedToEditCompany(financeRequest.CRNo, userId, httpContext: HttpContext))
            {
                Alert("You are not authorized to view this request", AlertType.warning);
                return RedirectToAction("CompanyDetails", "Company", new { crNo = financeRequest.CRNo });
            }

            if (financeRequest == null)
            {
                Alert("Request Not Found", AlertType.warning);
                return RedirectToAction(nameof(CompanyController.CompanyIndex));
            }

            return View("Details", financeRequest);
        }

        // [HttpPost]
        // [Authorize(Roles = "admin,support")]
        // public IActionResult Resubmit(int requestNo, DateOnly dateOfRequest, bool assessment)
        // {
        //     svc.ResubmitRequest(requestNo, dateOfRequest, assessment);
        //     return RedirectToAction(nameof(Details), new { RequestNo = requestNo });
        // }

        [HttpPost]
        [Authorize(Roles = "admin, analyst")]
        public IActionResult Close([Bind("RequestNo, Status")] FinanceRequest f)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            if (!_permissions.IsUserAuthorizedToEditCompany(f.CRNo, userId, httpContext: HttpContext))
            {
                Alert("You are not authorized to delete this request", AlertType.warning);
                return RedirectToAction("CompanyDetails", "Company", new { crNo = f.CRNo });
            }

            var financeRequest = svc.CloseRequest(f.RequestNo, f.Status);
            if (financeRequest == null)
            {
                Alert("Request Not Found", AlertType.warning);
            }
            else
            {
                Alert($"Request No. {f.RequestNo} closed", AlertType.info);
            }


            // redirect to the index view
            return RedirectToAction(nameof(Details), new { RequestNo = f.RequestNo });
        }

        [Authorize(Roles = "admin, analyst, borrower")]
        [HttpGet]
        public IActionResult CreateRequest(String crNo)
        {

            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
                var company = svc.GetCompany(crNo);

                if (company == null)
                {
                    // Handle the case where the company is not found.
                    Alert("Company Not Found", AlertType.warning);
                    return RedirectToAction("CompanyIndex", "Company", new { crNo = crNo });
                }

                if (!_permissions.IsUserAuthorizedToEditCompany(crNo, userId, httpContext: HttpContext))
                {
                    Alert("You are not authorized to create a request for this company", AlertType.warning);
                    return RedirectToAction("CompanyDetails", "Company", new { crNo = crNo });
                }
                var model = new FinanceRequest { CRNo = crNo };

                return View(model);

            }
        }

        [HttpPost]
        [Authorize(Roles = "admin, analyst, borrower")]
        public IActionResult CreateRequest([Bind("Purpose, Amount, Tenor, FacilityType, CRNo, Status, DateOfRequest, Assessment")] FinanceRequest fr)
        {


            if (ModelState.IsValid)
            {
                var request = svc.CreateRequest(fr);

                if (request is null)
                {
                    Alert("Encountered issue creating request.", AlertType.warning);
                    return RedirectToAction("CompanyDetails", "Company", new { crNo = fr.CRNo });
                }
                Alert($"Request Submitted", AlertType.info);
                return RedirectToAction(nameof(Details), new { requestNo = request.RequestNo });
            }

            // redisplay the form for editing
            return View(fr);
        }

        [Authorize(Roles = "admin, analyst")]
        [HttpGet]
        public ActionResult Index(int page = 1, int size = 20, string order = "RequestNo", string direction = "asc")
        {
            var table = svc.GetFinanceRequests(page, size, order, direction);
            return View(table);
        }

        [Authorize (Roles = "admin, analyst, borrower")]
        public IActionResult RequestEdit(int requestNo)
        {
            var financeRequest = svc.GetFinanceRequest(requestNo);
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);

            if (financeRequest == null)
            {
                Alert("Request Not Found", AlertType.warning);
                return RedirectToAction(nameof(Details));
            }
            if (!_permissions.IsUserAuthorizedToEditCompany(financeRequest.CRNo, userId, httpContext: HttpContext))
            {
                Alert("You are not authorized to edit this request", AlertType.warning);
                return RedirectToAction(nameof(Details), new { requestNo = financeRequest.RequestNo });
            }

            return View("RequestEdit", financeRequest);
        }

        [Authorize (Roles = "admin, analyst, borrower")]
        [HttpPost]
        public IActionResult RequestEdit([Bind("RequestNo, Purpose, Amount, Tenor, FacilityType, Status, DateOfRequest, Assessment")] FinanceRequest fr)
        {
            if (ModelState.IsValid)
            {
                var request = svc.UpdateRequest(fr);
                return RedirectToAction(nameof(Details), new { requestNo = fr.RequestNo });

            }
            // redisplay the form for editing
            return View(fr);
        }
        [Authorize(Roles = "admin, analyst")]
        public IActionResult Delete(int requestNo)
        {
            var financeRequest = svc.GetFinanceRequest(requestNo);
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);

            if (financeRequest == null)
            {
                Alert("Request Not Found", AlertType.warning);
                return RedirectToAction(nameof(Details));
            }

            return View("Delete", financeRequest);
        }

        public IActionResult DeleteConfirm (int requestNo)
        {
            var deleted= svc.DeleteRequest(requestNo);
            if (deleted)
            {
                Alert($"Request No. {requestNo} deleted successfully", AlertType.info);
            }
            else
            {
                Alert($"Request No. {requestNo} could not be deleted", AlertType.warning);
            }
            return RedirectToAction(nameof(Index));
        }

    }

}


