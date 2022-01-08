using Kursova.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Kursova.DAL;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;


namespace Kursova.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }


        public async Task<IActionResult> Index()
        {
            var games = await db.Games.Where(t => t.StartTime <= DateTime.Now).Where(t=>t.EndTime>=DateTime.Now).ToListAsync();
            return View(games);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
