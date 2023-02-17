using System.ComponentModel.DataAnnotations;

namespace ComplainBox.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
