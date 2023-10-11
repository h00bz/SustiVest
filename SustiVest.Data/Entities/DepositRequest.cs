using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage;
using System.Runtime;


namespace SustiVest.Data.Entities
{
    public class DepositRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepositRequestNo { get; set; }

        public int Amount { get; set; }
        
        public int InvestorId { get; set; }
        
        public int OfferId { get; set; }

        public string CRNo { get; set; }

        public string Status { get; set; }
       
        [ForeignKey("CRNo")]
        public Company Company { get; set; }
        [ForeignKey("InvestorId")]
        public User User { get; set; }
        [ForeignKey("OfferId")]
        public Offer Offer { get; set; }
    }
}
