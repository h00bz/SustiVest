using SustiVest.Data.Entities;

namespace SustiVest.Data.Services
{
    public interface IOfferService
    {
        IList<Offer> GetOffers();
        Offer GetOffer(int offerId);
        Offer CreateOffer( Offer o);
        Offer UpdateOffer(int offerId, int requestNo, string crNo, int amount, int tenor, string payback, string linens, string undertakings, string covenants, double ror, string facilityType, string utilizationMechanism, int analystNo, int assessmentNo);
        bool DeleteOffer(int offerId);
        Paged<Offer> GetOffers(int page = 1, int size = 20, string orderBy = "OfferId", string direction = "asc");
    }
}
