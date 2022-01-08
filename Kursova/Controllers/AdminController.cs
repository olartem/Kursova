using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kursova.DAL;
using Kursova.Models;
using Kursova.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kursova.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly UserManager<User> _userManager;

        public AdminController(ApplicationContext context,  UserManager<User> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> UserDashboard(SortState sortOrder = SortState.NameDesc)
        {
            var users = _userManager.Users;
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["EmailSort"] = sortOrder == SortState.EmailAsc ? SortState.EmailDesc : SortState.EmailAsc;

            users = sortOrder switch
            {
                SortState.NameAsc => users.OrderBy(s => s.UserName),
                SortState.EmailAsc => users.OrderBy(s => s.Email),
                SortState.EmailDesc => users.OrderByDescending(s => s.Email),
                _ => users.OrderByDescending(s => s.UserName),
            };
            return View(await users.ToListAsync());
        }
        public async Task<IActionResult> GameDashboard(SortState sortOrder = SortState.TitleDesc)
        {
            IQueryable<Game> games = _db.Games.Include(p=>p.Results);
            ViewData["TitleSort"] = sortOrder == SortState.TitleAsc ? SortState.TitleDesc : SortState.TitleAsc;
            ViewData["PriceSort"] = sortOrder == SortState.NumberAsc ? SortState.NumberDesc : SortState.NumberAsc;
            ViewData["PlayCountSort"] = sortOrder == SortState.PlayCountAsc ? SortState.PlayCountDesc : SortState.PlayCountAsc;

            games = sortOrder switch
            {
                SortState.TitleAsc => games.OrderBy(s => s.title),
                SortState.NumberAsc => games.OrderBy(s => s.number),
                SortState.NumberDesc => games.OrderByDescending(s => s.number),
                SortState.PlayCountAsc => games.OrderBy(s => s.Results.Count),
                SortState.PlayCountDesc => games.OrderByDescending(s => s.Results.Count),
                _ => games.OrderByDescending(s => s.title),
            };
            return View(await games.ToListAsync());
        }
        public async Task<IActionResult> ResultDashboard(string? game, string name)
        {
            IQueryable<GameResult> results = _db.Results.Include(g => g.Game).Include(u=>u.User).OrderByDescending(s=>s.score);
            if (game != null && game != "0")
            {
                results = results.Where(p => p.Game.Id == game);
            }
            if (!String.IsNullOrEmpty(name))
            {
                results = results.Where(p => p.User.UserName.Contains(name));
            }

            List<Game> games = await _db.Games.ToListAsync();
            games.Insert(0, new Game { title = "All", Id = "0" });

            ResultDashboardViewModel viewModel = new ResultDashboardViewModel
            {
                Results = await results.ToListAsync(),
                Games = new SelectList(games,"Id","title"),
                Name = name
            };
            return View(viewModel);
        }
        public IActionResult CreateUser() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.UserName };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.IsAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, "admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }
        public async Task<IActionResult> EditUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            AdminEditUserViewModel model = new AdminEditUserViewModel
                { Id = user.Id, Email = user.Email, UserName = user.UserName };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(AdminEditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserDashboard", "Admin");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            return View(model);
        }
    }
}
