using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestProject.Data;
using TestProject.Models;

namespace TestProject.Controllers
{
    public class PageController : Controller
    {
        private readonly AppDbContext db;

        public PageController(AppDbContext context)
        {
            db = context;
        }

        // GET: Page
        public async Task<IActionResult> Index(int? id)
        {
            var pages = db.Pages.Where(p => p.Sitemap.UId == id)
                                .OrderByDescending(p => p.ResponseTime);
            ViewBag.min = await pages.MinAsync(p=> p.ResponseTime);
            ViewBag.max = await pages.MaxAsync(p=>p.ResponseTime);
            ViewBag.id = id;
            return View();
        }
        public string GetChartParams(int id)
        {
            var pages =  db.Pages.Where(p => p.ResponseTime != null && p.Sitemap.UId == id).Take(350).ToList();
            var res = new ChartParams();
            res.links = new List<string>();
            res.values = new List<double?>();
            foreach(var page in pages)
            {
                res.links.Add(page.PageLink);
                res.values.Add(page.ResponseTime);
            }
            return JsonConvert.SerializeObject(res);
        }

        public async Task<IActionResult> GetPagesWithAjax(int? id, int startFrom, int numToTake)
        {
            var pages = db.Pages
                            .Include(p => p.Sitemap)
                            .Include(p => p.Sitemap.WebAddress);

            if(id == null) return PartialView("__PagesPartial", await pages.Skip(startFrom).Take(numToTake).ToListAsync());
            else 
            {
                var filteredPages = pages.Where(p => p.Sitemap.UId == id)
                                    .OrderByDescending(p => p.ResponseTime);
                if(filteredPages.Count() >= startFrom) return PartialView("__PagesPartial", await filteredPages.Skip(startFrom)
                                                                                                                .Take(numToTake)
                                                                                                                .ToListAsync());
                else return Content("Finish");
            }
        }
        
    }
}
