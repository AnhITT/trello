using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess_Layer.Models
{
    public class CheckList : TrackableEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Title { get; set; }

        // n-1 CheckListItem - Task Card
        public Guid TaskId { get; set; }
        public TaskCard TaskCards { get; set; }
    }
}
