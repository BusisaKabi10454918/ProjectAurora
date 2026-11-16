using System.ComponentModel.DataAnnotations;

namespace project_aurora.Models
{

    public class AppAdmin
    {
        public AppAdmin()
        {
        }

        public AppAdmin(Guid userID)
        {
            UserID = userID;
        }

        [Key]
        public Guid AdminID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserID { get; set; }

        public User? User { get; set; }
    }
}