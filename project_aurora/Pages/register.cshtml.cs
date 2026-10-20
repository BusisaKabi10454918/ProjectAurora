using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using project_aurora.Data;
using project_aurora.Models;

namespace project_aurora.Pages;

public class registerModel : PageModel
{
    private readonly AuroraDbContext _context;
    
    public registerModel(AuroraDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var firstName = Request.Form["first-name"].ToString();
        var lastName = Request.Form["last-name"].ToString();
        var username = Request.Form["username"].ToString();
        var password = Request.Form["password"].ToString();
        var confirmPassword = Request.Form["confirm-password"].ToString();
        var email = Request.Form["email"].ToString();
        var adminCode = Request.Form["admin-code"].ToString();

        //Setup a new user object, and prepare the checks, after all the checks are done, we will save the user to the database.
        var user = new User();

        //These fields shouldnt be empty, they are required fields, if they are empty we will return an error message to the user.
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(username) 
            || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(email))
        {
            ModelState.AddModelError(string.Empty, "All fields are required.");
            return Page();
        }

        //Verify the username is unique
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            ModelState.AddModelError(string.Empty, "Username already exists.");
            return Page();
        }

        //Verify the email is unique
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError(string.Empty, "Email already exists.");
            return Page();
        }

        //Verify that the password and confirm password fields match
        if (password != confirmPassword)
        {
            ModelState.AddModelError(string.Empty, "Passwords do not match.");
            return Page();
        }

        //If there is an admin code in the field verify it is correct, if it is correct set the isAdmin field to true
        if (!string.IsNullOrEmpty(adminCode))
        {

            //It isnt empty so verify it matches the admin code.
            string path = Path.Combine(Directory.GetCurrentDirectory(), "a-c.txt");
            string hashedCode = System.IO.File.ReadAllText(path).Trim();
            if (BCrypt.Net.BCrypt.Verify(adminCode, hashedCode))
            {
                user.isAdmin = true;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid admin code.");
                return Page();
            }
        }

        //If all checks pass, we will hash the password and save the user to the database.
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Username = username;
        user.Email = email;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        user.CreatedAt = DateTime.Today;

        _context.Users.Add(user);
        Console.WriteLine($"User {username} registered successfully.");
        await _context.SaveChangesAsync();
        return RedirectToPage("/login");
    }

}
