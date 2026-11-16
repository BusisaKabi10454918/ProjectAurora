using System.ComponentModel.DataAnnotations;

namespace project_aurora.Models;

public class Challenge
{
    public Challenge()
    {

    }

    public Challenge(string title, string description, string answer)
    {
        Title = title;
        Description = description;
        Answer = answer;
    }
    public Guid ID { get; set; } = Guid.NewGuid();

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Answer { get; set; } = string.Empty;
}
