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
    //[Authorize]
    public class AssessmentsController : BaseController
    {
        private readonly IAssessmentsService _svc;

        public AssessmentsController(IAssessmentsService svc)
        {
            _svc = svc;
        }

        // GET /assessments
        public ActionResult Index(int page = 1, int size = 10, string orderBy = "CompanyName", string direction = "asc")
        {
            var table = _svc.GetAssessments(page, size, orderBy, direction);
            return View(table);
        }

        // GET /assessments/details/{requestNo}/{analystNo}
        public IActionResult Details(int requestNo, int analystNo)
        {
            var assessment = _svc.GetAssessment(requestNo, analystNo);

            if (assessment == null)
            {
                Alert("Assessment not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(assessment);
        }

        // GET: /assessments/create
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        public IActionResult Create()
        {
            // Display a blank form to create an assessment
            return View();
        }

        // POST /assessments/create
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create([Bind("RequestNo, AnalystNo, Sales, EBITDA, DSR, CCC, RiskRating, MarketPosition, RepaymentStatus, FinancialLeverage, WorkingCapital, OperatingAssets, CRNo, TotalAssets, NetEquity")] Assessments a)
        {
            // Check if an assessment with the same RequestNo and AnalystNo exists

            if (_svc.GetAssessment(a.RequestNo, a.AnalystNo) != null)
            {
                ModelState.AddModelError("", "An assessment with the same RequestNo and AnalystNo already exists.");
            }

            if (ModelState.IsValid)
            {
                var assessment = _svc.AddAssessment(a);

                if (assessment != null)
                {
                    return RedirectToAction(nameof(Details), new { requestNo = assessment.RequestNo, analystNo = assessment.AnalystNo });
                }

                Alert("Encountered an issue while creating the assessment.", AlertType.warning);
            }

            // Redisplay the form for editing as there are validation errors
            return View(a);
        }

        // GET /assessments/edit/{requestNo}/{analystNo]
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        public IActionResult Edit(int requestNo, int analystNo)
        {
            var assessment = _svc.GetAssessment(requestNo, analystNo);

            if (assessment == null)
            {
                Alert("Assessment not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(assessment);
        }

        // POST /assessments/edit/{requestNo}/{analystNo]
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(int requestNo, int analystNo, [Bind("Sales, EBITDA, DSR, CCC, RiskRating, MarketPosition, RepaymentStatus, FinancialLeverage, WorkingCapital, OperatingAssets, TotalAssets, NetEquity")] Assessments a)
        {

            // Check if an assessment with the same RequestNo and AnalystNo exists
            if (ModelState.IsValid)
            {
                var assessment = _svc.UpdateAssessment(a);

                if (assessment != null)
                {
                    return RedirectToAction(nameof(Details), new { requestNo = assessment.RequestNo, analystNo = assessment.AnalystNo });
                }

                Alert("Encountered an issue while updating the assessment.", AlertType.warning);
            }

            // Redisplay the form for editing as there are validation errors
            return View(a);
        }

        // GET /assessments/delete/{requestNo}/{analystNo]
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        public IActionResult Delete(int requestNo, int analystNo)
        {
            var assessment = _svc.GetAssessment(requestNo, analystNo);

            if (assessment == null)
            {
                Alert("Assessment not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(assessment);
        }

        // POST /assessments/delete/{requestNo}/{analystNo]
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int requestNo, int analystNo)
        {
            var deleted = _svc.DeleteAssessment(requestNo, analystNo);

            if (deleted)
            {
                Alert("Assessment deleted successfully.", AlertType.success);
            }
            else
            {
                Alert("Assessment could not be deleted.", AlertType.warning);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}