    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;
    using SustiVest.Data.Entities;
    using System.ComponentModel.DataAnnotations.Schema;
    
    namespace SustiVest.Data.Entities
{
    // Add User roles relevant to your application
    public enum Role { admin, borrower, investor, analyst, guest }
    
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // User role within application
        public Role Role { get; set; } // admin, borrower, investor, analyst, guest
        //database values for Role enum 0, 1, 2, 3, 4

    }
}
