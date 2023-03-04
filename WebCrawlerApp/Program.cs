using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Xml.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.ExceptionServices;
using Microsoft.FSharp.Data.UnitSystems.SI.UnitNames;

namespace WebCrawlerApp 
{
    class Program
    {
        static ScrapingBrowser scrapingBrowser= new ScrapingBrowser();
        static int maxRetryCount = 3;
        static TimeSpan retryDelay = TimeSpan.FromSeconds(10);

        public static HtmlNode GetHtml()
        {

            
            int minDelay = 5000;
            int maxDelay = 10000;
            Random rand = new Random();

            //tarayıcının çerezleri yok sayması sağlanıyor.
            //Bu işlem, tarayıcının önbelleğindeki veya önceden kaydedilmiş çerezlerin taranacak
            //sayfanın sonucunu etkilememesini sağlar.
            scrapingBrowser.IgnoreCookies = true;

            //tarayıcının sayfayı yüklemesi için verilen süre 15 dakika olarak ayarlanıyor.
            scrapingBrowser.Timeout = TimeSpan.FromMinutes(15);

            //tarayıcının kullanacağı user agent belirleniyor.
            //rand.Next(userAgents.Length) ifadesi,
            //userAgents dizisinden rastgele bir user-agent seçerek ip banlanmalarına karşı korruma sağlar.
            string[] userAgents = { "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3",
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36",
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.79 Safari/537.36"};
            string userAgent = userAgents[rand.Next(userAgents.Length)];
            scrapingBrowser.Headers["User-Agent"] = userAgent;

            //türkçe karakterler uygun olacak şekilde ayarlanıyor
            scrapingBrowser.Encoding = Encoding.UTF8;


            try
            {   
                //deneme sayısını tutmak için değişken
                int retryCount = 0;
                do
                {
                    // Rastgele bir Gecikme ekleme
                    int delay = rand.Next(minDelay, maxDelay); // Rastgele bir gecikme belirleyin (500-1500ms)
                    Thread.Sleep(delay); // Gecikmeyi uygulayın
                    //"https://www.sahibinden.com/" adresine bir istek gönderir.
                    WebPage webPage = scrapingBrowser.NavigateToPage(new Uri("https://www.sahibinden.com/"));
                    //Eğer istek başarılı olursa, "WebPage" nesnesi içindeki HTML içeriği döndürülür.
                    if (webPage != null)
                    {
                        return webPage.Html;
                    }
                    else
                    {
                        //başarısız olursa, "retryCount" arttırılır ve hata mesajı gösterilir.
                        //Daha sonra "retryDelay" değişkeni kadar gecikme uygulanır ve döngü yeniden başlatılır.
                        //Döngü, "maxRetryCount" değerine ulaşana kadar devam eder.
                        retryCount++;
                        Console.WriteLine($"Site erişiminde bir hata oluştu, yeniden denenecek ({retryCount}/{maxRetryCount})");
                        Thread.Sleep(retryDelay);
                    }
                } while (retryCount < maxRetryCount);
                // Max retry sayısına ulaşıldı, hata mesajı gösterin
                //Eğer maksimum yeniden deneme sayısına ulaşırsa,
                //hata mesajı gösterilir ve program sonlandırılır.
                //Eğer istek başarılı olursa, HTML içeriği döndürülür.
                Console.WriteLine($"Site erişimi engellenmiştir ({maxRetryCount} kez denendi), program sonlandırılıyor.");
                Environment.Exit(1);
                return null;

            }
            catch (WebException ex)
            {
                //hata mesajlarının yakalanması ve gösterilmesi
                Console.WriteLine("WebException occurred: {0}", ex.Message);
                Console.WriteLine("Status: {0}", ex.Status);
                Console.WriteLine("Response: {0}", ex.Response);
                Console.WriteLine("Source: {0}", ex.Source);
                Console.WriteLine("StackTrace: {0}", ex.StackTrace);
                throw;
            }
            catch (Exception e1)
            {
                //hata mesajlarının yakalanması ve gösterilmesi
                Console.WriteLine("Site erişimi engellenmiştir bir süre sonra tekrar deneyin. Message = {0}", e1.Message);
                Console.WriteLine("Stack trace: {0}", e1.StackTrace);
                throw;
            }
            
        }
        public static List<ProductDetail> GetProducts()
        {
            int minDelay = 5000;
            int maxDelay = 10000;
            Random rand = new Random();

            HtmlDocument doc = new HtmlDocument();
            var productDetails=new List<ProductDetail>();

            //GetHtml fonksiyonundan gerekli HTML alınır
            var html = GetHtml();

            //vitrin listin olduğu bölüm seçilir
            var links = html.CssSelect("div.uiBox.showcase>ul.vitrin-list.clearfix>li");
            
            //ürünler tek tek gezilir
            foreach (var item in links)
            {
                ProductDetail productDetail = new ProductDetail();

                //bir HTML belgesinin içeriğinin yüklenmesine izin verir
                //ve HtmlDocument nesnesi üzerinde gezinmeyi sağlar.
                doc.LoadHtml(item.InnerHtml);

                //ürün başlığına ulaşmak için gerekli node seçilir
                HtmlNode link = doc.DocumentNode.SelectSingleNode("//a[@title]");
                //eğer null değilse girilir burada reklam mı ürün mü kontrolü yapılır
                if (link != null)
                {
                    //başlık alınır ve encode edilerek türkçe karakterlere uygun hale getirilir.
                    string title = link.GetAttributeValue("title", "").Trim();
                    byte[] bytes = Encoding.UTF8.GetBytes(title);
                    title = Encoding.UTF8.GetString(bytes);
                    //başlığın detayına gitmek için gerekli olan adres alınır
                    string href = link.GetAttributeValue("href", "");
                    //tekrar bir istek yapılacağı için try catch bloğu açılır
                    try
                    {

                        //rastgele gecikme eklenir
                        int delay = rand.Next(minDelay, maxDelay);
                        Thread.Sleep(delay); // 5 saniyelik gecikme ekleme
                        WebPage webPage = scrapingBrowser.NavigateToPage(new Uri("https://www.sahibinden.com" + href));
                        HtmlNode pageHtml = webPage.Html;

                        //ürün fiyatına ulaşmak için gerekli css seçilir
                        var node = pageHtml.CssSelect("div.classifiedInfo>h3").FirstOrDefault();
                        if (node != null)
                        {
                            var price = node.ChildNodes[0].InnerText.Trim().Replace(" TL", "").Replace("€ ", "").Replace("$ ", "").Replace(".", "").Replace(",", "");
                            productDetail.Price = Int32.Parse(price);
                        }
                    }
                    catch (Exception e1)
                    {
                        Console.WriteLine("Site erişimi engellenmiştir bir süre sonra tekrar deneyin. Message = {0}", e1.Message);
                        Console.WriteLine("Stack trace: {0}", e1.StackTrace);
                        throw;
                    }
                    productDetail.ProductName = title;
                    productDetails.Add(productDetail);
                }
            }
            return productDetails;
        }
        static void Main(string[] args)
        {
            //masaüstünde oluşturulacak
            //dosya ismi verilir
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\productDetails.txt";
            List<ProductDetail> productDetails = new List<ProductDetail>();
            //ürünler çekilir
            productDetails = GetProducts();

            //konsol ekranına ürün ismi ve fiyatları yazılır
            foreach (ProductDetail product in productDetails)
            {
                Console.WriteLine($"Product Name: {product.ProductName}, Price: {product.Price} TL");
            }

            //fiyat ortalaması hesaplanır
            double averagePrice = productDetails.Average(p => p.Price);
            Console.WriteLine("Average Products: " + averagePrice);

            // Masaüstünde dosya oluşturularak dosyaya yazma işlemi yapılır
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (ProductDetail product in productDetails)
                {
                    string line = $"{product.ProductName}\t{product.Price}";
                    writer.WriteLine(line);
                }
            }
        }
    }



    public class ProductDetail
    {
        public string? ProductName { get; set; }
        public int Price { get; set; }
    }
}

