using DinSko.Services;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DinSko.Pages.Account
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        private readonly UserService _userService; // store UserService

        public ChangePasswordModel(UserService userService)
        {
            _userService = userService; // get UserService
        }

        [BindProperty]
        [Required]
        public string CurrentPassword { get; set; }

        [BindProperty]
        [Required]
        [StringLength(64, MinimumLength = 15,
            ErrorMessage = "Password must be between 15 and 64 characters.")]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required]
        public string ConfirmPassword { get; set; }

        public string ErrorMessage { get; set; }

        public string SuccessMessage { get; set; }

        public void OnGet()
        {
            // display the change password page
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // return to the page if the data is invalid
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "The new passwords do not match.";
                return Page();
            }

            string userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Challenge(); // require authentication if the user ID is missing
            }

            var user = _userService.GetById(userId); // get the current user from the database

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !Argon2.Verify(user.PasswordHash, CurrentPassword))
            {
                ErrorMessage = "Current password is incorrect.";
                return Page();
            }

            string passwordHash = Argon2.Hash(NewPassword); // hash the new password before storing it

            _userService.ChangePassword(userId, passwordHash); // update the password hash in the database
           
            TempData["SuccessMessage"] = "Password changed successfully.";

            return RedirectToPage("/Index"); // return to the home page

        }
    }
}