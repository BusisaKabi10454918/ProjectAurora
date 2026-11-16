using System.ComponentModel.DataAnnotations;

namespace project_aurora.Models;

public class AdminCode
{
    public AdminCode()
    {
    }

    public AdminCode(string newAdminCode, DateTime createdTime, string creator)
    {
        GenerationDate = createdTime;
        CodeHash = BCrypt.Net.BCrypt.HashPassword(newAdminCode);
        CreatorName = creator;
    }

    [Key]
    public Guid CodeID { get; set; } = Guid.NewGuid();

    [Required]
    public string CodeHash { get; set; } = string.Empty;

    [Required]
    public DateTime GenerationDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiryDate { get; set; }

    [Required]
    public string CreatorName { get; set; } = string.Empty;

    [Required]
    public bool NewAdminVerified { get; set; } = false;

    public Guid? UsedByUserID { get; set; }

    public DateTime? UsedAt { get; set; }
}

