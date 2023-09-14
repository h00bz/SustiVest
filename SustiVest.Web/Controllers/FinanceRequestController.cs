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

namespace SustiVest.Web.Controllers
{

    // [Authorize]
    public class FinanceRequestController : BaseController
    {
        private readonly ICompanyService svc;

        public FinanceRequestController(ICompanyService _svc)
        {
            svc = _svc;
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
        public IActionResult Details(int request_No)
        {
            var financeRequest = svc.GetFinanceRequest(request_No);
            if (financeRequest == null)
            {
                Alert("Request Not Found", AlertType.warning);
                return RedirectToAction(nameof(CompanyController.CompanyIndex));
            }

            return View("Details", financeRequest);
        }

        // [HttpPost]
        // [Authorize(Roles = "admin,support")]
        public IActionResult Resubmit(int request_No, DateOnly dateOfRequest, bool assessment)
        {
            svc.ResubmitRequest(request_No, dateOfRequest, assessment);
            return RedirectToAction(nameof(Details), new { Request_No = request_No });
        }

        // POST /ticket/close/{id}
        // [HttpPost]
        // [Authorize(Roles = "admin,support")]
        public IActionResult Close([Bind("Request_No, Status")] FinanceRequest f)
        {
            // close ticket via service
            var financeRequest = svc.CloseRequest(f.Request_No, f.Status);
            if (financeRequest == null)
            {
                Alert("Request Not Found", AlertType.warning);
            }
            else
            {
                Alert($"Request No. {f.Request_No} closed", AlertType.info);
            }

            // redirect to the index view
            return RedirectToAction(nameof(Details), new { Request_No = f.Request_No });
        }

        // GET /ticket/create
        // [Authorize(Roles = "admin,support")]
        [HttpGet]
        public IActionResult CreateRequest()
        {
            // var companies = svc.GetCompanies();
            // populate viewmodel select list property
            // var rvm = new RequestFinanceViewModel
            // {
            //     Companies= new SelectList(companies, nameof(Company.CR_No), nameof(Company.CompanyName))
            // };

            // render blank form
            return View();
        }

        // // POST /ticket/create
        [HttpPost]
        // [Authorize(Roles = "admin,support")]
        public IActionResult CreateRequest( [Bind("Purpose, Amount, Tenor, FacilityType, CR_No, Status, DateOfRequest, Assessment")] FinanceRequest fr)
        {
            if (ModelState.IsValid)
            {
               var request = svc.CreateRequest(fr.Purpose, fr.Amount, fr.Tenor, fr.FacilityType, fr.CR_No, fr.Status, fr.DateOfRequest, fr.Assessment);
           
            if (request is null) 
            {
                Alert("Encountered issue creating request.", AlertType.warning);
                return RedirectToAction(nameof(Details), new {Request_No=2});
            }
                Alert($"Request Submitted", AlertType.info);
                return RedirectToAction(nameof(Details) , new { Request_No = 2});
            }

            // redisplay the form for editing
            return View(fr);
        }

        public IActionResult Index()
        {
            var table = svc.GetFinanceRequests();
            return View(table);
        }

    public IActionResult RequestEdit(int request_No)
        {
            var financeRequest = svc.GetFinanceRequest(request_No);
            if (financeRequest == null)
            {
                Alert("Request Not Found", AlertType.warning);
                return RedirectToAction(nameof(Details));
            }

            return View("RequestEdit", financeRequest);
        }
        [HttpPost]
        public IActionResult RequestEdit(int request_No, [Bind("Purpose, Amount, Tenor, FacilityType, Status, DateOfRequest, Assessment")] FinanceRequest f)
        {
            if (ModelState.IsValid)
            {
                var request = svc.UpdateRequest(request_No, f.Purpose, f.Amount, f.Tenor, f.FacilityType, f.Status, f.DateOfRequest, f.Assessment);
                return RedirectToAction(nameof(Details), new { request_No = request_No });

            }
            // redisplay the form for editing
            return View(f);
        }

    }

}

