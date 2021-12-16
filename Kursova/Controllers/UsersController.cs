using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Kursova.Models;
using Kursova.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Kursova.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new EditUserViewModel
            { Id = user.Id, Email = user.Email, UserName = user.UserName };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    if (model.NewPassword == null)
                    {
                        model.NewPassword = model.OldPassword;
                    }

                    IdentityResult password =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (password.Succeeded)
                    {
                        user.Email = model.Email;
                        user.UserName = model.UserName;

                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
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

                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}