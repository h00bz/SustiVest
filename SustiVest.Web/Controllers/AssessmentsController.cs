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
        public IActionResult Details(int assessmentNo)
        {
            var assessment = _svc.GetAssessment(assessmentNo);

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
        public IActionResult Create(int AssessmentNo, [Bind("RequestNo, AnalystNo, Sales, EBITDA, DSR, CCC, RiskRating, MarketPosition, RepaymentStatus, FinancialLeverage, WorkingCapital, OperatingAssets, CRNo, TotalAssets, NetEquity")] Assessments a)
        {
            // Check if an assessment with the same RequestNo and AnalystNo exists

            // if (_svc.GetAssessment(a.RequestNo, a.AnalystNo) != null)
            // {
            //     ModelState.AddModelError("", "An assessment with the same RequestNo and AnalystNo already exists.");
            // }

            if (ModelState.IsValid)
            {
                var assessment = _svc.AddAssessment(a);

                if (assessment != null)
                {
                    return RedirectToAction(nameof(Details), new { assessmentNo = assessment.AssessmentNo });
                }

                Alert("Encountered an issue while creating the assessment.", AlertType.warning);
            }

            // Redisplay the form for editing as there are validation errors
            return View(a);
        }

        // GET /assessments/edit/{requestNo}/{analystNo]
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        public IActionResult Edit(int assessmentNo)
        {
            var assessment = _svc.GetAssessment(assessmentNo);

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
        public IActionResult Edit(int assessmentNo, [Bind("RequestNo, AnalystNo, Sales, EBITDA, DSR, CCC, RiskRating, MarketPosition, RepaymentStatus, FinancialLeverage, WorkingCapital, OperatingAssets, CRNo, TotalAssets, NetEquity")] Assessments a)
        {
            var assessment = _svc.GetAssessment(assessmentNo);

            // Check if an assessment with the same RequestNo and AnalystNo exists
            if (ModelState.IsValid)
            {
                var updated = _svc.UpdateAssessment(assessmentNo, a.RequestNo, a.AnalystNo, a.Sales, a.EBITDA, a.DSR, a.CCC, a.RiskRating, a.MarketPosition, a.RepaymentStatus, a.FinancialLeverage, a.WorkingCapital, a.OperatingAssets, a.CRNo, a.TotalAssets, a.NetEquity);

                if (updated != null)
                {
                    Alert("Assessment Updated", AlertType.success);

                    return RedirectToAction(nameof(Details), new { requestNo = updated.RequestNo, updated.AnalystNo });

                }

                Alert("Encountered an issue while updating the assessment.", AlertType.warning);
            }

            // Redisplay the form for editing as there are validation errors
            return View(a);
        }

        // GET /assessments/delete/{requestNo}/{analystNo]
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        public IActionResult Delete(int assessmentNo)
        {
            var assessment = _svc.GetAssessment(assessmentNo);

            if (assessment == null)
            {
                Alert("Assessment not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(assessment);
        }

        // POST /assessments/delete/{requestNo}/{analystNo]
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult DeleteConfirm(int assessmentNo)
        {
            var deleted = _svc.DeleteAssessment(assessmentNo);

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