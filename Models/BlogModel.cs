using System.ComponentModel.DataAnnotations;
using UetdsProgramiNet.Entities;

namespace UetdsProgramiNet.Models
{
    public class BlogModel : BaseEntity
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Başlık gereklidir")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Açıklama gereklidir")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Açıklama gereklidir")]
        public string SubDescription { get; set; }
        [Required(ErrorMessage = "Bağlantı yolu gereklidir")]
        public string InfoUrl { get; set; }
        [Required(ErrorMessage = "Resim yolu gereklidir")]
        public string ImgUrl { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}
