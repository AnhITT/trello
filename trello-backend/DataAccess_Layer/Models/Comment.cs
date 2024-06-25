using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess_Layer.Models
{
    public class Comment : TrackableEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Description { get; set; }

        // n-1  Comment-Task Card
        public Guid TaskId { get; set; }
        public TaskCard TaskCards { get; set; }
        public Guid UserId { get; set; }
        public User Users { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }
    }
}
