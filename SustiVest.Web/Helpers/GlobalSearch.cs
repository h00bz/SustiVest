using Microsoft.AspNetCore.Mvc;
using SustiVest.Data.Repositories;
using SustiVest.Web.Controllers;
using SustiVest.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Services;
using Microsoft.AspNetCore.Authorization;

namespace SustiVest.Web
{
    public class GlobalSearch : BaseController
    {
        private readonly DatabaseContext ctx;

        private readonly ICompanyService _companyService;

        private readonly IAssessmentsService _assessmentsService;

        private readonly IOfferService _offerService;

        private readonly IDepositRequestService _depositRequestService;

        public GlobalSearch(DatabaseContext ctx, ICompanyService companyService, IAssessmentsService assessmentsService, IOfferService offerService, IDepositRequestService depositRequestService)
        {
            _companyService = companyService;
            _assessmentsService = assessmentsService;
            _offerService = offerService;
            _depositRequestService = depositRequestService;
            this.ctx = ctx;
        }
        [Authorize(Roles = "admin, analyst, borrower")]
        public IActionResult Search(string query, string property, string entity)
        {
            Console.WriteLine("=================Search  being called===================");
            Console.WriteLine($"=======entity:{entity}==========propertyName={property} and query={query}===================");

            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(property))
            {
                Console.WriteLine("=================String is null/white===================");

                return Json(new List<Company>());
            }

            // Normalize property name to lowercase
            var propertyName = property.ToLower();


            Console.WriteLine($"=================propertyName={propertyName} and query={query}====entity {entity}===============");
            if (entity == "company")
            {
                var results = _companyService.GetCompanies()
            .Where(c =>

                (propertyName == "crno" && c.CRNo.Equals(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "taxid" && c.TaxID.Equals(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "companyname" && c.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "industry" && c.Industry.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "type" && c.Type.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();
                Console.WriteLine($"IN COMP IF======={propertyName} and query={query}==========result?={results}");

                return Json(results);
            }

            else if (entity == "financerequest" && (!User.IsInRole("borrower") || !User.IsInRole("investor")))
            {
                var results = _companyService.GetFinanceRequests()
                .Where(fr =>
            (propertyName == "requestno" && fr.RequestNo.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "financerequest.company.companyname" && fr.Company != null && fr.Company.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "amount" && fr.Amount.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "tenor" && fr.Tenor.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "facilitytype" && fr.FacilityType.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "status" && fr.Status.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .ToList();
                return Json(results);
            }
            else if (entity == "assessments")
            {
                var results = _assessmentsService.GetAssessments()
                .Where(a =>
                (propertyName == "assessmentno" && a.AssessmentNo.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "assessments.company.companyname" && a.Company != null && a.Company.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "riskrating" && a.RiskRating.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "repaymentstatus" && a.RepaymentStatus.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (propertyName == "analystno" && a.AnalystNo.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();
                return Json(results);
            }

            if (entity == "offer")
            {
                var results = _offerService.GetOffers()
                .Where(o =>
            (propertyName == "offerid" && o.OfferId.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "offer.company.companyname" && o.Company != null && o.Company.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "amount" && o.Amount.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "tenor" && o.Tenor.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "ror" && o.ROR.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "facilitytype" && o.FacilityType.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .ToList();
                return Json(results);
            }

            if (entity == "depositrequest" && !User.IsInRole("borrower"))
            {
                var results = _depositRequestService.GetDeposits()
                .Where(d =>
            (propertyName == "depositrequestno" && d.DepositRequestNo.ToString().Equals(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "investorid" && d.InvestorId.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "offerid" && d.OfferId.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "status" && d.Status.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (propertyName == "depositrequest.company.companyname" && d.Company != null && d.Company.CompanyName.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .ToList();
                return Json(results);
            }

            Console.WriteLine("________did not enter if");
            return View("_GlobalSearch");
        }
    }
}
