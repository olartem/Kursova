using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kursova.ViewModels
{
    public class CreateProductViewModel
    {
        [Required]
        public string title { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public int price { get; set; }
        public byte[] image { get; set; }
    }
}
