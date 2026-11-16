using System.ComponentModel.DataAnnotations;

namespace project_aurora.Models;

public class User
{
    public User()
    {

    }

    public User(string firstname, string lastname, string username, string email, string passwordHash, bool adminStatus)
    {
        FirstName = firstname;
        LastName = lastname;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        isAdmin = adminStatus;
    }

    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool isAdmin { get; set; } = false;

}
