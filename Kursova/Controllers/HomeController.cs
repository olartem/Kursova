using Kursova.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Kursova.DAL;
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
            return View(await db.Products.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
