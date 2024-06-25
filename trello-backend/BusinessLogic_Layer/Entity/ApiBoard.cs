
namespace BusinessLogic_Layer.Entity
{
    public class ApiBoard
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid WorkspaceId { get; set; }
        public string? Type { get; set;}
    }
}
