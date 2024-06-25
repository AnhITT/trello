using Microsoft.AspNetCore.Http;

namespace BusinessLogic_Layer.Entity
{
    public class ApiFileUpload
    {
        public List<IFormFile> Files { get; set; }
        public Guid TaskId { get; set; }
    }
}
