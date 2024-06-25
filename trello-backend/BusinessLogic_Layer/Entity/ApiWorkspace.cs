
namespace BusinessLogic_Layer.Entity
{
    public class ApiWorkspace
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        public List<Guid>? UserIds { get; set; }
    }
}
