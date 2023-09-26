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

    public class Permissions : BaseController
    {
        private readonly ICompanyService _companyService;

        public Permissions(ICompanyService companyService)
        {
            _companyService = companyService;
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
        [Authorize(Roles = "admin, borrower, analyst")]
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
    }
}
