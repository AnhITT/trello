using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess_Layer.Models
{
    public class User : TrackableEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
        [JsonIgnore]
        public bool VerifyEmail { get; set; }
        public EmailVerificationToken? EmailVerificationTokens { get; set; }
        // n-n User-Workspace
        public ICollection<WorkspaceUser> WorkspaceUsers { get; set; }
        // n-n User-TaskCard
        public ICollection<TaskCardUser> TaskCardUsers { get; set; }
    }
}
