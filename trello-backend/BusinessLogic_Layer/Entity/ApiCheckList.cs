
namespace BusinessLogic_Layer.Entity
{
    public class ApiCheckList
    {
        public string Title { get; set; }

        // n-1 CheckListItem - Task Card
        public Guid TaskId { get; set; }
    }
}
