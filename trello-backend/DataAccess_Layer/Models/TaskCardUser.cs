
namespace DataAccess_Layer.Models
{
    public class TaskCardUser : TrackableEntry
    {
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }

        public TaskCard TaskCards { get; set; }
        public User Users { get; set; }
    }
}
