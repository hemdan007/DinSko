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
        private readonly UserService _userService; // store UserService
        private readonly IMemoryCache _memoryCache; // store login attempt data

        public LoginModel(UserService userService, IMemoryCache memoryCache)
        {
            _userService = userService; // get UserService
            _memoryCache = memoryCache; // get memory cache
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // display the login page
        }

        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required.";
                return Page();
            }

            string attemptKey = "LoginAttempts_" + Email; // key for failed login attempts
            string lockKey = "LoginLock_" + Email; // key for login lock

            if (_memoryCache.TryGetValue(lockKey, out bool isLocked) && isLocked)
            {
                ErrorMessage = "Too many failed login attempts. Try again later.";
                return Page();
            }

            User user = _userService.GetByEmail(Email); // find the user by email

            if (user == null)
            {
                AddFailedAttempt(attemptKey, lockKey); // count the failed login attempt
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            bool passwordIsValid = Argon2.Verify(user.PasswordHash, Password); // check if the password matches the stored hash

            if (!passwordIsValid)
            {
                AddFailedAttempt(attemptKey, lockKey); // count the failed login attempt
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            _memoryCache.Remove(attemptKey); // remove failed attempts after successful login

            var claims = new List<Claim> // create claims with information about the logged-in user
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // store the user's ID
                new Claim(ClaimTypes.Name, user.Email), // store the user's email
                new Claim(ClaimTypes.Role, user.Role.ToString()) // store the user's role
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme); // create an identity for the user

            var principal = new ClaimsPrincipal(identity); // create the authenticated user

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal); // create the authentication cookie

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

            _memoryCache.Set(
                attemptKey,
                attempts,
                TimeSpan.FromMinutes(5)); // store the number of failed attempts for 5 minutes

            if (attempts >= 5)
            {
                _memoryCache.Set(
                    lockKey,
                    true,
                    TimeSpan.FromMinutes(5)); // lock login for 5 minutes after 5 failed attempts
            }
        }
    }
}