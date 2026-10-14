using System.ComponentModel.DataAnnotations;

namespace DinSko.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public UserRole Role { get; set; }
        [Required]
        [StringLength(64, MinimumLength = 15,
        ErrorMessage = "Password must be between 15 and 64 characters.")]
        public string Password { get; set; } // user´s pass
        public string? PasswordHash { get; set; } // passHash in DB
    }
}