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
            try
            {
                AjaxResponse ajaxResponse = new AjaxResponse();
                ajaxResponse.url = wa.Url;
                
                if(!ModelState.IsValid ) 
                {
                    ajaxResponse.htmlResult = "<li>The URL field is not a valid fully-qualified http, https, or ftp URL.</li>";
                    return Content(JsonConvert.SerializeObject(ajaxResponse));
                }
                
                wa.Url = UrlFormater.Formate(wa.Url);

                if(db.WebAddresses.Where(w => w.Url == wa.Url).Any())
                {
                    ajaxResponse.htmlResult = "<li>This url has already checked.</li>";
                    return Content(JsonConvert.SerializeObject(ajaxResponse));
                }

                ViewBag.Messages = new List<string>();

                SitemapSearcher searcher = new SitemapSearcher(wa);
                searcher.FindIn("robots.txt");
                
                if(!searcher.webAddress.isSuccess) 
                {
                    db.WebAddresses.Add(searcher.webAddress);
                    await db.SaveChangesAsync();
                    ajaxResponse.htmlResult = "<li>No sitemap found</li>";
                }
                else 
                {
                    LinkParser ls = new LinkParser(searcher.webAddress);
                    db.WebAddresses.Add(await ls.GetLinksAsync(wa));
                    await db.SaveChangesAsync();
                    
                    ajaxResponse.isSuccess = true;
                    ajaxResponse.htmlResult += "<li>Starting to checking links...</li>";
                    ajaxResponse.htmlResult += "<li>Discovered: " + db.Pages.Where(p => p.Sitemap.UId == wa.UId).Count() + " pages</li>";
                    ajaxResponse.htmlResult += "<li>Discovered: " + wa.Sitemaps.Count + " sitemaps</li>";
                }
                return Content(JsonConvert.SerializeObject(ajaxResponse));
            }
            catch { return Content("Error"); }
        } 

        public async Task<IActionResult> GetPages(string url,int startFrom, int numToTake)
        {
            try
            {
                url = UrlFormater.Formate(url);
                ViewBag.url = url;
                var webAddress = db.WebAddresses.Where(wa => wa.Url == url).First();
                System.Diagnostics.Stopwatch timer = new Stopwatch();
                HttpWebRequest request;
                HttpWebResponse response;

                var pages = db.Pages.Where(p => p.Sitemap.UId == webAddress.UId)
                                    .OrderBy(p => p.PId);
                if(pages.Count() >= startFrom) 
                {
                    var partOfPages = pages.Skip(startFrom).Take(numToTake);
                    foreach(var page in partOfPages)
                    {
                        page.ResponseTime = 0;
                        while(page.ResponseTime < 50)
                        {
                            request = (HttpWebRequest)WebRequest.Create(url + page.PageLink);

                            timer.Start();

                            response = (HttpWebResponse)request.GetResponse();
                            response.Close ();

                            timer.Stop();

                            page.ResponseTime = timer.Elapsed.Milliseconds;
                            timer.Reset();
                        }
                    }
                    db.Pages.UpdateRange(partOfPages); 
                    await db.SaveChangesAsync();
                    return PartialView("GetPages", partOfPages.ToList());
                }
                else return Content("Success! Your result saved to history.");
            }
            catch { return Content("Server Error");}
        }
    
    }
}