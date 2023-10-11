using SustiVest.Data.Entities;

namespace SustiVest.Data.Services
{
    public interface IDepositRequestService
    {
        DepositRequest Create(DepositRequest depositRequest);
        DepositRequest GetDepositRequest(int depositRequestNo);
        DepositRequest GetDepositByOfferId(int offerId);
        DepositRequest Update(DepositRequest d);
        DepositRequest ApproveDeposit(DepositRequest d);
        bool DeleteDeposit(int depositRequestNo);
        IList<DepositRequest> GetDepositsByInvestor(int investorId);
        IList<DepositRequest> GetDepositsByOffer(int offerId);
        IList<DepositRequest> GetDepositsByCompany(string crNo);
        IList<DepositRequest> GetDeposits();
        Paged<DepositRequest> GetDeposits(int page = 1, int size = 20, string orderBy = "DepositRequestNo", string direction = "asc");
    }
}   