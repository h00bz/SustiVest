using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Entities;
using SustiVest.Data.Repositories;


namespace SustiVest.Data.Services
{
    public class OfferServiceDb : IOfferService
    {
        private readonly DatabaseContext ctx;

        // private readonly ICompanyService _companySVC;

        public OfferServiceDb(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }

        public IList<Offer> GetOffers()
        {
            return ctx.Offers
                .Include(o => o.Company)
                .ToList();
        }
        public Offer GetOffer(int offerId)
        {

            return ctx.Offers
                .Include(o => o.Company)
                .FirstOrDefault(o => o.OfferId == offerId);

        }


        public Offer CreateOffer(Offer o)
        {
            var offer = new Offer
            {
                RequestNo = o.RequestNo,
                CRNo = o.CRNo,
                Amount = o.Amount,
                FundedAmount = o.FundedAmount,
                Tenor = o.Tenor,
                Payback = o.Payback,
                Linens = o.Linens,
                Undertakings = o.Undertakings,
                Covenants = o.Covenants,
                ROR = o.ROR,
                FacilityType = o.FacilityType,
                UtilizationMechanism = o.UtilizationMechanism,
                AnalystNo = o.AnalystNo,
                AssessmentNo = o.AssessmentNo
            };

            ctx.Offers.Add(offer);
            ctx.SaveChanges();
            return offer;
        }
        public Offer PostOffer(int offerId)
        {
            var offer = GetOffer(offerId);
            var assessment = ctx.Assessments.FirstOrDefault(a => a.RequestNo == offer.RequestNo);
            if (offer == null)
            {
                return null;
            }
            offer.Posted = true;
            assessment.Posted = true;
            ctx.SaveChanges();
            return offer;
        }


        public Offer UpdateOffer(int offerId, int requestNo, string crNo, int amount, int fundedAmount, int tenor, string payback, string linens, string undertakings, string covenants, double ror, string facilityType, string utilizationMechanism, int analystNo, int assessmentNo)
        {
            var offer = GetOffer(offerId);
            if (offer == null)
            {

                return null;
            }

            // var company = _companySVC.GetCompany(crNo);
            // if (company == null)
            // {
            //     return null;
            // }
            offer.RequestNo = requestNo;
            offer.CRNo = crNo;
            offer.Amount = amount;
            offer.FundedAmount = fundedAmount;
            offer.Tenor = tenor;
            offer.Payback = payback;
            offer.Linens = linens;
            offer.Undertakings = undertakings;
            offer.Covenants = covenants;
            offer.ROR = ror;
            offer.FacilityType = facilityType;
            offer.UtilizationMechanism = utilizationMechanism;
            offer.AnalystNo = analystNo;
            offer.AssessmentNo = assessmentNo;

            ctx.SaveChanges();
            return offer;
        }

        public bool DeleteOffer(int offerId)
        {
            var offer = GetOffer(offerId);
            if (offer == null)
                return false;

            ctx.Offers.Remove(offer);
            ctx.SaveChanges();
            return true;
        }
        public Paged<Offer> GetOffers(int page = 1, int size = 20, string orderBy = "OfferId", string direction = "asc")
        {
            var results = (orderBy.ToLower(), direction.ToLower()) switch
            {
                ("offerId", "asc") => ctx.Offers.OrderBy(o => o.OfferId),
                ("offerId", "desc") => ctx.Offers.OrderByDescending(o => o.OfferId),
                ("crNo", "asc") => ctx.Offers.OrderBy(o => o.CRNo),
                ("crNo", "desc") => ctx.Offers.OrderByDescending(o => o.CRNo),
                ("amount", "asc") => ctx.Offers.OrderBy(o => o.Amount),
                ("amount", "desc") => ctx.Offers.OrderByDescending(o => o.Amount),
                ("tenor", "asc") => ctx.Offers.OrderBy(o => o.Tenor),
                ("tenor", "desc") => ctx.Offers.OrderByDescending(o => o.Tenor),
                ("ror", "asc") => ctx.Offers.OrderBy(o => o.ROR),
                ("ror", "desc") => ctx.Offers.OrderByDescending(o => o.ROR),
                ("facilitytype", "asc") => ctx.Offers.OrderBy(o => o.FacilityType),
                ("facilitytype", "desc") => ctx.Offers.OrderByDescending(o => o.FacilityType),
                _ => ctx.Offers.OrderBy(o => o.OfferId)
            };
            return results.Include(o => o.Company).ToPaged(page, size, orderBy, direction);
        }

    }
}
