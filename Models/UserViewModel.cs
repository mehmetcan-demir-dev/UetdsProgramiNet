using UetdsProgramiNet.Entities;

namespace UetdsProgramiNet.Models
{
    public class UserViewModel
    {
        public List<Blog> BlogData { get; set; }
        public List<Fiyat> FiyatData { get; set; }
        public List<Hizmet> HizmetData { get; set; }
        public List<Referans> ReferansData { get; set; }
        public List<Slider> SliderData { get; set; }
    }
}
