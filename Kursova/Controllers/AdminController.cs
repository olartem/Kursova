using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Kursova.DAL;
using Kursova.Models;
using Kursova.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        public async Task<IActionResult> ProductDashboard(SortState sortOrder = SortState.TitleDesc)
        {
            IQueryable<Product> products = _db.Products.Include(p=>p.purchases);
            ViewData["TitleSort"] = sortOrder == SortState.TitleAsc ? SortState.TitleDesc : SortState.TitleAsc;
            ViewData["PriceSort"] = sortOrder == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;
            ViewData["PurchaseCountSort"] = sortOrder == SortState.PurchaseCountAsc ? SortState.PurchaseCountDesc : SortState.PurchaseCountAsc;

            products = sortOrder switch
            {
                SortState.TitleAsc => products.OrderBy(s => s.title),
                SortState.PriceAsc => products.OrderBy(s => s.price),
                SortState.PriceDesc => products.OrderByDescending(s => s.price),
                SortState.PurchaseCountAsc => products.OrderBy(s => s.purchases.Count),
                SortState.PurchaseCountDesc => products.OrderByDescending(s => s.purchases.Count),
                _ => products.OrderByDescending(s => s.title),
            };
            return View(await products.ToListAsync());
        }
        public async Task<IActionResult> PurchaseDashboard(DateTime? start, DateTime? end, SortState sortOrder = SortState.DateDesc)
        {
            IQueryable<Purchase> purchases = _db.Purchases.Include(p => p.Product).Include(u => u.User);
            ViewData["TitleSort"] = sortOrder == SortState.TitleAsc ? SortState.TitleDesc : SortState.TitleAsc;
            ViewData["PriceSort"] = sortOrder == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["DateSort"] = sortOrder == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            if (start != null && end != null)
            {
                purchases = sortOrder switch
                {
                    SortState.DateAsc => purchases.Where(s=>s.CreatedAt<=end&&s.CreatedAt>=start).OrderBy(s => s.CreatedAt),
                    SortState.NameAsc => purchases.Where(s => s.CreatedAt <= end && s.CreatedAt >= start).OrderBy(s => s.User.UserName),
                    SortState.NameDesc => purchases.Where(s => s.CreatedAt <= end && s.CreatedAt >= start).OrderByDescending(s => s.User.UserName),
                    SortState.TitleAsc => purchases.Where(s => s.CreatedAt <= end && s.CreatedAt >= start).OrderBy(s => s.Product.title),
                    SortState.TitleDesc => purchases.Where(s => s.CreatedAt <= end && s.CreatedAt >= start).OrderByDescending(s => s.Product.title),
                    SortState.PriceAsc => purchases.Where(s => s.CreatedAt <= end && s.CreatedAt >= start).OrderBy(s => s.Product.price),
                    SortState.PriceDesc => purchases.Where(s => s.CreatedAt <= end && s.CreatedAt >= start).OrderByDescending(s => s.Product.price),
                    _ => purchases.Where(s => s.CreatedAt <= end && s.CreatedAt >= start).OrderByDescending(s => s.CreatedAt),
                };
            }
            else
            {
                purchases = sortOrder switch
                {
                    SortState.DateAsc => purchases.OrderBy(s => s.CreatedAt),
                    SortState.NameAsc => purchases.OrderBy(s => s.User.UserName),
                    SortState.NameDesc => purchases.OrderByDescending(s => s.User.UserName),
                    SortState.TitleAsc => purchases.OrderBy(s => s.Product.title),
                    SortState.TitleDesc => purchases.OrderByDescending(s => s.Product.title),
                    SortState.PriceAsc => purchases.OrderBy(s => s.Product.price),
                    SortState.PriceDesc => purchases.OrderByDescending(s => s.Product.price),
                    _ => purchases.OrderByDescending(s => s.CreatedAt),
                };
            }
            return View(await purchases.ToListAsync());
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
