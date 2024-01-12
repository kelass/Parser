using AngleSharp;
using AngleSharp.Dom;
using OpenQA.Selenium.Chrome;
using Prysukha.Parser.Domain.Models;
using Prysukha.Parser.Infrastructure.Interfaces;

namespace Prysukha.Parser.Infrastructure.Services
{
    public class Parser : IParser
    {
        private string _url = "https://www.add.ua/ua/";
        private readonly Dictionary<string, string> _categories = new Dictionary<string, string>();
        private readonly IJsonSaver _jsonSaver;
        public Parser(IJsonSaver jsonSaver)
        {
            _jsonSaver = jsonSaver;
        }

        private async Task<IDocument> OpenNewPageAsync(string url)
        {
            using (var driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(url);

                var html = driver.PageSource;
                var context = BrowsingContext.New(Configuration.Default);
                return await context.OpenAsync(req => req.Content(html));
            }
        }

        private Dictionary<string, string> SaveCategoriesLinks(IDocument document)
        {
            var categories = document.GetElementsByClassName("advancedmenu-link level-top");
            foreach (var category in categories)
            {
                _categories.Add(category.TextContent, category.GetAttribute("href"));
            }
            return _categories;
        }

        private async Task ParseItemsInCategoryAsync()
        {
            foreach (var category in _categories)
            {
                await ParseCatalogAsync(category.Value);
            }
        }

        private async Task ParseCatalogAsync(string url)
        {

            var document = await OpenNewPageAsync(url);
            int lastPage = Convert.ToInt32(document.GetElementsByClassName("page last").Last().Children.Last().InnerHtml);
            for (int i = 1; i <= lastPage; i++)
            {
                try
                {
                    if (i != 1)
                        document = await OpenNewPageAsync($"{url}?p={i}");

                    var items = document.GetElementsByClassName("item product product-item");
                    foreach (var item in items)
                    {
                        Item data = new Item
                        {
                            ItemCode = item.GetElementsByClassName("product sku product-item-sku").First().TextContent,
                            ItemCreator = item.GetElementsByClassName("value").First().TextContent,
                            ItemName = item.GetElementsByClassName("product-item-link").First().TextContent,
                            ItemPrice = item.GetElementsByClassName("price").First().TextContent,
                        };
                        await _jsonSaver.SaveToJson(data);
                    }
                }
                catch (Exception ex)
                {
                }
            }


        }

        public async Task StartParsingAsync()
        {
            var homePageDom = await OpenNewPageAsync(_url);
            SaveCategoriesLinks(homePageDom);
            await ParseItemsInCategoryAsync();
        }
    }
}
