using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kursova.ViewModels
{
    public class EditGameViewModel
    {
        public string Id { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public uint number { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartTime { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndTime { get; set; }
        public byte[] image { get; set; }
    }
}
