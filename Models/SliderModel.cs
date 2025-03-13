using System.ComponentModel.DataAnnotations;

namespace UetdsProgramiNet.Models
{
    public class SliderModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Alt Başlık zorunludur.")]
        public string Subtitle { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Alt Açıklama zorunludur.")]
        public string SubDescription { get; set; }

        public string InfoUrl { get; set; }  // Burada [Required] attribute'ü kaldırıldı

        [Required(ErrorMessage = "Resim Yolu zorunludur.")]
        public string ImgUrl { get; set; }
    }

}
