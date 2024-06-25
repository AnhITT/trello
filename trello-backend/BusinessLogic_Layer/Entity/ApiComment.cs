using DataAccess_Layer.Models;

namespace BusinessLogic_Layer.Entity
{
    public class ApiComment
    {
        public Guid? Id { get; set; }
        public string Description { get; set; }

        // n-1  Comment-Task Card
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public TaskCard? TaskCards { get; set; }
        public User? Users { get; set; }
        public Comment? ParentComment { get; set; }
        public List<ApiComment>? Replies { get; set; }
    }
}
