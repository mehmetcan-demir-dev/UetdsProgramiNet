<script>
    document.addEventListener('DOMContentLoaded', function () {
        // Modalı açma fonksiyonu
        function openModal(title, content) {
            document.getElementById('modalLabel').textContent = title;
            document.getElementById('modalBody').innerHTML = content;
            document.getElementById('customModal').style.display = 'block';
        }
          // Modalı kapatma fonksiyonu
          function closeModal() {
        document.getElementById('customModal').style.display = 'none';
          }
    // Kapatma butonuna basıldığında modalı kapat
    document.querySelector('.btn-close').addEventListener('click', closeModal);
    document.querySelector('.btn-secondary').addEventListener('click', closeModal);
          // Linklere tıklanıldığında modalı aç
          document.querySelectorAll('a').forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault(); // Linkin normal işlevini engelle
            if (this.textContent.includes('Gizlilik Politikası')) {
                openModal(
                    'Gizlilik Politikası',
                    `<p>Gizliliğiniz bizim için önemlidir. Bu gizlilik politikası, topladığımız bilgilerin türlerini, bu bilgilerin nasıl kullanıldığını ve korunmasını açıklar.</p>
                   <ul>
                     <li><strong>Toplanan Bilgiler:</strong> Ad, e-posta adresi, IP adresi gibi kişisel bilgiler toplanabilir.</li>
                     <li><strong>Kullanım Amaçları:</strong> Hizmetleri sağlamak, kullanıcı deneyimini geliştirmek, iletişim kurmak.</li>
                     <li><strong>Veri Paylaşımı:</strong> Üçüncü taraflarla paylaşılmaz, yasal zorunluluk dışında açıklanmaz.</li>
                     <li><strong>Güvenlik:</strong> Kişisel verilerinizin güvenliği için uygun önlemler alınmaktadır.</li>
                   </ul>
                   <p>Bu politika zaman zaman güncellenebilir. Lütfen düzenli olarak kontrol ediniz.</p>`
                );
            }
            if (this.textContent.includes('Hizmet Şartları')) {
                openModal(
                    'Hizmet Şartları',
                    `<p>Bu web sitesini kullanarak aşağıdaki hizmet şartlarını kabul etmiş olursunuz:</p>
                   <ul>
                     <li><strong>Kullanım Koşulları:</strong> Siteyi yalnızca yasal amaçlar için kullanabilirsiniz.</li>
                     <li><strong>Fikri Mülkiyet:</strong> Sitedeki tüm içerikler bize aittir ve izinsiz kullanılamaz.</li>
                     <li><strong>Sorumluluk Reddi:</strong> Verilen bilgilerin doğruluğu garanti edilmez.</li>
                     <li><strong>Değişiklik Hakkı:</strong> Hizmet şartlarını önceden bildirim yapmadan değiştirme hakkımız saklıdır.</li>
                   </ul>
                   <p>Lütfen siteyi kullanmadan önce bu şartları dikkatlice okuyunuz.</p>`
                );
            }
        });
          });
        });
</script>