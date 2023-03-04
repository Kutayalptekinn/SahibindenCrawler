`    `**Problem Sahibinden.com sitesinde ana sayfa vitrinindeki ilanların ismini ve ilanların fiyat bilgisini çekmek, bunları arayüzde göstermek ve olası ip banlamalarına karşı korunmaktan oluşuyor.** 

**Bu problemi çözmek için öncelikle siteye istek göndermek ve verileri çekmek için ScrapySharp kütüphanesi kullanıldı ScrapySharp kütüphanesinin bir ürünü olan ScrapingBrowser kullanılarak siteye istek atmadan önce gerekli ayarlamalar yapıldı bu ayarlamaların detayı yorum satırlarında gözüküyor.** 

![](Aspose.Words.b702b876-b30f-4d9c-8a20-8d2d6fff6db1.001.jpeg)

`    `**Siteye istek denemelerinin sayısının tutulması ve maximum deneme sayısı aşıldıysa denemeyi bırakmak için retryCount ve maxRetryCount adında iki değişken tutuluyor. IP banlamarından kaçınmak için siteye istek atmadan önce rastgele bekleme süreleri konuldu. ScrapingBrowser ile siteye istek atılır ve başarılı olursa sitenin HTML’İ fonksiyondan döndürülür.** 

![](Aspose.Words.b702b876-b30f-4d9c-8a20-8d2d6fff6db1.002.jpeg)

**Eğer hata oluşursa yakalamak için try/catch blokları konuldu.** 

![](Aspose.Words.b702b876-b30f-4d9c-8a20-8d2d6fff6db1.003.jpeg)

**Siteden html çekilerek istenen vitrin ilanlarının olduğu css seçiliyor** 

![](Aspose.Words.b702b876-b30f-4d9c-8a20-8d2d6fff6db1.004.png)**Vitrindeki ürünler tek tek gezilerek başlıkları ve detay adresleri alınır. Karakterler Encode edilerek Türkçe karakter hatası çözülür.** 

![](Aspose.Words.b702b876-b30f-4d9c-8a20-8d2d6fff6db1.005.jpeg)

**Detay adresine istek atmadan önce yine banlanma ihtimalini azaltmak için gecikme eklenir ve detay sitesine erişim isteğinde bulunularak ilgili css’den fiyat bilgisi çekilir.** 

![](Aspose.Words.b702b876-b30f-4d9c-8a20-8d2d6fff6db1.006.jpeg)

**Dosya işlemleri için dosya yolu verilir. Ürünler ve ortalama fiyat konsola yazdırılır.** 

![](Aspose.Words.b702b876-b30f-4d9c-8a20-8d2d6fff6db1.007.jpeg)

**Ürün detayları dosyaya yazılır.** 

![](Aspose.Words.b702b876-b30f-4d9c-8a20-8d2d6fff6db1.008.png)

**ÇIKTILAR** 

![](Aspose.Words.b702b876-b30f-4d9c-8a20-8d2d6fff6db1.009.jpeg)

![](Aspose.Words.b702b876-b30f-4d9c-8a20-8d2d6fff6db1.010.jpeg)
