
namespace BusinessLogic_Layer.Entity
{
    public class ApiCheckListItem
    {
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        // n-1 CheckListItem - Task Card
        public Guid CheckListId { get; set; }
    }
}
