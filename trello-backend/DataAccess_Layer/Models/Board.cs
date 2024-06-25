using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess_Layer.Models
{
    public class Board : TrackableEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid WorkspaceId { get; set; }
        [JsonIgnore]
        public Workspace Workspaces { get; set; }
    }
}
