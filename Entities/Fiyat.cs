namespace UetdsProgramiNet.Entities
{
    public class Fiyat : BaseEntity
    {
        public string AracPaketi { get; set; }
        public string KullaniciMiktari { get; set; }
        public string MobilBilgisi { get; set; }
        public string DestekBilgisi { get; set; }
        public string DestekSaatleri { get; set; }
        public string YedeklemeTuru { get; set; }
        public string WhatsAppUrl { get; set; }
    }
}
