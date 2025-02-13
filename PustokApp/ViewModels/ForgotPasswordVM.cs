using System.ComponentModel.DataAnnotations;

namespace PustokApp.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
