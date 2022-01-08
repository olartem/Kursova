using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Kursova.Models
{
    public class User : IdentityUser
    {
        public List<GameResult> Results { get; set; } = new List<GameResult>();
    }
}