using System.ComponentModel.DataAnnotations;
namespace UetdsProgramiNet.Models
{
    public class FiyatModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Arac Sayısı Açıklaması zorunludur.")]
        public string AracSayiAciklamasi { get; set; }

        [Required(ErrorMessage = "Kullanıcı Miktarı zorunludur.")]
        public string KullaniciMiktari { get; set; }

        [Required(ErrorMessage = "Mobil Ücretsiz Mi zorunludur.")]
        public string MobilBilgisi { get; set; }

        [Required(ErrorMessage = "Destek Ücretsiz Mi zorunludur.")]
        public string DestekBilgisi { get; set; }

        [Required(ErrorMessage = "Destek Saatleri zorunludur.")]
        public string DestekSaatleri { get; set; }

        [Required(ErrorMessage = "Yedekleme Türü zorunludur.")]
        public string YedeklemeTuru { get; set; }

    }
}
