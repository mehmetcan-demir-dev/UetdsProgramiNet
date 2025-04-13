using Microsoft.AspNetCore.Identity;

namespace UetdsProgramiNet.Entities
{
    public class Kullanici : BaseEntity
    {
        public string AdSoyad { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
