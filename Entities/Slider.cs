using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UetdsProgramiNet.Entities
{
    public class Slider : BaseEntity
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string SubDescription { get; set; }
        public string InfoUrl { get; set; }
        public string ImgUrl { get; set; }
    }
}
