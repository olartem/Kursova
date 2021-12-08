using Kursova.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kursova.DAL;
using Microsoft.EntityFrameworkCore;

namespace Kursova.Controllers
{
    public class HomeController : Controller
    {

        private readonly ProductContext db;
        public HomeController(ProductContext context)
        {
            db = context;
        }


        public async Task<IActionResult> Index()
        {
            return View(await db.Products.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
