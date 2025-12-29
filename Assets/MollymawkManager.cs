using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Bu satırı en üste ekle!
using System.Collections;

public class MollymawkManager : MonoBehaviour
{
    [Header("Sesler (.wav)")]
    public AudioSource sesKaynagi;
    public AudioClip ballSes, catSes, crownSes, dropSes;

    [Header("Görsel Semboller (Assets'teki Resimler)")]
    public Sprite ballSprite;
    public Sprite catSprite;
    public Sprite crownSprite;
    public Sprite dropSprite;
    private string gorselIsim = "";

    [Header("Ekran Kutuları (Hierarchy'deki Imageler)")]
    public Image solKutuImage; // Oyuncu Hedefi
    public Image sagKutuImage; // PC Hedefi

    [Header("Zaman ve Puan")]
    public Text timerText;
    public GameObject finalPanel;
    public Text finalPuanText;

    [Header("Konsantrasyon Barı")]
    public Slider konsantrasyonSlider;
    public float dususHizi = 0.08f; // 6 dakikada seni zorlayacak bir hız
    public float yukselisMiktari = 0.12f; // Space'e her basışta çıkış miktarı

    [Header("Uyarı Sistemleri")]
    public GameObject kirmiziPanel; // Oluşturduğun kırmızı Image
    public AudioClip bipSesi;      // Bip ses dosyası (.wav)

    [Header("Sembol Ayarları")]
    public string[] semboller = { "ball", "cat", "crown", "drop" }; // Hata veren kısım burasıydı, artık tanımlı.
    public Sprite[] sembolResimleri; // Müfettiş (Inspector) üzerinden 4 resmi buraya sürükle.

    [Header("Kural Sistemi")]
    public Image yasakliGostergeImage; // Sol üstteki kırmızı kutunun içindeki resim.
    private string yasakliSembol = "";
    private int turSayaci = 0;

    [Header("Buton Karıştırma Ayarları")]
    public RectTransform[] oyunButonlari; // Cat, Ball, Crown, Drop butonlarını buraya sürükle
    private Vector3[] butonPozisyonlari;  // Butonların orijinal yerlerini tutacak

    [Header("Loading Ayarları")]
    public GameObject loadingPaneli; // Tüm loading ekranı (Panel)
    public Image loadingBarImage;    // Üzerinde "Filled" ayarı yaptığın çubuk

    private float kalanSure = 360f;
    private int toplamPuan = 0;
    private bool oyunBitti = false;
    private string duyulanSes = "";
    private bool siraOyuncuda = true; // Oyun oyuncu hedefiyle başlasın
    public AudioClip[] sembolSesleri; // 4 adet ses dosyasını (ball, cat, crown, drop) buraya sürükleyeceğiz
    

    void Start()
    {
        Time.timeScale = 0f; 
        loadingPaneli.SetActive(true);
        loadingBarImage.fillAmount = 0f; 

        finalPanel.SetActive(false);
        solKutuImage.gameObject.SetActive(false);
        sagKutuImage.gameObject.SetActive(false);

        // Buton pozisyonlarını kaydetmek Start'ta kalabilir, sorun olmaz
        butonPozisyonlari = new Vector3[oyunButonlari.Length];
        for (int i = 0; i < oyunButonlari.Length; i++)
        {
            butonPozisyonlari[i] = oyunButonlari[i].localPosition;
        }

        // DİKKAT: Buradaki YeniTur() ve YasakliyiGuncelle() satırlarını SİLDİK.
        StartCoroutine(LoadingSureci());
    }

    IEnumerator LoadingSureci()
    {
        float sayac = 0f;
        float yuklemeSuresi = 3f; 

        while (sayac < yuklemeSuresi)
        {
            sayac += Time.unscaledDeltaTime; 
            loadingBarImage.fillAmount = sayac / yuklemeSuresi;
            yield return null;
        }

        // --- BURASI DEĞİŞTİ ---
        loadingPaneli.SetActive(false);
        Time.timeScale = 1f; 

        // --- SES KAYNAĞINI ZORLA AKTİF ET ---
        if (sesKaynagi != null)
        {
            sesKaynagi.enabled = true; // Bileşeni aç
            sesKaynagi.gameObject.SetActive(true); // Objeyi aç
        }

        // Sistemlerin kendine gelmesi için çok kısa bir "Gerçek Zamanlı" bekleme
        yield return new WaitForSecondsRealtime(0.2f); 

        YasakliyiGuncelle(); 
        // Sesin çalındığı ana fonksiyonu şimdi çağırıyoruz
        YeniTur(); 
    }

    void Update()
    {
        if (oyunBitti) return;
        
        // 6 Dakikalık Geri Sayım
        if (kalanSure > 0)
        {
            kalanSure -= Time.deltaTime;
            System.TimeSpan t = System.TimeSpan.FromSeconds(kalanSure);
            timerText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
        else { OyunBitis(); }

        HandleTimer(); // Süreyi yönetir
        HandleConcentration(); // Barı yönetir
    }

    void ButonlariKaristir()
    {
        // Pozisyon listesini karıştırmak için basit bir algoritma (Fisher-Yates)
        for (int i = 0; i < butonPozisyonlari.Length; i++)
        {
            Vector3 gecici = butonPozisyonlari[i];
            int rastgeleIndex = Random.Range(i, butonPozisyonlari.Length);
            butonPozisyonlari[i] = butonPozisyonlari[rastgeleIndex];
            butonPozisyonlari[rastgeleIndex] = gecici;
        }

        // Karıştırılmış yeni pozisyonları butonlara ata
        for (int i = 0; i < oyunButonlari.Length; i++)
        {
            oyunButonlari[i].localPosition = butonPozisyonlari[i];
        }

        Debug.Log("<color=blue>BUTONLARIN YERİ DEĞİŞTİ!</color>");
    }

    void HandleTimer()
    {
        if (kalanSure > 0)
        {
            kalanSure -= Time.deltaTime; // Her saniye gerçek zaman kadar düşer
            
            // Saniyeyi Dakika:Saniye formatına çevir (06:00 gibi)
            System.TimeSpan t = System.TimeSpan.FromSeconds(kalanSure);
            timerText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
        else 
        { 
            OyunBitis(); 
        }
    }

    void HandleConcentration()
    {
        if (oyunBitti || loadingPaneli.activeSelf) return; // Loading açıksa bar işlemleri yapma

        // 1. Barın sürekli düşmesini sağla
        konsantrasyonSlider.value -= dususHizi * Time.deltaTime;

        // 2. Kontrol Mekanizması (Space VEYA Dokunmatik)
        // EventSystem kontrolü: Sadece buton dışı boş bir yere dokunulursa bar yükselir
        if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()))
        {
            konsantrasyonSlider.value += yukselisMiktari;
            Debug.Log("Bar yükseltildi. Mevcut Değer: " + konsantrasyonSlider.value);
        }

        // 3. UYARI VE CEZA SİSTEMİ (Bar bittiğinde)
        if (konsantrasyonSlider.value <= 0)
        {
            // Eğer kırmızı panel henüz açık değilse aç ve bip sesi çal
            if (!kirmiziPanel.activeSelf)
            {
                kirmiziPanel.SetActive(true);
                
                if (bipSesi != null) 
                {
                    sesKaynagi.PlayOneShot(bipSesi);
                }
                
                Debug.LogWarning("DİKKAT: Bar boşaldı! Kırmızı uyarı aktif.");
            }
        }
        else
        {
            // Bar 0'ın üzerine çıktığı an (tıklandığı an) kırmızıyı kapat
            if (kirmiziPanel.activeSelf)
            {
                kirmiziPanel.SetActive(false);
                Debug.Log("Bar yükseldi, uyarı sistemi kapatıldı.");
            }
        }
    }

    Sprite ResimBul(string isim)
    {
        for (int i = 0; i < semboller.Length; i++)
        {
            if (semboller[i] == isim)
            {
                return sembolResimleri[i];
            }
        }
        Debug.LogError("Resim bulunamadı: " + isim);
        return null;
    }

    AudioClip SesBul(string isim)
    {
        // Listede arama yap
        for (int i = 0; i < semboller.Length; i++)
        {
            if (semboller[i].ToLower() == isim.ToLower())
            {
                // EĞER LISTE DOLUYSA SESİ DÖNDÜR
                if (sembolSesleri != null && i < sembolSesleri.Length && sembolSesleri[i] != null)
                {
                    return sembolSesleri[i];
                }
            }
        }
        
        // EĞER LISTE BOŞSA, YUKARIDAKİ TEKİL DEĞİŞKENLERİ DENE (B Planı)
        if (isim == "ball") return ballSes;
        if (isim == "cat") return catSes;
        if (isim == "crown") return crownSes;
        if (isim == "drop") return dropSes;

        Debug.LogError("DİKKAT: " + isim + " için ses dosyası bulunamadı! Inspector'ı kontrol et.");
        return null;
    }

    

    void YeniTur()
    {
        if (oyunBitti) return;

        // Her iki kutuyu da önce kapatıyoruz
        solKutuImage.gameObject.SetActive(false);
        sagKutuImage.gameObject.SetActive(false);

        // Yeni görseli seç (Daha önceki rastgele resim seçme mantığın)
        gorselIsim = semboller[Random.Range(0, semboller.Length)];
        
        // SIRA KONTROLÜ
        if (siraOyuncuda)
        {
            // Oyuncu Hedefi (SOL) Aktif
            solKutuImage.gameObject.SetActive(true);
            solKutuImage.sprite = ResimBul(gorselIsim);
            Debug.Log("Sıra Sizde: SOL kutu aktif.");
        }
        else
        {
            // AI Target (SAĞ) Aktif
            sagKutuImage.gameObject.SetActive(true);
            sagKutuImage.sprite = ResimBul(gorselIsim);
            Debug.Log("Sıra Bilgisayarda: SAĞ kutu aktif (Basmayın!).");
        }

        // Bir sonraki tur için sırayı değiştir
        siraOyuncuda = !siraOyuncuda;

        // Yeni sesi çal
        duyulanSes = semboller[Random.Range(0, semboller.Length)];
        sesKaynagi.PlayOneShot(SesBul(duyulanSes));
    }

    Sprite SpriteGetir(string ad)
    {
        if (ad == "ball") return ballSprite;
        if (ad == "cat") return catSprite;
        if (ad == "crown") return crownSprite;
        return dropSprite;
    }

    public void ButonaBasildi(string basilan)
    {
        if (oyunBitti) return;

        // Her butona basıldığında tur sayacını 1 artırıyoruz (Kural değişimi takibi için)
        turSayaci++; 
        
        string secim = basilan.ToLower();
        
        // Hangi butona basıldığını ve o anki durumu konsolda göster
        Debug.Log("Sizin Seçiminiz: " + secim + " | Tur: " + turSayaci);

        // --- KURALLAR VE HATA KONTROLLERİ ---
        
        // 1. Sağ kutu (AI Target) aktif mi ve basılan buton o resimle aynı mı?
        bool sagKutuAktif = sagKutuImage.gameObject.activeSelf;
        bool sagKutuHatasi = (sagKutuAktif && secim == gorselIsim); 
        
        // 2. Duyulan sesle aynı mı?
        bool sesHatasi = (secim == duyulanSes);
        
        // 3. O anki yasaklı sembol mü? (Her 10 turda bir değişen dinamik yasaklı)
        bool yasakliSembolHatasi = (secim == yasakliSembol);

        // --- PUANLAMA MANTIĞI ---
        if (sagKutuHatasi) 
        {
            toplamPuan -= 2;
            Debug.Log("<color=red>HATA:</color> Sağ kutudaki (PC hedefi) resme bastınız! -2 Puan.");
        }
        else if (sesHatasi) 
        {
            toplamPuan -= 2;
            Debug.Log("<color=red>HATA:</color> Duyduğunuz sese bastınız! -2 Puan.");
        }
        else if (yasakliSembolHatasi) 
        {
            toplamPuan -= 2;
            Debug.Log("<color=red>HATA:</color> O an yasaklı olan sembole (" + yasakliSembol + ") bastınız! -2 Puan.");
        }
        else 
        {
            toplamPuan += 1;
            Debug.Log("<color=green>TEBRİKLER:</color> Doğru seçim. +1 Puan.");
        }

        Debug.Log("GÜNCEL TOPLAM PUAN: " + toplamPuan);

        // --- KURAL DEĞİŞTİRME KONTROLÜ ---
        // Eğer tur sayısı 10'un katı ise (10, 20, 30...) yasaklıyı değiştiriyoruz
        if (turSayaci % 10 == 0)
        {
            YasakliyiGuncelle();   // Yasaklı sembol değişir
            ButonlariKaristir();   // Butonların yerleri değişir
        }

        // Bir sonraki tura/sesse geçmek için kısa bir bekleme süresi
        Invoke("YeniTur", 0.4f);
    }
    void SesCal(string isim)
    {
        AudioClip calinacakKlip = null;

        if (isim == "ball") calinacakKlip = ballSes;
        else if (isim == "cat") calinacakKlip = catSes;
        else if (isim == "crown") calinacakKlip = crownSes;
        else if (isim == "drop") calinacakKlip = dropSes;

        // KORUMA: Eğer klip hala boşsa hata verme, sadece uyar
        if (calinacakKlip != null && sesKaynagi != null)
        {
            sesKaynagi.PlayOneShot(calinacakKlip);
        }
        else
        {
            Debug.LogWarning("DİKKAT: " + isim + " sesi Inspector'da atanmamış!");
        }
    }

    void YasakliyiGuncelle()
    {
        Debug.Log("SES ÇALINMAYA ÇALIŞILIYOR!");
        
        // Rastgele yeni bir yasaklı seç
        int rastgeleIndex = Random.Range(0, semboller.Length);
        yasakliSembol = semboller[rastgeleIndex];
        
        // Kırmızı kutudaki resmi güncelle
        if (yasakliGostergeImage != null && sembolResimleri.Length > 0)
        {
            yasakliGostergeImage.sprite = sembolResimleri[rastgeleIndex];
        }
        
        Debug.Log("<color=red>KURAL DEĞİŞTİ!</color> Yeni Yasaklı: " + yasakliSembol);
    }


    void OyunBitis()
    {
        oyunBitti = true;
        finalPanel.SetActive(true);

        // Zamanı durdur (Arka planda oyunun akmaya devam etmemesi için)
        Time.timeScale = 0f; 

        string sonucMesaji = "";
        if (toplamPuan >= 90) {
            sonucMesaji = "BAŞARILI! \nSkorun: " + toplamPuan;
            finalPuanText.color = Color.green;
        } else {
            sonucMesaji = "GELİŞTİRMELİSİN \nSkorun: " + toplamPuan;
            finalPuanText.color = Color.red;
        }

        finalPuanText.text = sonucMesaji;
    }

    public void YenidenBaslat()
    {
        Time.timeScale = 1f; // Zamanı tekrar akıt
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    public void OyundanCik()
    {
        Application.Quit(); // Uygulamayı kapatır
        Debug.Log("Oyundan çıkıldı.");
    }
}