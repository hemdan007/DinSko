using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DinSko.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPost()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme); // remove the authentication cookie

            return RedirectToPage("/Index"); // return to the home page
        }
    }
}