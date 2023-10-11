using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Entities;
using SustiVest.Data.Repositories;


namespace SustiVest.Data.Services
{
    public class DepositRequestServiceDb : IDepositRequestService
    {
        private readonly DatabaseContext ctx;


        public DepositRequestServiceDb(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }

        public IList<DepositRequest> GetDeposits()
        {
            return ctx.DepositRequests
                .Include(d => d.User)
                .Include(d => d.Offer)
                .Include(d => d.Company)
                .ToList();
        }

        public DepositRequest GetDepositRequest(int depositRequestNo)
        {
            return ctx.DepositRequests
                .Include(d => d.User)
                .Include(d => d.Offer)
                .Include(d => d.Company)
                .FirstOrDefault(d => d.DepositRequestNo == depositRequestNo);
        }

        public DepositRequest GetDepositByOfferId(int offerId)
        {
            return ctx.DepositRequests
                .Include(d => d.User)
                .Include(d => d.Offer)
                .FirstOrDefault(d => d.OfferId == offerId);
        }

        public DepositRequest Create(DepositRequest d)
        {
            var deposit = new DepositRequest
            {
                Amount = d.Amount,
                InvestorId = d.InvestorId,
                OfferId = d.OfferId,
                CRNo = d.CRNo,
                Status = d.Status
            };

            ctx.DepositRequests.Add(deposit);
            ctx.SaveChanges();
            return deposit;
        }

        public DepositRequest ApproveDeposit(DepositRequest d)
        {
            var deposit = GetDepositRequest(d.DepositRequestNo);
            if (deposit == null)
            {
                return null;
            }

            deposit.Status = "Approved";
            ctx.SaveChanges();
            return deposit;
        }


        public DepositRequest Update(DepositRequest d)
        {
            var deposit = GetDepositRequest(d.DepositRequestNo);
            if (deposit == null)
            {
                return null;
            }
            deposit.Amount= d.Amount;
            deposit.InvestorId = d.InvestorId;
            deposit.OfferId = d.OfferId;
            deposit.CRNo = d.CRNo;
            deposit.Status = d.Status;
            Console.WriteLine($"deposit status: {deposit.Status}");
            ctx.SaveChanges();

            return deposit;
        }

        public bool DeleteDeposit(int depositRequestNo)
        {
            var deposit = GetDepositRequest(depositRequestNo);
            if (deposit == null)
            {
                return false;
            }

            ctx.DepositRequests.Remove(deposit);
            ctx.SaveChanges();
            return true;
        }

        public IList<DepositRequest> GetDepositsByInvestor(int investorId)
        {
            return ctx.DepositRequests
                .Include(d => d.User)
                .Include(d => d.Offer)
                .Where(d => d.InvestorId == investorId)
                .ToList();
        }

        public IList<DepositRequest> GetDepositsByOffer(int offerId)
        {
            return ctx.DepositRequests
                .Include(d => d.User)
                .Include(d => d.Offer)
                .Where(d => d.OfferId == offerId)
                .ToList();
        }

        public IList<DepositRequest> GetDepositsByCompany(string crNo)
        {
            return ctx.DepositRequests
                .Include(d => d.User)
                .Include(d => d.Offer)
                .Include(d => d.Company)
                .Where(d => d.CRNo == crNo)
                .ToList();
        }

        public Paged<DepositRequest> GetDeposits(int page = 1, int size = 20, string orderBy = "DepositRequestNo", string direction = "asc")
        {
            var results = (orderBy.ToLower(), direction.ToLower()) switch
            {
                ("depositRequestNo", "asc") => ctx.DepositRequests.OrderBy(d => d.DepositRequestNo),
                ("depositRequestNo", "desc") => ctx.DepositRequests.OrderByDescending(d => d.DepositRequestNo),
                ("amount", "asc") => ctx.DepositRequests.OrderBy(d => d.Amount),
                ("amount", "desc") => ctx.DepositRequests.OrderByDescending(d => d.Amount),
                ("investorId", "asc") => ctx.DepositRequests.OrderBy(d => d.InvestorId),
                ("investorId", "desc") => ctx.DepositRequests.OrderByDescending(d => d.InvestorId),
                ("offerId", "asc") => ctx.DepositRequests.OrderBy(d => d.OfferId),
                ("offerId", "desc") => ctx.DepositRequests.OrderByDescending(d => d.OfferId),
                ("companyName", "asc") => ctx.DepositRequests.OrderBy(d => d.Company.CompanyName),
                ("companyName", "desc") => ctx.DepositRequests.OrderByDescending(d => d.Company.CompanyName),
                ("crno", "asc") => ctx.DepositRequests.OrderBy(d => d.CRNo),
                ("crno", "desc") => ctx.DepositRequests.OrderByDescending(d => d.CRNo),
                ("status", "asc") => ctx.DepositRequests.OrderBy(d => d.Status),
                ("status", "desc") => ctx.DepositRequests.OrderByDescending(d => d.Status),
                _ => ctx.DepositRequests.OrderBy(d => d.DepositRequestNo)
            };
            return results.Include(d => d.User).Include(d => d.Offer).Include(d => d.Company).ToPaged(page, size, orderBy, direction);
        }




    }




}
