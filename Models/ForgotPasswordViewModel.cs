using System.ComponentModel.DataAnnotations;

namespace UetdsProgramiNet.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
