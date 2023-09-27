using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using SustiVest.Data.Services;
using SustiVest.Web.Models.User;
using SustiVest.Data.Entities;
using SustiVest.Data.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using SustiVest.Web.Controllers;

namespace SustiVest.Web
{
    // [Authorize]
    public class Permissions : BaseController
    {
        private readonly ICompanyService _companyService;
        private readonly IAssessmentsService _assessmentsService;
        private readonly IOfferService _offerService
        ;


        public Permissions(ICompanyService companyService, IAssessmentsService assessmentsService, IOfferService offerService)
        {
            _companyService = companyService;
            _assessmentsService = assessmentsService;
            _offerService = offerService;
        }

        // public bool IsUserAuthorizedToEditCompany(string crNo, int userId)
        // {
        //     var company = _companyService.GetCompany(crNo);

        //     if (userId != company.RepId && !User.IsInRole("admin"))
        //     {
        //         Alert($"Sorry, you are not authorized to edit this company's profile", AlertType.warning);
        //         return false;
        //     }

        //     return true;
        // }
        // public bool IsUserAuthorizedToEditCompany(string crNo, int userId)
        // {
        //     var company = _companyService.GetCompany(crNo);

        //     if (company == null)
        //     {
        //         // Handle the case where the company is not found.
        //         Alert("Company Not Found", AlertType.warning);
        //         return false;
        //     }

        //     if (userId != company.RepId && !User.IsInRole("admin"))
        //     {
        //         Alert($"Sorry, you are not authorized to edit this company's profile", AlertType.warning);
        //         return false;
        //     }

        //     return true;
        // }

        // public bool IsUserAuthorizedToEditCompany(string crNo, int userId, HttpContext httpContext)
        public bool IsUserAuthorizedToEditCompany(string crNo, int userId, HttpContext httpContext)
        {
            var company = _companyService.GetCompany(crNo);

            if (company == null)
            {
                Alert("Company Not Found", AlertType.warning);
                return false;
            }

            if (httpContext != null && httpContext.User.Identity.IsAuthenticated && userId != company.RepId && !httpContext.User.IsInRole("admin"))

            {
                Alert($"Sorry, you are not authorized to edit this company's profile", AlertType.warning);
                return false;
            }

            return true;
        }

        public bool IsUserAuthorizedToEditAssessment(int assessmentNo, int userId, HttpContext httpContext)
        {
            var assessment = _assessmentsService.GetAssessment(assessmentNo);

            if (assessment == null)
            {
                Alert("Assessment Not Found", AlertType.warning);
                return false;
            }

            if (httpContext != null && httpContext.User.Identity.IsAuthenticated && userId != assessment.AnalystNo && !httpContext.User.IsInRole("admin"))

            {
                Alert($"Sorry, you are not authorized to edit this assessment", AlertType.warning);
                return false;
            }

            return true;
        }
        public bool IsUserAuthorizedToEditOffer(int offerId, int userId, HttpContext httpContext)
        {
            var offer = _offerService.GetOffer(offerId);
            if (offer == null)
            {
                Alert("Offer Not Found", AlertType.warning);
                return false;
            }


            if (httpContext != null && httpContext.User.Identity.IsAuthenticated && userId != offer.AnalystNo && !httpContext.User.IsInRole("admin"))

            {
                Alert($"Sorry, you are not authorized to edit this offer", AlertType.warning);
                return false;
            }

            return true;
        }
    }
}
