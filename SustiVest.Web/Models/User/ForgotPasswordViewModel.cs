using System.ComponentModel.DataAnnotations;

namespace SustiVest.Web.Models.User;
public class ForgotPasswordViewModel
{
    [Required]
    public string Email { get; set; }
    
}
