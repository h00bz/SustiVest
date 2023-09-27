using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using SustiVest.Web.Views;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;
using Microsoft.AspNetCore.Authorization;
using SustiVest.Data.Security;
using SustiVest.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using Microsoft.AspNetCore;
using System.Security.Claims;
using SustiVest.Web;
using System;
namespace SustiVest.Web.Controllers
{

    // [Authorize]
    public class FinanceRequestController : BaseController
    {
        private readonly ICompanyService svc;

        private readonly Permissions _permissions;

        public FinanceRequestController(ICompanyService _svc, Permissions permissions)
        {
            svc = _svc;
            _permissions = permissions;

        }

        // GET/POST /Request/index
        // public IActionResult Index(FinanceRequestSearchViewModel search)
        // {
        //     // set the viewmodel Tickets property by calling service method 
        //     // using the range and query values from the viewmodel 
        //     search.Tickets = svc.SearchTickets(search.Range, search.Query, search.OrderBy, search.Direction);

        //     // build custom alert message                
        //     var alert = $"{search.Tickets.Count} result(s) found searching '{search.Range}' Tickets";
        //     if (search.Query != null)
        //     {
        //         alert += $" for '{search.Query}'";
        //     }
        //     // display alert
        //     Alert(alert, AlertType.info);

        //     return View(search);
        // }

        // display page containg JS query 
        // public IActionResult Search()
        // {
        //     return View();
        // }

        // //[AllowAnonymous]

        // // GET /tickets/query
        // [HttpGet("api/ticket/search")]
        // public IActionResult Search(string query, TicketRange range = TicketRange.ALL)
        // {
        //     // search tickets   
        //     var tickets = svc.SearchTickets(range, query);
        //     // map tickets to custom DTO object
        //     var data = tickets.Select(t => new
        //     {
        //         Id = t.Id,
        //         Issue = t.Issue,
        //         CreatedOn = t.CreatedOn,
        //         Active = t.Active,
        //         Student = t.Student?.Name
        //     });
        //     // return json containing custom tickets list
        //     return Ok(tickets);
        // }

        // GET/ticket/{id}
        [Authorize(Roles = "admin, analyst, borrower")]
        public IActionResult Details(int requestNo)
        {
            var financeRequest = svc.GetFinanceRequest(requestNo);
            if (financeRequest == null)
            {
                Alert("Request Not Found", AlertType.warning);
                return RedirectToAction(nameof(CompanyController.CompanyIndex));
            }

            return View("Details", financeRequest);
        }

        // [HttpPost]
        // [Authorize(Roles = "admin,support")]
        public IActionResult Resubmit(int requestNo, DateOnly dateOfRequest, bool assessment)
        {
            svc.ResubmitRequest(requestNo, dateOfRequest, assessment);
            return RedirectToAction(nameof(Details), new { RequestNo = requestNo });
        }

        // POST /ticket/close/{id}
        [HttpPost]
        [Authorize(Roles = "admin, analyst")]
        public IActionResult Close([Bind("RequestNo, Status")] FinanceRequest f)
        {
            // close ticket via service
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

        // GET /ticket/create
        [Authorize(Roles = "admin, analyst, borrower")]
        [HttpGet]
        public IActionResult CreateRequest(String crNo)
        {

            {
                // if (string.IsNullOrEmpty(crNo))
                // {
                //     // Handle the case where CRNo is not provided.
                //     // You can show an error message or redirect as needed.
                //     return RedirectToAction(nameof(CompanyController.CompanyIndex)); // Example redirect.
                // }

                // You can use the crNo parameter to pre-populate the CRNo field in your form.
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

        // // POST /ticket/create
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

        [Authorize(Roles = "admin, analyst, borrower")]
        public ActionResult Index(int page = 1, int size = 20, string order = "Purpose", string direction = "asc")
        {
            var table = svc.GetFinanceRequests(page, size, order, direction);   
            return View(table);
        }

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
        [HttpPost]
        public IActionResult RequestEdit(int requestNo, [Bind("Purpose, Amount, Tenor, FacilityType, Status, DateOfRequest, Assessment")] FinanceRequest f)
        {
            if (ModelState.IsValid)
            {
                var request = svc.UpdateRequest(requestNo, f.Purpose, f.Amount, f.Tenor, f.FacilityType, f.Status, f.DateOfRequest, f.Assessment);
                return RedirectToAction(nameof(Details), new { requestNo = requestNo });

            }
            // redisplay the form for editing
            return View(f);
        }

    }

}


