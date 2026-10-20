using System.ComponentModel.DataAnnotations;

namespace project_aurora.Models;

public class Challenge
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Answer { get; set; } = string.Empty;
}
