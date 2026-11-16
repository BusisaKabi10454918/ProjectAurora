using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using project_aurora.Data;
using project_aurora.Models;


namespace project_aurora.Pages;

public class loginModel : PageModel
{
    public string validationResult { get; set; } = string.Empty;

    private readonly VerificationServices _verificationServices;

    private readonly AuroraDbContext _context;

    public loginModel(VerificationServices verifyer, AuroraDbContext context)
    {
        _verificationServices = verifyer;
        _context = context;
    }

    [BindProperty]
    public string Credential { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public IActionResult OnGet()
    {
        //Here we'll do a primary session check, if the session is set, move to the appropriate profile page.
        string? userSession = HttpContext.Session.GetString("User");
        string? adminSession = HttpContext.Session.GetString("Admin");

        if(userSession != null)
        {
            return RedirectToPage("/Users/Profile");
        }
        if(adminSession != null)
        {
            return RedirectToPage("/Admin/Dashboard");
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        string credential = Credential;
        string password = Password;

        if (string.IsNullOrWhiteSpace(credential) ||
            string.IsNullOrWhiteSpace(password))
        {
            validationResult =
                "VER-LOG-0005:\nPlease enter both email/username and password";

            return Page();
        }

        validationResult =
            _verificationServices.attemptLogin(credential, password);

        if (validationResult.Contains("VER-LOG-0000") ||
            validationResult.Contains("VER-LOG-0001"))
        {
            User? user = _context.Users.FirstOrDefault(
                u => u.Username == credential || u.Email == credential);

            if (user == null)
            {
                validationResult = "VER-LOG-0004:\nUnable to retrieve account.";
                return Page();
            }

            HttpContext.Session.Remove("User");
            HttpContext.Session.Remove("Admin");

            if (validationResult.Contains("VER-LOG-0001"))
            {
                AppAdmin? admin = _context.Admins.FirstOrDefault(
                    ad => ad.UserID == user.Id);

                if (admin == null)
                {
                    validationResult = "VER-LOG-0004:\nAdmin record not found.";
                    return Page();
                }

                HttpContext.Session.SetString("Admin", admin.AdminID.ToString());
            }
            else
            {
                HttpContext.Session.SetString("User", user.Id.ToString());
            }

            return RedirectToPage("/Index");
        }

        validationResult += "\nLogin failed.\nPlease check your credentials.";
        return Page();
    }

}
