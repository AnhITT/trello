using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess_Layer.Models
{
    public class TaskCard : TrackableEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Cover { get; set; }
        public int Position { get; set; }

        // n-1 Task Card - Workflow
        public Guid WorkflowId { get; set; }
        public Workflow Workflows { get; set; }
        // n-n Task Card - User
        public ICollection<TaskCardUser> TaskCardUsers { get; set; }
    }
}
