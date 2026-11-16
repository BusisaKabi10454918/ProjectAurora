using project_aurora.Data;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

public class VerificationServices
{

    public VerificationServices(AuroraDbContext context)
    {
        _context = context;
    }

    private readonly AuroraDbContext _context;

    public string checkCredentials(string firstname, string lastname, string username, string email)
    {
        //Checks will happen to verify the parameters aren't empty regardless we must check them
        if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(firstname) || String.IsNullOrWhiteSpace(lastname))
        {
            //Invalid Entry
            return "VER-REG-2001: INVALID ENTRY";
        }

        //Check if the first and last names are valid, only letters allowed
        if (!Regex.IsMatch(firstname, @"^[a-zA-Z]+$"))
        {
            return "VER-REG-2004: INVALID FIRST NAME";
        }

        if(!Regex.IsMatch(lastname, @"^[a-zA-Z]+$"))
        {
            return "VER-REG-2005: INVALID LAST NAME";
        }

        //Check if the email is valid, must contain @ and a domain
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return "VER-REG-2006: INVALID EMAIL FORMAT";
        }

        //No invalid entries start checks for credentials
        if (credSearch(username))
        {
            //Username already in use
            return "VER-REG-2002: USERNAME ALREADY IN USE";
        }

        if (credSearch(email))
        {
            //Email in use
            return "VER-REG-2003: EMAIL ALREADY REGISTERED";
        }

        //No issues, clean exit code
        return "VER-REG-0000: REGISTRATION SUCCESSFUL";
    }

    public static string GenCode()
    {
        const string characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        string codeSnippet = "AUR";

        for (int section = 0; section < 3; section++)
        {
            codeSnippet += "-";

            for (int character = 0; character < 4; character++)
            {
                int index = RandomNumberGenerator.GetInt32(characters.Length);
                codeSnippet += characters[index];
            }
        }
        return codeSnippet;
    }

    public bool VerifyAdminCode(string proposedCode)
    {
        if (string.IsNullOrWhiteSpace(proposedCode))
            return false;

        var codes = _context.AdminCodes
            .Where(ac =>
                !ac.NewAdminVerified &&
                ac.ExpiryDate > DateTime.UtcNow)
            .ToList();

        return codes.Any(ac =>
            BCrypt.Net.BCrypt.Verify(proposedCode, ac.CodeHash));
    }

    public string passwordStrength(string password)
    {
        //Measure length
        if (!Regex.IsMatch(password, @"^.{8,}$"))
        {
            //Below minimum
            return "VER-PWD-1001: PASSWORD TOO SHORT";
        }

        //At least 1 number
        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            //No Number
            return "VER-PWD-1002: PASSWORD MUST CONTAIN AT LEAST ONE NUMBER";
        }

        //At least 1 special character
        if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
        {
            //No special character
            return "VER-PWD-1003: PASSWORD MUST CONTAIN AT LEAST ONE SPECIAL CHARACTER";
        }

        //All check good, proceed
        return "VER-PWD-0000";
    }

    public string attemptLogin(string credential, string password)
    {
        //Check if credential exists
        var user = _context.Users.FirstOrDefault(u => u.Username == credential || u.Email == credential);

        //If user is null the credential doesn't exist or is not found
        if (user == null)
        {
            return "VER-LOG-0002:\nUsername/Email or Password is incorrect";
        }
        //Credential exists, so check if the password is the same. If not do not login.
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return "VER-LOG-0002:\nUsername/Email or Password is incorrect";
        }

        //Password matches so check if user is an admin. if not proceed to normal user login
        if (_context.Admins.Any(a => a.UserID == user.Id))
        {
            return $"VER-LOG-0001:\nLogin Successful.\nWelcome Admin: {user.FirstName} {user.LastName}";
        }

        return $"VER-LOG-0000: \nLogin Successful.\nWelcome Back: {user.Username}";
    }

    private bool credSearch(string credential)
    {
        //Check if the credential is an email, if so check if taken.
        if (credential.Contains("@"))
        {                
            return _context.Users.Any(u => u.Email == credential);        
        }

        return _context.Users.Any(u => u.Username == credential);
    }
}
