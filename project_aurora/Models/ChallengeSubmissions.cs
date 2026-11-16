using System.ComponentModel.DataAnnotations;

namespace project_aurora.Models
{
    public class ChallengeSubmission
    {
        public ChallengeSubmission()
        {
        }

        [Key]
        public Guid SubmissionID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserID { get; set; }

        [Required]
        public Guid ChallengeID { get; set; }

        [Required]
        public string SubmissionFile { get; set; } = string.Empty;

        [Required]
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
