using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using TestProject.Models;

namespace TestProject.Services
{
    public class LinkParser
    {
        private WebAddress webAddress;

        public LinkParser(WebAddress webAddress)
        {
            this.webAddress = webAddress;
        }

        public async Task<WebAddress> GetLinksAsync(WebAddress wa)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            
            for(int i = 0; i < wa.Sitemaps.Count; i++)
            {
                var sitemap = wa.Sitemaps.ElementAt(i);
                var document = await context.OpenAsync(wa.Url + sitemap.SitemapLink);

                var links = document.QuerySelectorAll(" * :not(:has(*))").Where(v => v.TextContent.Contains(wa.Url)).Select(v => v.TextContent).ToList();

                if(!links.Any()) continue;

                if(links.First().EndsWith(".xml")) 
                {
                    foreach(var item in links)
                    {
                        wa.Sitemaps.Add(new Sitemap()
                        { 
                            SitemapLink = item.Substring(wa.Url.Length)
                        });
                    } 
                    wa.Sitemaps.Remove(sitemap);
                }
                else 
                {
                    sitemap.Pages = Page.toPage(links, wa.Url.Length);
                }
            }
            return wa;
        }
    }
}