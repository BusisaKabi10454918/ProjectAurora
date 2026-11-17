using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using project_aurora.Data;
using project_aurora.Models;

namespace project_aurora.Pages
{
    public class challengesModel : PageModel
    {
        private readonly AuroraDbContext _context;

        public List<Challenge> Challenges { get; set; } = new();

        public challengesModel(AuroraDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Challenges = _context.Challenges
                .AsNoTracking()
                .ToList();
        }
    }
}
