using DinSko.Models;
using DinSko.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DinSko.Pages.Account
{
    public class RegisterModel : PageModel
    {
        //Dependency Injection
        private readonly UserService _userService; // store UserService

        public RegisterModel(UserService userService)
        {
            _userService = userService; // get UserService
        }

        [BindProperty]
        public User User { get; set; } // bind the form data to the User object

        public void OnGet()
        {
            // display the registration page
        }

        public IActionResult OnPost()
        {
            // check if the submitted form data is valid
            if (!ModelState.IsValid)
            {
                return Page(); // return to the registration page if the data is not valid
            }

            _userService.Add(User); // send the user data to the UserService to create the account

            return RedirectToPage("/Index"); // redirect to the home page after successful registration
        }
    }
}