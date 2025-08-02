using HtmlAgilityPack;
using ProductService.Data;
using ProductService.Grpc;
using ProductService.Models;

namespace ProductService.Utils
{
    public class TestingUtils
    {
        private const string BaseUrl = "https://books.toscrape.com/";
        public async Task<List<Product>> ParseProductsAsync(IConfiguration config, int maxPages = 1)
        {
            var httpClient = new HttpClient();
            var products = new List<Product>();

            for (int page = 1; page <= maxPages; page++)
            {
                var pageUrl = page == 1 ? $"{BaseUrl}catalogue/page-1.html" : $"{BaseUrl}catalogue/page-{page}.html";
                var html = await httpClient.GetStringAsync(pageUrl);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var bookNodes = htmlDoc.DocumentNode.SelectNodes("//article[@class='product_pod']");

                if (bookNodes == null) continue;

                foreach (var bookNode in bookNodes)
                {
                    var titleNode = bookNode.SelectSingleNode(".//h3/a");
                    var name = titleNode.GetAttributeValue("title", "").Trim();

                    var priceNode = bookNode.SelectSingleNode(".//p[@class='price_color']");
                    var priceStr = priceNode?.InnerText?.Replace("£", "").Trim();
                    int.TryParse(priceStr?.Split('.')[0], out int price);

                    var imgNode = bookNode.SelectSingleNode(".//img");
                    var imgSrc = imgNode.GetAttributeValue("src", "").Replace("../", "");
                    var fullImgUrl = $"{BaseUrl}{imgSrc}";

                    var productLinkNode = bookNode.SelectSingleNode(".//h3/a");
                    var productHref = productLinkNode.GetAttributeValue("href", "").Replace("../", "");
                    var productPageUrl = $"{BaseUrl}catalogue/{productHref}";
                    var productDetailHtml = await httpClient.GetStringAsync(productPageUrl);

                    var detailDoc = new HtmlDocument();
                    detailDoc.LoadHtml(productDetailHtml);

                    var descNode = detailDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");
                    var description = descNode?.GetAttributeValue("content", "").Trim() ?? "No description";

                    var attrTable = detailDoc.DocumentNode.SelectNodes("//table[@class='table table-striped']/tr");
                    var attributes = new List<ProductAttribute>();

                    if (attrTable != null)
                    {
                        foreach (var row in attrTable)
                        {
                            var key = row.SelectSingleNode("./th")?.InnerText?.Trim();
                            var val = row.SelectSingleNode("./td")?.InnerText?.Trim();
                        }
                    }

                    var product = new Product
                    {
                        Name = name,
                        Price = price,
                        Description = description,
                        ImageURLs = new List<string> { fullImgUrl },
                        Attributes = attributes,
                        CanBeOrdered = true,
                        StockQuantity = new Random().Next(0, 100),
                        OwnerId = "fanifffs",
                        CreatedAt = DateTimeUtil.GetCurrentTimeFormatted(config),
                        Type = Protos.ProductType.Shirts,
                        UpdatedAt = DateTimeUtil.GetCurrentTimeFormatted(config),
                        ParentCardId = string.Empty

                    };

                    products.Add(product);
                }
            }

            return products;
        }

        public void Proccess(List<Product> products, IProductRepo repo)
        {
            foreach (var product in products) 
            {
                repo.CreateProductAsync(product);
            }
        }
    }
}
