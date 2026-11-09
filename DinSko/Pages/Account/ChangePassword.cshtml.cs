
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
        private readonly UserService _userService;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserService userService,
            ILogger<ChangePasswordModel> logger)
        {
            _userService = userService;
            _logger = logger;
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
            // Display the change password page
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "The new passwords do not match.";
                return Page();
            }

            string userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Challenge();
            }

            // get the current user from the database
            var user = _userService.GetById(userId);

            if (user == null ||
                string.IsNullOrEmpty(user.PasswordHash) ||
                !Argon2.Verify(user.PasswordHash, CurrentPassword))
            {
                // log failed password change
                _logger.LogWarning(
                    "Password change failed for User ID {UserId}: Incorrect current password.",
                    userId);

                ErrorMessage = "Current password is incorrect.";
                return Page();
            }

            // hash the new password before storing it
            string passwordHash = Argon2.Hash(NewPassword);

            // update the password hash in the database
            _userService.ChangePassword(userId, passwordHash);

            // log successful password change
            _logger.LogInformation(
                "Password changed successfully for User ID {UserId}.",
                userId);

            TempData["SuccessMessage"] = "Password changed successfully.";

            return RedirectToPage("/Index");
        }
    }
}