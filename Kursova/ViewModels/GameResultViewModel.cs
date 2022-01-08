using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kursova.Models;
using System.ComponentModel.DataAnnotations;
namespace Kursova.ViewModels
{
    public class GameResultViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string GameTitle { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public uint score { get; set; }
        [Required]
        public uint number { get; set; }
    }
}
