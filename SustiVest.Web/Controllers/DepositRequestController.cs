using Microsoft.AspNetCore.Mvc;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SustiVest.Web.Controllers
{
    [Authorize(Roles = "admin, analyst, investor")]
    public class DepositRequestController : BaseController
    {
        private readonly IDepositRequestService _svc;

        public DepositRequestController(IDepositRequestService svc)
        {
            _svc = svc;
        }

        // GET /depositrequest
        [Authorize(Roles = "admin, analyst, investor")]
        [HttpGet]
        public IActionResult Details(int depositRequestNo)
        {
            var deposit = _svc.GetDepositRequest(depositRequestNo);
            if (deposit == null)
            {
                Alert("Deposit Request Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return View(deposit);
        }

        [Authorize(Roles = "admin, analyst, investor")]
        [HttpGet]
        public IActionResult Create(int offerId, string crNo)
        {
            var deposit = new DepositRequest
            {
                OfferId = offerId,
                CRNo = crNo,
                InvestorId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value)
            };
            return View(deposit);
        }
      
        [Authorize(Roles = "admin, analyst, investor")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create([Bind("OfferId, Amount, InvestorId, CRNo, Status")] DepositRequest d)
        {
            if (ModelState.IsValid)
            {
                var deposit = _svc.Create(d);

                if (deposit == null)
                {
                    Alert("Encountered issue creating the deposit request", AlertType.warning);
                    return RedirectToAction(nameof(Index));
                }

                Alert("Deposit requested successfully ", AlertType.success);
                return RedirectToAction(nameof(Details), new { depositRequestNo = deposit.DepositRequestNo });
            }

            return View(d);
        }

            [Authorize(Roles = "admin, analyst")]
            [HttpGet]
            public IActionResult Edit(int depositRequestNo)
            {
                var deposit = _svc.GetDepositRequest(depositRequestNo);
                Console.WriteLine($"get deposit status: {deposit.Status}");
                if (deposit == null)
                {
                    Alert("Deposit Request Not Found", AlertType.warning);
                    return RedirectToAction(nameof(Index));
                }
                return View("Edit", deposit);
            }

            [Authorize(Roles = "admin, analyst")]
            [ValidateAntiForgeryToken]
            [HttpPost]
            public IActionResult Edit (DepositRequest d)
            
            {
                if (ModelState.IsValid)
                {
                    var updatedDeposit = _svc.Update(d);

                    if (updatedDeposit == null)
                    {
                        Alert("Encountered issue updating the deposit request", AlertType.warning);
                        return RedirectToAction(nameof(Index));
                    }
                    
                    Console.WriteLine($"Cont deposit status: {updatedDeposit.Status}");

                    Alert("Deposit request updated successfully ", AlertType.success);
                    return RedirectToAction(nameof(Details), new { depositRequestNo = updatedDeposit.DepositRequestNo });
                }

                return View(d);
            }

            [Authorize(Roles = "admin, analyst")]
            [HttpGet]
            public IActionResult Delete(int depositRequestNo)
            {
                var deposit = _svc.GetDepositRequest(depositRequestNo);
                if (deposit == null)
                {
                    Alert("Deposit Request Not Found", AlertType.warning);
                    return RedirectToAction(nameof(Index));
                }
                return View(deposit);
            }

            [Authorize(Roles = "admin, analyst")]
            [ValidateAntiForgeryToken]
            [HttpPost]
            public IActionResult DeleteConfirm(int depositRequestNo)
            {
                var deleted = _svc.DeleteDeposit(depositRequestNo);
                if (deleted)
                {
                    Alert("Deposit request deleted successfully.", AlertType.success);
                }
                else
                {
                    Alert("Deposit request could not  be deleted", AlertType.warning);
                }

                // redirect to the index view
                return RedirectToAction(nameof(Index));
            }

        [Authorize(Roles = "admin, analyst")]
        [HttpGet]
        public IActionResult Approve(int depositRequestNo)
        {
            var deposit = _svc.GetDepositRequest(depositRequestNo);
            if (deposit == null)
            {
                Alert("Deposit Request Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            return View(deposit);
        }

        [Authorize(Roles = "admin, analyst")]
        [HttpPost]
        public IActionResult Approve(DepositRequest d)
        {
            var depositRequest= _svc.GetDepositRequest(d.DepositRequestNo);
            
   
            if (ModelState.IsValid)
            {
                var depositApproved = _svc.ApproveDeposit(depositRequest);
                
                if (depositApproved == null)
                {
                    Alert("Encountered issue approving the deposit request", AlertType.warning);
                    return RedirectToAction(nameof(Index));
                }
                               
                Alert("Deposit request approved", AlertType.success);
                return RedirectToAction(nameof(Details), new { depositRequestNo = d.DepositRequestNo });
            }

            return View(d);
        }
        [Authorize(Roles = "admin, analyst")]
        public IActionResult Index(int page = 1, int size = 20, string orderBy = "DepositRequestNo", string direction = "asc")
        {
            var deposits = _svc.GetDeposits(page, size, orderBy, direction);
            return View(deposits);
        }

    }
}