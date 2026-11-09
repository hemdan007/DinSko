
using DinSko.Models;
using DinSko.Services;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace DinSko.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly UserService _userService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            UserService userService,
            IMemoryCache memoryCache,
            ILogger<LoginModel> logger)
        {
            _userService = userService;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Display the login page
        }

        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required.";
                return Page();
            }

            string attemptKey = "LoginAttempts_" + Email;
            string lockKey = "LoginLock_" + Email;

            // Check if the account is temporarily locked
            if (_memoryCache.TryGetValue(lockKey, out bool isLocked) && isLocked)
            {
                _logger.LogWarning("Login blocked because the account is temporarily locked.");

                ErrorMessage = "Too many failed login attempts. Try again later.";
                return Page();
            }

            // Find the user by email
            User user = _userService.GetByEmail(Email);

            if (user == null)
            {
                AddFailedAttempt(attemptKey, lockKey);

                // Log failed login without storing the email or password
                _logger.LogWarning("Login failed: Invalid email or password.");

                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            // Verify the entered password against the stored password hash
            bool passwordIsValid = Argon2.Verify(user.PasswordHash, Password);

            if (!passwordIsValid)
            {
                AddFailedAttempt(attemptKey, lockKey);

                // Log failed login without storing the email or password
                _logger.LogWarning("Login failed: Invalid email or password.");

                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            // Remove failed attempts after successful login
            _memoryCache.Remove(attemptKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            // Log successful login using the user's ID
            _logger.LogInformation(
                "User {UserId} logged in successfully.",
                user.UserId);

            return RedirectToPage("/Index");
        }

        private void AddFailedAttempt(string attemptKey, string lockKey)
        {
            int attempts = 0;

            if (_memoryCache.TryGetValue(attemptKey, out int currentAttempts))
            {
                attempts = currentAttempts;
            }

            attempts++;

            // Store the number of failed attempts for 5 minutes
            _memoryCache.Set(
                attemptKey,
                attempts,
                TimeSpan.FromMinutes(5));

            if (attempts >= 5)
            {
                // Lock login for 5 minutes after 5 failed attempts
                _memoryCache.Set(
                    lockKey,
                    true,
                    TimeSpan.FromMinutes(5));

                // Log account lockout
                _logger.LogWarning(
                    "Account temporarily locked after {Attempts} failed login attempts.",
                    attempts);
            }
        }
    }
}
