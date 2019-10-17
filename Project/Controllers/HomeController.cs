using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestProject.Models;
using TestProject.Services;
using System.Linq;
using TestProject.Data;
using System;
using System.Net;
using AngleSharp;
using AngleSharp.Html.Dom;
using Newtonsoft.Json;

namespace TestProject.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db;

        public HomeController(AppDbContext db)
        {
            this.db = db;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(WebAddress wa)
        {
            AjaxResponse ajaxResponse = new AjaxResponse();
            ajaxResponse.url = wa.Url;
            try
            {    
                if(!ModelState.IsValid ) 
                {
                    ajaxResponse.htmlResult = "<li>The URL field is not a valid fully-qualified http, https, or ftp URL.</li>";
                    return Content(JsonConvert.SerializeObject(ajaxResponse));
                }
                
                wa.Url = UrlFormater.GetLeftPart(wa.Url);
                ajaxResponse.url = wa.Url;

                if(db.WebAddresses.Where(w => w.Url == wa.Url).Any())
                {
                    ajaxResponse.htmlResult = "<li>This url has already checked.</li>";
                    return Content(JsonConvert.SerializeObject(ajaxResponse));
                }
                var sitemap = new Sitemap{
                    SitemapLink = "-",
                    Pages = new List<Page>()
                };
                sitemap.Pages.Add(new Page{PageLink = "", Sitemap = sitemap});

                if(wa.Sitemaps == null) wa.Sitemaps = new List<Sitemap>();
                
                SitemapSearcher searcher = new SitemapSearcher(wa);
                searcher.FindIn("robots.txt");
                wa = searcher.webAddress;

                if(wa.isSuccess)
                {
                    LinkParser ls = new LinkParser(searcher.webAddress);
                    wa = await ls.GetLinksAsync(wa);
                }
                
                ajaxResponse.htmlResult += "<li>Discovered: " + wa.Sitemaps.Count + " sitemaps</li>";

                
                wa.Sitemaps.Add(sitemap);
                db.WebAddresses.Add(wa);
                await db.SaveChangesAsync();

                ajaxResponse.isSuccess = true;                
                ajaxResponse.htmlResult += "<li>Discovered: " + db.Pages.Where(p => p.Sitemap.UId == wa.UId).Count() + " pages from sitemaps</li>";
                ajaxResponse.htmlResult += "<li>Starting to checking links...</li>";
            }
            catch 
            {
                ajaxResponse.isSuccess = false;
                ajaxResponse.htmlResult = "<li>Server error</li>";
            }                

            return Content(JsonConvert.SerializeObject(ajaxResponse));
        } 

        public async Task<IActionResult> AnalyzePage(string url)
        {
            AjaxResponse ajaxResponse = new AjaxResponse();
            ajaxResponse.url = url;

            System.Diagnostics.Stopwatch timer = new Stopwatch();
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);

                var pages = db.Pages;
                Page page; 
                
                if(pages.Where(p => p.Sitemap.WebAddress.Url == url && p.MinResponseTime == null).OrderBy(p => p.PId).Any())
                {
                    page = pages.Where(p => p.Sitemap.WebAddress.Url == url && p.MinResponseTime == null).OrderBy(p => p.PId).First();
                }
                else
                {
                    ajaxResponse.isSuccess = false;
                    ajaxResponse.htmlResult += "<li>Finish</li>";
                    return Content(JsonConvert.SerializeObject(ajaxResponse));
                }

                var defaultSitemap = db.Sitemaps.Where(s => s.WebAddress.Url == url && s.SitemapLink == "-").First();
                var document = await context.OpenAsync(url + page.PageLink);
                for (int i = 0; i < 3; i++)
                {
                    timer.Start();
                    document = await context.OpenAsync(url + page.PageLink);
                    var statusCode = document.StatusCode;
                    timer.Stop();
                    if(page.MinResponseTime == null || page.MinResponseTime > timer.Elapsed.TotalMilliseconds ) 
                        page.MinResponseTime = timer.Elapsed.TotalMilliseconds;
                    if(page.MaxResponseTime == null || page.MaxResponseTime < timer.Elapsed.TotalMilliseconds ) 
                        page.MaxResponseTime = timer.Elapsed.TotalMilliseconds;
                    timer.Reset();
                }
                var links = document.QuerySelectorAll("a")
                                .Where(l => ((IHtmlAnchorElement)l).Href.Count() > url.Length &&
                                            ((IHtmlAnchorElement)l).Href.Contains(UrlFormater.GetLeftPartWOHttp(url)) &&
                                            UrlFormater.GetRightPart( ((IHtmlAnchorElement)l).Href)[0] != '#')
                                .Select(l => UrlFormater.GetRightPart( ((IHtmlAnchorElement)l).Href) )
                                .ToList();

                foreach(var item in links)
                {
                    if(!pages.Local.Where(p => p.PageLink.Equals(item)).Any() && !pages.Where(p => p.PageLink.Equals(item)).Any()) pages.Add(new Page(){PageLink = item, Sitemap = defaultSitemap });
                }
                db.SaveChanges();

                ajaxResponse.isSuccess = true;
                ajaxResponse.htmlResult += "<li>" + url + page.PageLink + ": " + page.MinResponseTime + "~" 
                                            + page.MaxResponseTime + " ms";
            }
            catch 
            {
                ajaxResponse.isSuccess = true;
                ajaxResponse.htmlResult += "<li>Link Error</li>";
            }
            return Content(JsonConvert.SerializeObject(ajaxResponse));
        }
    }
}