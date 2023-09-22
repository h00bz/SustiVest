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
    public class OfferController : BaseController
    {
        private readonly IOfferService svc;

        public OfferController(IOfferService offerService)
        {
            svc = offerService;
        }

        public IActionResult Details(int offerId)
        {
            var offer = svc.GetOffer(offerId);
            if (offer == null)
            {
                Alert("Offer Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(offer);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // You can add logic to populate any necessary data for the create view here.
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(int offerId, [Bind("OfferId, RequestNo, CRNo, Amount, Tenor, Payback, Linens, Undertakings, Covenants, ROR, FacilityType, UtilizationMechanism, AnalystNo")] Offer o)
        {
            if (ModelState.IsValid)
            {
                var offer = svc.CreateOffer(offerId, o.RequestNo, o.CRNo, o.Amount, o.Tenor, o.Payback, o.Linens, o.Undertakings, o.Covenants, o.ROR, o.FacilityType, o.UtilizationMechanism, o.AnalystNo);

                if (offer == null)
                {
                    Alert("Encountered issue creating offer.", AlertType.warning);
                    return RedirectToAction(nameof(Index));
                }

                Alert("Offer Created Successfully", AlertType.info);
                return RedirectToAction(nameof(Details), new {offerId = offer.OfferId});
            }

            // Redisplay the form for editing with validation errors
            return View(o);
        }

        [HttpGet]
        public IActionResult Edit(int OfferId)
        {
            var offer = svc.GetOffer(OfferId);
            if (offer == null)
            {
                Alert("Offer Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(offer);
        }

        [HttpPost]
        public IActionResult Edit(int offerId,[Bind("RequestNo, CRNo, Amount, Tenor, Payback, Linens, Undertakings, Covenants, ROR, FacilityType, UtilizationMechanism, AnalystNo")] Offer o)
        {
            
            if (ModelState.IsValid)
            {
                var updated = svc.UpdateOffer(offerId, o.RequestNo, o.CRNo, o.Amount, o.Tenor, o.Payback, o.Linens, o.Undertakings, o.Covenants, o.ROR, o.FacilityType, o.UtilizationMechanism, o.AnalystNo);

                if (updated == null)
                {
                    Alert("Encountered issue updating offer.", AlertType.warning);
                    return RedirectToAction(nameof(Details), new { offerId = o.OfferId});
                }

                Alert("Offer Updated Successfully", AlertType.info);
                return RedirectToAction(nameof(Details), new { offerId = o.OfferId});
            }

            // Redisplay the form for editing with validation errors
            return View(o);
        }

        public IActionResult Delete(int offerId)
        {
            var offer = svc.GetOffer(offerId);

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
            var deleted = svc.DeleteOffer(offerId);

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
                var offers = svc.GetOffers(page, size, orderBy, direction);
                return View(offers);
            }
        }
}

    

