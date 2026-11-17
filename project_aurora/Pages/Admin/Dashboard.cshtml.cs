using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using project_aurora.Data;
using project_aurora.Models;

namespace project_aurora.Pages.Admin
{
    public class AdminModel : PageModel
    {
        private readonly AuroraDbContext _context;

        public AdminModel(AuroraDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public string Answer { get; set; } = string.Empty;

        [BindProperty]
        public List<string> AllowedLanguages { get; set; } = new();

        private bool IsAdminLoggedIn()
        {
            string? adminSession = HttpContext.Session.GetString("Admin");

            if (!Guid.TryParse(adminSession, out Guid adminID))
                return false;

            return _context.Admins.Any(a => a.AdminID == adminID);
        }

        public IActionResult OnGet()
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("/Login");

            return Page();
        }

        public IActionResult OnPostCreateChallenge()
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("/Login");

            Title = Title.Trim();
            Description = Description.Trim();
            Answer = Answer.Trim();

            string[] validLanguages =
            {
                "CSharp", "Java", "Bash", "Python"
            };

            AllowedLanguages = AllowedLanguages
                .Distinct()
                .ToList();

            if (string.IsNullOrWhiteSpace(Title) ||
                string.IsNullOrWhiteSpace(Description) ||
                string.IsNullOrWhiteSpace(Answer))
            {
                ModelState.AddModelError("", "All fields are required.");
            }

            if (AllowedLanguages.Count == 0 ||
                AllowedLanguages.Any(l => !validLanguages.Contains(l)))
            {
                ModelState.AddModelError(
                    "",
                    "Select at least one valid programming language.");
            }

            if (!ModelState.IsValid)
                return Page();

            Challenge newChallenge = new Challenge
            {
                Title = Title,
                Description = Description,
                Answer = Answer,
                AllowedLanguages = string.Join(",", AllowedLanguages)
            };

            _context.Challenges.Add(newChallenge);
            _context.SaveChanges();

            return RedirectToPage();
        }
    }
}