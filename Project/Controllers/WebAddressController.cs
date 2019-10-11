using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;

namespace TestProject.Controllers
{
    public class WebAddressController : Controller
    {
        private readonly AppDbContext _context;

        public WebAddressController(AppDbContext context)
        {
            _context = context;
        }

        // GET: WebAddress
        public async Task<IActionResult> Index()
        {
            return View(await _context.WebAddresses.ToListAsync());
        }

        
    }
}
