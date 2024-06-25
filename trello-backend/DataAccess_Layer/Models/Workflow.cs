using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess_Layer.Models
{
    public class Workflow : TrackableEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }

        // n-1 Workflows - Board
        public Guid BoardId { get; set; }
        public Board Boards { get; set; }
    }
}
