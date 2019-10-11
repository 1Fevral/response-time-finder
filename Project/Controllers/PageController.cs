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
        private readonly AppDbContext _context;

        public PageController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Page
        public async Task<IActionResult> Index(int? id)
        {
            var appDbContext = _context.Pages.Include(p => p.Sitemap).Include(p => p.Sitemap.WebAddress);
            if(id == null)
            {
                return View(await appDbContext.ToListAsync());
            }
            var pages = appDbContext.Where(p => p.Sitemap.UId == id).OrderByDescending(p => p.ResponseTime);
            ViewBag.min = await pages.MinAsync(p=> p.ResponseTime);
            ViewBag.max = await pages.MaxAsync(p=>p.ResponseTime);

            return View(await pages.ToListAsync());

        }
        public string GetChartParams(int id)
        {
            var pages =  _context.Pages.Where(p => p.ResponseTime != null).ToList();
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
    }
}
