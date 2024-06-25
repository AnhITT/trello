
namespace BusinessLogic_Layer.Entity
{
    public class ApiWorkflow
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        // n-1 Workflows - Board
        public Guid BoardId { get; set; }
    }
}
