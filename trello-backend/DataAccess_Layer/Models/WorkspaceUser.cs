
namespace DataAccess_Layer.Models
{
    public class WorkspaceUser : TrackableEntry
    {
        public Guid WorkspaceId { get; set; }
        public Guid UserId { get; set; }
        public Workspace Workspaces { get; set; }
        public User Users { get; set; }
    }
}
