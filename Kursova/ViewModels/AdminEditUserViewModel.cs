using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kursova.ViewModels
{
    public class AdminEditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
