using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using project_aurora.Data;

namespace project_aurora.Pages
{
    

    public class loginModel : PageModel
    {
        private readonly AuroraDbContext _context;

        public loginModel(AuroraDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var credential = Request.Form["credential"].ToString();
            var password = Request.Form["password"].ToString();

            //Check if the credential is in the system. Fields are required so unlikely to be empty, but we will check anyway.
            if (string.IsNullOrEmpty(credential) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "All fields are required.");
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == credential || u.Email == credential);
            //Check if the credential is in the system.
            if (user != null)
            {
                //credential is in the system, check the password matches
                if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    //Password is correct, login successful
                    return RedirectToPage("/Users/logged_Index");
                }
                ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
                return Page();
            }

            ModelState.AddModelError(string.Empty, "400: Bad Request");
            return Page();
        }
    }
}
