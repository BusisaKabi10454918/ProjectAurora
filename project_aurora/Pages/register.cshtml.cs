using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using project_aurora.Data;
using project_aurora.Models;

namespace project_aurora.Pages;

public class registerModel : PageModel
{
    public string validationResult { get; set; } = string.Empty;

    private readonly AuroraDbContext _context;
    private readonly VerificationServices _verificationServices;

    public registerModel(AuroraDbContext context, VerificationServices verifyer)
    {
        _context = context;
        _verificationServices = verifyer;
    }

    [BindProperty]
    public string FirstName { get; set; } = "";

    [BindProperty]
    public string LastName { get; set; } = "";

    [BindProperty]
    public string Email { get; set; } = "";

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    [BindProperty]
    public string ConfirmPassword { get; set; } = "";

    [BindProperty]
    public string AdminCode { get; set; } = "";

    public void OnPost()
    {
        string firstName = FirstName;
        string lastName = LastName;
        string username = Username;
        string email = Email;
        string password = Password;
        string confirmPassword = ConfirmPassword;
        string adminCode = AdminCode;
        bool adminStatus = false;



        string check_A = _verificationServices.passwordStrength(password);

        //Password and confirm password must match
        if (password != confirmPassword)
        {
            validationResult = "VER-REG-1004: PASSWORDS DO NOT MATCH";
            return;
        }

        //Check if the password is strong enough and if the credentials are valid
        if (check_A != "VER-PWD-0000")
        {
            validationResult = check_A;
            return;
        }

        string check_B = _verificationServices.checkCredentials(firstName, lastName, username, email);

        //Check if the credentials are valid
        if (check_B != "VER-REG-0000: REGISTRATION SUCCESSFUL")
        {
            validationResult = check_B;
            return;
        }

        //If all checks pass, create a new user and save to the database
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        //Check on their admin status
        adminStatus = _verificationServices.VerifyAdminCode(adminCode);

        User newUser = new User(firstName, lastName, username, email, passwordHash, adminStatus);


        if (!String.IsNullOrWhiteSpace(adminCode))
        {
            if (adminStatus)
            {
                try
                {
                    using var newAdminTransaction =
                        _context.Database.BeginTransaction();

                    _context.Users.Add(newUser);
                    _context.SaveChanges();

                    AppAdmin newAdmin = new AppAdmin(newUser.Id);
                    _context.Admins.Add(newAdmin);

                    var codes = _context.AdminCodes
                    .Where(c => !c.NewAdminVerified && c.ExpiryDate > DateTime.UtcNow)
                    .ToList();

                    AdminCode? ad = codes.FirstOrDefault(
                        c => BCrypt.Net.BCrypt.Verify(adminCode, c.CodeHash)
                    );

                    if (ad == null)
                    {
                        validationResult = "VER-REG-1005: INVALID OR EXPIRED ADMIN CODE";
                        return;
                    }

                    ad.NewAdminVerified = true;
                    ad.UsedAt = DateTime.UtcNow;
                    ad.UsedByUserID = newUser.Id;

                    _context.SaveChanges();
                    newAdminTransaction.Commit();

                    validationResult = "VER-REG-0000: REGISTRATION SUCCESSFUL";
                    return;
                }
                catch
                {
                    validationResult = "VER-REG-1006: REGISTRATION FAILURE";
                    return;
                }
            }
            else
            {
                validationResult = "VER-REG-1005: INVALID OR EXPIRED ADMIN CODE";
                return;
            }
        }
        else
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();

                _context.Users.Add(newUser);
                _context.SaveChanges();

                transaction.Commit();

                validationResult = "VER-REG-0000: REGISTRATION SUCCESSFUL";
                return;
            }
            catch
            {
                validationResult = "VER-REG-1006: REGISTRATION FAILURE";
                return;
            }
        }
    }
}
