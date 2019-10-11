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
using System.Threading;

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
            if(!ModelState.IsValid ) 
            {
                return Content("<li>The URL field is not a valid fully-qualified http, https, or ftp URL.</li>");
            }
            
            wa.Url = UrlFormater.Formate(wa.Url);

            if(db.WebAddresses.Where(w => w.Url == wa.Url).Any())
            {
                return Content("<li>Starting to rechecking links...</li><li>This url has already checked.</li>");
            }

            ViewBag.Messages = new List<string>();

            SitemapSearcher searcher = new SitemapSearcher(wa);
            searcher.FindIn("robots.txt");
            
            if(!wa.isSuccess) {
                return Content("<li>No sitemap found</li>");
            }

            LinkParser ls = new LinkParser(searcher.webAddress);
            
            db.WebAddresses.Add(await ls.GetLinksAsync(wa));
            await db.SaveChangesAsync();
            
            ViewBag.Messages.Add("Starting to checking links...");
            ViewBag.Messages.Add("Discovered: " + db.Pages.Where(p => p.Sitemap.UId == wa.UId).Count() + " pages");
            ViewBag.Messages.Add("Discovered: " + wa.Sitemaps.Count + " sitemaps");
            
            return PartialView("Result", wa);
        } 

        public IActionResult Result()
        {
            return View();
        }

        public async Task<IActionResult> GetPages(string url,int startFrom, int numToTake)
        {
            url = UrlFormater.Formate(url);
            ViewBag.url = url;
            var webAddress = db.WebAddresses.Where(wa => wa.Url == url).First();
            System.Diagnostics.Stopwatch timer = new Stopwatch();
            HttpWebRequest request;
            HttpWebResponse response;

            var pages = db.Pages.Where(p => p.Sitemap.UId == webAddress.UId).OrderBy(p => p.PId);
            if(pages.Count() > startFrom) 
            {
                var partOfPages = pages.Skip(startFrom).Take(numToTake);
                foreach(var page in partOfPages)
                {
                    request = (HttpWebRequest)WebRequest.Create(url + page.PageLink);

                    timer.Start();

                    response = (HttpWebResponse)request.GetResponse();
                    response.Close ();

                    timer.Stop();
                    
                    page.ResponseTime = timer.Elapsed.Milliseconds;
                    timer.Reset();
                }
                db.Pages.UpdateRange(partOfPages); 
                await db.SaveChangesAsync();
                return PartialView("GetPages", partOfPages.ToList());
            }
            else return Content("Success! Your result saved to history.");
        }
    
    }
}