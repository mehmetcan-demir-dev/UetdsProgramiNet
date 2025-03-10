
using System.ComponentModel.DataAnnotations;

namespace UetdsProgramiNet.Models
{
    public class ReferansModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Açıklama gereklidir")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Resim URL gereklidir")]
        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
