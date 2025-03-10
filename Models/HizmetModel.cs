using System.ComponentModel.DataAnnotations;
namespace UetdsProgramiNet.Models
{
    public class HizmetModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Icon gereklidir")]
        public string IconUrl { get; set; }
        [Required(ErrorMessage = "Başlık gereklidir")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Açıklama gereklidir")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Bağlantı gereklidir")]
        public string InfoUrl { get; set; }

    }
}
