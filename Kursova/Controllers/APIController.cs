
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kursova.Models;
using Kursova.ViewModels;
using Kursova.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Kursova.Controllers
{
    [Route("api")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationContext db;

        public APIController(UserManager<User> userManager, IConfiguration configuration, ApplicationContext db)
        {
            this.userManager = userManager;
            _configuration = configuration;
            this.db = db;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddDays(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    user_id = user.Id
                });
            }
            return Unauthorized();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("getgames")]
        public async Task<ActionResult> GetGames()
        {
            var temp = await db.Games.Where(t => t.StartTime <= DateTime.Now).Where(t => t.EndTime >= DateTime.Now).ToListAsync();
            List<string> games = new List<string>();
            foreach(var item in temp)
            {
                string str = item.title + item.number;
                games.Add(str);
            }
            return Ok(games);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("postresult")]
        public async Task<ActionResult> PostGameResult(GameResultViewModel res)
        {
            if(ModelState.IsValid)
            {
                Game game = await db.Games.Where(t=>t.title==res.GameTitle).FirstOrDefaultAsync(t=>t.number==res.number);
                User user = await userManager.FindByIdAsync(res.UserId);
                if(user!=null&&game!=null&&DateTime.Compare(game.EndTime,res.EndTime)>0)
                {
                    GameResult UserResult = new GameResult { Game = game, score = res.score, User = user, UserEndTime = res.EndTime };
                    db.Results.Add(UserResult);
                    await db.SaveChangesAsync();
                    return Ok();
                }
            }
            return NotFound();
        }
    }
}
