using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_aurora.Models;

public class CommunityPost
{
    public Guid ID { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public Guid PosterId { get; set; }

    [ForeignKey(nameof(PosterId))]
    public User? Poster { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public int Answers { get; set; } = 0;

    public int Votes { get; set; } = 0;

    public int Views { get; set; } = 0;
}