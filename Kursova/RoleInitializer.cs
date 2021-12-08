using Kursova.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Kursova
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await userManager.FindByEmailAsync("admin@mail.com") == null)
            {
                User admin = new User { Email = "admin@mail.com", UserName = "admin" };
                IdentityResult result = await userManager.CreateAsync(admin, "Admin1!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}