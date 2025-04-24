using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace UniMarket.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string? FullName { get; set; }  // Cho phép null
        public string? Address { get; set; }
        public string? Age { get; set; }
        [DefaultValue("User")] // Đặt giá trị mặc định

        public string Role { get; set; } = "User";
<<<<<<< HEAD
        
=======
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
    }
}
