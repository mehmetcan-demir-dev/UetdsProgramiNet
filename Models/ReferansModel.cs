
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UetdsProgramiNet.Models
{
    public class ReferansModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Açıklama gereklidir")]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }
        public IFormFile? Dosya { get; set; }
    }
}
