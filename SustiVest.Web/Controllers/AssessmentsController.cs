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
    [Authorize]
    public class AssessmentsController : BaseController
    {
        private readonly IAssessmentsService _svc;
        private readonly ICompanyService _companyService;
        private readonly Permissions _permissions;

        public AssessmentsController(IAssessmentsService svc, ICompanyService companyService, Permissions permissions)
        {
            _svc = svc;
            _companyService = companyService;
            _permissions = permissions;
        }

        // GET /assessments
        [Authorize(Roles = "admin, analyst")]
        public ActionResult Index(int page = 1, int size = 10, string orderBy = "CompanyName", string direction = "asc")
        {
            var table = _svc.GetAssessments(page, size, orderBy, direction);
            return View(table);
        }

        // GET /assessments/details/{requestNo}/{analystNo}
        [Authorize(Roles = "admin, analyst, borrower")]
        public IActionResult Details(int? assessmentNo, int? requestNo)
        {

            Assessments assessment = null;
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Sid).Value);

            if (assessmentNo.HasValue)
            {
                // Try to get the assessment by assessment number.
                assessment = _svc.GetAssessment(assessmentNo.Value);
            }
            else if (requestNo.HasValue)
            {
                // Try to get the assessment by request number.
                assessment = _svc.GetAssessmentByRequestNo(requestNo.Value);
            }

            if (assessment == null)
            {
                Alert("Assessment not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            var company = _companyService.GetCompany(assessment.CRNo);

            // if (!_permissions.IsUserAuthorizedToEditCompany(company.CRNo, userId, httpContext: HttpContext) && !_permissions.IsUserAuthorizedToEditAssessment(assessment.AssessmentNo, userId, httpContext: HttpContext) && !User.IsInRole("admin"))
            // {
            //     Alert("You are not authorized to view this assessment", AlertType.warning);
            //     return RedirectToAction("Details", "FinanceRequest", new { requestNo = assessment.RequestNo});
            // }

            return View(assessment);
        }


        //  public IActionResult Details(int requestNo)
        // {
        //     var assessment = _svc.GetAssessmentByRequestNo(requestNo);

        //     if (assessment == null)
        //     {
        //         Alert("Assessment not found", AlertType.warning);
        //         return RedirectToAction(nameof(Index));
        //     }

        //     return View(assessment);
        // }

        // GET: /assessments/create
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        [Authorize(Roles = "admin, analyst")]
        public IActionResult Create()
        {
            // Display a blank form to create an assessment
            return View();
        }

        // POST /assessments/create
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        [Authorize(Roles = "admin, analyst")]
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
        [Authorize(Roles = "admin, analyst")]
        public IActionResult Edit(int assessmentNo)
        {
            var assessment = _svc.GetAssessment(assessmentNo);
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Sid).Value);

            if (assessment == null)
            {
                Alert("Assessment not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            if (!_permissions.IsUserAuthorizedToEditAssessment(assessment.AssessmentNo, userId, httpContext: HttpContext))
            {
                Alert("You are not authorized to edit this assessment", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            return View(assessment);
        }


        // POST /assessments/edit/{requestNo}/{analystNo]
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        [Authorize(Roles = "admin, analyst")]
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

                    return RedirectToAction(nameof(Details), new { assessmentNo = updated.AssessmentNo });

                }

                Alert("Encountered an issue while updating the assessment.", AlertType.warning);
            }

            // Redisplay the form for editing as there are validation errors
            return View(a);
        }

        // GET /assessments/delete/{requestNo}/{analystNo]
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        [Authorize(Roles = "admin, analyst")]
        public IActionResult Delete(int assessmentNo)
        {
            var assessment = _svc.GetAssessment(assessmentNo);
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Sid).Value);
            if (assessment == null)
            {
                Alert("Assessment not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            if (!_permissions.IsUserAuthorizedToEditAssessment(assessment.AssessmentNo, userId, httpContext: HttpContext))
            {
                Alert("You are not authorized to delete this assessment", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            return View(assessment);

        }

        // POST /assessments/delete/{requestNo}/{analystNo]
        // Commented out the [Authorize(Roles = "admin,support")] attribute
        [Authorize(Roles = "admin, analyst")]
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