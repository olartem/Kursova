using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Braintree;
using Kursova.DAL;
using Kursova.Models;
using Kursova.Services;
using Kursova.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kursova.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationContext db;
        private readonly IBraintreeService _braintreeService;
        private readonly UserManager<User> _userManager;

        public ProductController(ApplicationContext context, IBraintreeService braintreeService, UserManager<User> userManager)
        {
            db = context;
            _braintreeService = braintreeService;
            _userManager = userManager;
        }
        [Authorize(Roles = "admin")]
        public IActionResult Create() => View();

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model)
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
                    Product product = new Product
                    { description = model.description, price = model.price, title = model.title, image = imageData };
                    db.Products.Add(product);
                }
                else
                {
                    Product product = new Product
                    { description = model.description, price = model.price, title = model.title, image = null };
                    db.Products.Add(product);
                }


                await db.SaveChangesAsync();
                return RedirectToAction("ProductDashboard", "Admin");
            }
            return View(model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {

            Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            if (product.image != null)
            {
                string imageBase64Data =
                    Convert.ToBase64String(product.image);
                string imageUrl =
                    string.Format("data:image/*;base64,{0}",
                        imageBase64Data);
                ViewBag.ImageUrl = imageUrl;
            }
            else
            {
                ViewBag.ImageUrl = "/noimage.jpg";
            }
            var gateway = _braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;
            DetailsProductViewModel model = new DetailsProductViewModel
            { title = product.title, description = product.description, price = product.price, Id = product.Id, Nonce = "" };
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string id)
        {
            Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            EditProductViewModel model = new EditProductViewModel
            { Id = product.Id, title = product.title, description = product.description, image = product.image, price = product.price };
            return View(model);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
                if (product != null)
                {
                    product.description = model.description;
                    product.title = model.title;
                    product.price = model.price;
                    db.Products.Update(product);
                    await db.SaveChangesAsync();
                    return RedirectToAction("ProductDashboard", "Admin");
                }
            }
            return View(model);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            Product product = await db.Products.Include(p => p.purchases).FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                db.Products.Remove(product);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("ProductDashboard", "Admin");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Buy(DetailsProductViewModel model)
        {
            var gateway = _braintreeService.GetGateway();
            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(model.price),
                PaymentMethodNonce = model.Nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var result = gateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                User user = await _userManager.FindByIdAsync(userId);
                Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
                Purchase purchase = new Purchase
                { Product = product, User = user, CreatedAt = DateTime.Now };
                db.Purchases.Add(purchase);
                await db.SaveChangesAsync();
                return RedirectToAction("UserPurchases", "Product");
            }
            else
            {
                return View("Details", model);
            }
        }
        public async Task<IActionResult> UserPurchases()
        {
            var purchases = await db.Purchases.Include(p => p.Product).ToListAsync();
            var userPurchases = from t in purchases
                                where t.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                                select t;
            return View(userPurchases.ToList());
        }

    }
}

