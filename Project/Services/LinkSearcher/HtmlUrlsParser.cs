using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using TestProject.Data;
using TestProject.Models;
using Npgsql.EntityFrameworkCore;

namespace TestProject.Services
{
    public class HtmlUrlsParser {

        private AppDbContext db;

        public HtmlUrlsParser(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<WebAddress> GetLinksFromHtmlAsync(WebAddress wa)
        {
            
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var sitemap = new Sitemap{
                SitemapLink = "-",
                Pages = new List<Page>()
            };
            
            wa.Sitemaps = new List<Sitemap>();
            wa.Sitemaps.Add(sitemap);
            sitemap.Pages.Add(new Page{PageLink = "", Sitemap = sitemap});
            
            for (int i = 0; i < sitemap.Pages.Count; i++)
            {
                var document = await context.OpenAsync(wa.Url + sitemap.Pages.ElementAt(i).PageLink);
            
                var links = document.QuerySelectorAll("a")
                            .Where(l => ((IHtmlAnchorElement)l).Href.Count() > wa.Url.Length &&
                                        ((IHtmlAnchorElement)l).Href.Contains(wa.Url.Substring(5)) &&
                                        ((IHtmlAnchorElement)l).Href[wa.Url.Length+1] != '#')
                            .Select(l => ((IHtmlAnchorElement)l).Href.Substring(wa.Url.Length))
                            .ToList();

                sitemap = CheckLinksInDb(links.Distinct(), sitemap, wa.Url);

            } 
            
            return wa;
        }

        public Sitemap CheckLinksInDb(IEnumerable<string> list, Sitemap sitemap, string url)
        {
            var pages = sitemap.Pages;

            foreach(var item in list)
            {
                if(!pages.Where(dp => dp.PageLink == item).Any()) 
                pages.Add(new Page(){PageLink = item, Sitemap = sitemap});
            }

            return sitemap;
        }

    }
}