using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kursova.DAL;
using Kursova.Models;
using Kursova.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kursova.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly ApplicationContext db;
        private readonly UserManager<User> _userManager;

        public GameController(ApplicationContext context, UserManager<User> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "admin")]
        public IActionResult Create() => View();

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var files = Request.Form.Files;
                if (files.Count > 0)
                {
                    MemoryStream ms = new MemoryStream();
                    await files.First().CopyToAsync(ms);
                    var imageData = ms.ToArray();
                    ms.Close();
                    await ms.DisposeAsync();
                    Game game = new Game
                    { number = model.number, title = model.title, description = model.description, image = imageData, EndTime = model.EndTime, StartTime = model.StartTime };
                    db.Games.Add(game);
                }
                else
                {
                    Game game = new Game
                    { number = model.number, title = model.title, description = model.description, EndTime = model.EndTime, StartTime = model.StartTime, image = null };
                    db.Games.Add(game);
                }


                await db.SaveChangesAsync();
                return RedirectToAction("GameDashboard", "Admin");
            }
            return View(model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {

            Game game = await db.Games.FirstOrDefaultAsync(p => p.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            if (game.image != null)
            {
                string imageBase64Data =
                    Convert.ToBase64String(game.image);
                string imageUrl =
                    string.Format("data:image/*;base64,{0}",
                        imageBase64Data);
                ViewBag.ImageUrl = imageUrl;
            }
            else
            {
                ViewBag.ImageUrl = "/noimage.jpg";
            }
            DetailsGameViewModel model = new DetailsGameViewModel
            { title = game.title, number = game.number, description = game.description, StartTime = game.StartTime, EndTime = game.EndTime };
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string id)
        {
            Game game = await db.Games.FirstOrDefaultAsync(p => p.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            EditGameViewModel model = new EditGameViewModel
            { Id = game.Id, title = game.title, description = game.description, image = game.image, number = game.number, EndTime = game.EndTime, StartTime = game.StartTime };
            return View(model);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditGameViewModel model)
        {
            if (ModelState.IsValid)
            {
                Game game = await db.Games.FirstOrDefaultAsync(p => p.Id == model.Id);
                if (game != null)
                {
                    game.description = model.description;
                    game.title = model.title;
                    game.number = model.number;
                    game.EndTime = model.EndTime;
                    game.StartTime = model.StartTime;
                    db.Games.Update(game);
                    await db.SaveChangesAsync();
                    return RedirectToAction("GameDashboard", "Admin");
                }
            }
            return View(model);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            Game game = await db.Games.FirstOrDefaultAsync(p => p.Id == id);
            if (game != null)
            {
                db.Games.Remove(game);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("GameDashboard", "Admin");
        }

        public async Task<IActionResult> UserGames()
        {
            var results = await db.Results.ToListAsync();
            var userPurchases = from t in results
                                where t.User.Id == User.FindFirstValue(ClaimTypes.NameIdentifier)
                                select t;
            return View(userPurchases.ToList());
        }

    }
}

