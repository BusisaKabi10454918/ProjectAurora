using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using project_aurora.Data;
using project_aurora.Models;
using SQLitePCL;

namespace project_aurora.Pages.Users
{
    public class ProfileModel : PageModel
    {
        public string errorMessage = string.Empty;

        private readonly AuroraDbContext _context;

        public User? userProfile { get; set; }
        public List<ChallengeSubmission> userSubmissions { get; set; } = new();

        public ProfileModel(AuroraDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string NewUsername { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            string? userID = HttpContext.Session.GetString("User");

            if (!Guid.TryParse(userID, out Guid id))
            {
                return RedirectToPage("/Login");
            }

            userProfile = _context.Users.FirstOrDefault(u => u.Id == id);

            if (userProfile == null)
            {
                errorMessage = "Unable to retrieve your profile.";
                return Page();
            }

            userSubmissions = _context.ChallengeSubmissions
                .Where(s => s.UserID == id)
                .ToList();

            return Page();
        }

        public IActionResult OnPostChangeUsername()
        {
            string? userID = HttpContext.Session.GetString("User");

            if (!Guid.TryParse(userID, out Guid id))
                return RedirectToPage("/Login");

            User? user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                errorMessage = "Unable to retrieve your account.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewUsername))
            {
                errorMessage = "Please enter a username.";
                return Page();
            }

            bool usernameExists = _context.Users.Any(
                u => u.Username == NewUsername && u.Id != id);

            if (usernameExists)
            {
                errorMessage = "That username is already taken.";
                return Page();
            }

            user.Username = NewUsername.Trim();

            _context.SaveChanges();

            return RedirectToPage();
        }
    }
}
