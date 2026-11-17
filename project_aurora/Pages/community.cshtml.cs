using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using project_aurora.Data;
using project_aurora.Models;

namespace project_aurora.Pages
{
    public class communityModel : PageModel
    {
        private readonly AuroraDbContext _context;

        public List<CommunityPost> Posts { get; set; } = new();

        [BindProperty]
        public string NewPostTitle { get; set; } = string.Empty;

        [BindProperty]
        public string NewPostDescription { get; set; } = string.Empty;

        public communityModel(AuroraDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGetCheckLogin()
        {
            string? userID = HttpContext.Session.GetString("User");

            if (!Guid.TryParse(userID, out Guid id))
            {
                return new JsonResult(new { loggedIn = false });
            }

            bool userExists = _context.Users.Any(u => u.Id == id);

            return new JsonResult(new { loggedIn = userExists });
        }

        public void OnGet()
        {
            LoadPosts();
        }

        private void LoadPosts()
        {
            Posts = _context.CommunityPosts
                .Include(p => p.Poster)
                .OrderByDescending(p => p.ID)
                .ToList();
        }

        public IActionResult OnPostCreatePost()
        {
            //Check here who is logged in, if they arent logged in make them log in first
            string? userID = HttpContext.Session.GetString("User");

            if (!Guid.TryParse(userID, out Guid id))
            {
                return RedirectToPage("/Login");
            }

            //Since we have a login apparently, find their user id, and use that to find their details
            User? user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                HttpContext.Session.Remove("User");
                return RedirectToPage("/Login");
            }

            //Check that the user actually submitted something
            string title = NewPostTitle.Trim();
            string description = NewPostDescription.Trim();

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(description))
            {
                ModelState.AddModelError("", "Both fields are required.");
                LoadPosts();
                return Page();
            }

            //Ensure the title isnt too long
            if (title.Length > 150)
            {
                ModelState.AddModelError("", "Your title cannot exceed 150 characters.");
                LoadPosts();
                return Page();
            }

            //Passes gone through, create the post
            CommunityPost newPost = new CommunityPost
            {
                ID = Guid.NewGuid(),
                Title = title,
                Description = description,
                PosterId = user.Id,
                Answers = 0,
                Votes = 0,
                Views = 0
            };

            
            _context.CommunityPosts.Add(newPost);
            _context.SaveChanges();

            
            return RedirectToPage("/Community");
        }

        public IActionResult OnPostVote(Guid postID)
        {
            string? userID = HttpContext.Session.GetString("User");

            if (!Guid.TryParse(userID, out Guid id))
                return RedirectToPage("/Login");

            CommunityPost? post = _context.CommunityPosts
                .FirstOrDefault(p => p.ID == postID);

            if (post == null)
                return RedirectToPage("/Community");

            post.Votes++;

            _context.SaveChanges();

            return RedirectToPage("/Community");
        }

        public IActionResult OnGetViewPost(Guid postID)
        {
            CommunityPost? post = _context.CommunityPosts
                .FirstOrDefault(p => p.ID == postID);

            if (post == null)
                return RedirectToPage("/Community");

            post.Views++;

            _context.SaveChanges();

            return RedirectToPage("/Community");
        }
    }
}
