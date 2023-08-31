using System.ComponentModel.DataAnnotations;

namespace SustiVest.Web.Models.User;  
public class LoginViewModel
{       
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

}
