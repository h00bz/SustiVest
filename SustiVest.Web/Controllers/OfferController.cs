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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SustiVest.Web.Controllers
{
    public class OfferController : BaseController
    {
        private readonly IOfferService _svc;

        private readonly Permissions _permissions;

        public OfferController(IOfferService offerService, Permissions permissions)
        {
            _svc = offerService;
            _permissions = permissions;
        }
        [Authorize(Roles = "admin, analyst, borrower, investor")]
        public IActionResult Details(int offerId)
        {
            var offer = _svc.GetOffer(offerId);
            if (offer == null)
            {
                Alert("Offer Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(offer);
        }

        [Authorize(Roles = "admin, analyst")]
        [HttpGet]
        public IActionResult Create()
        {
            // You can add logic to populate any necessary data for the create view here.
            return View();
        }

        [Authorize(Roles = "admin, analyst")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create ([Bind("OfferId, RequestNo, CRNo, Amount, Tenor, Payback, Linens, Undertakings, Covenants, ROR, FacilityType, UtilizationMechanism, AnalystNo")] Offer o)
        {
            if (ModelState.IsValid)
            {
                var offer = _svc.CreateOffer( o.RequestNo, o.CRNo, o.Amount, o.Tenor, o.Payback, o.Linens, o.Undertakings, o.Covenants, o.ROR, o.FacilityType, o.UtilizationMechanism, o.AnalystNo, o.AssessmentNo);

                if (offer == null)
                {
                    Alert("Encountered issue creating offer.", AlertType.warning);
                    return RedirectToAction(nameof(Index));
                }

                Alert("Offer Created Successfully", AlertType.info);
                return RedirectToAction(nameof(Details), new { offerId = offer.OfferId });
            }

            // Redisplay the form for editing with validation errors
            return View(o);
        }
        [Authorize(Roles = "admin, analyst")]
        [HttpGet]
        public IActionResult Edit(int offerId)
        {
            var offer = _svc.GetOffer(offerId);
            
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);

            if (offer == null)
            {
                Alert("Offer Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            if (!_permissions.IsUserAuthorizedToEditOffer(offer.OfferId, userId, httpContext: HttpContext))
            {
                Alert("You are not authorized to edit this offer", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(offer);
        }

        [HttpPost]
        public IActionResult Edit(int offerId, int requestNo, string crNo, int analystNo, int assessmentNo, [Bind("Amount, Tenor, Payback, Linens, Undertakings, Covenants, ROR, FacilityType, UtilizationMechanism")] Offer o)
        {

            if (ModelState.IsValid)
            {
                var updated = _svc.UpdateOffer(offerId, requestNo, crNo, o.Amount, o.Tenor, o.Payback, o.Linens, o.Undertakings, o.Covenants, o.ROR, o.FacilityType, o.UtilizationMechanism,  analystNo, assessmentNo);

                if (updated == null)
                {
                    Alert("Encountered issue updating offer.", AlertType.warning);
                    return RedirectToAction(nameof(Details), new { offerId = offerId });
                }

                Alert("Offer Updated Successfully", AlertType.info);
                return RedirectToAction(nameof(Details), new { offerId = offerId });
            }

            // Redisplay the form for editing with validation errors
            return View(o);
        }

        public IActionResult Delete(int offerId)
        {
            var offer = _svc.GetOffer(offerId);

            if (offer == null)
            {
                Alert("Offer not found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(offer);
        }

        [HttpPost]
        public IActionResult DeleteConfirm(int offerId)
        {
            var deleted = _svc.DeleteOffer(offerId);

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

        public IActionResult Index(int page = 1, int size = 20, string orderBy = "OfferId", string direction = "asc")
        {
            var offers = _svc.GetOffers(page, size, orderBy, direction);
            return View(offers);
        }
    }
}



