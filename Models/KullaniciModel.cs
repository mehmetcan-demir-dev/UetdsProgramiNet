
using System.ComponentModel.DataAnnotations;

public class KullaniciModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } // E-posta

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } // Şifre

    public bool RememberMe { get; set; } // Beni hatırla checkbox'ı
}
