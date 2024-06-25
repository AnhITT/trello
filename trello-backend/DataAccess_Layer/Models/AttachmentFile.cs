using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess_Layer.Models
{
    public class AttachmentFile : TrackableEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FileNameToken { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Guid TaskId { get; set; }
    }
}
