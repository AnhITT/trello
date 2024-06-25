
namespace BusinessLogic_Layer.Entity
{
    public class ApiTaskCard
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public Guid WorkflowId { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
    }
}
