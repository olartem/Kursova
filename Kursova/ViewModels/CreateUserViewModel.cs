using System.ComponentModel.DataAnnotations;

namespace Kursova.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}