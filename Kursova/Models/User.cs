using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Kursova.Models
{
    public class User : IdentityUser
    {
        public List<Purchase> purchases { get; set; } = new List<Purchase>();
    }
}