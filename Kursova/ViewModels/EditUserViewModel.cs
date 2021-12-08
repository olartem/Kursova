using System.ComponentModel.DataAnnotations;

namespace Kursova.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
