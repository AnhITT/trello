using Microsoft.EntityFrameworkCore;
using DataAccess_Layer.Models;

namespace DataAccess_Layer.Data
{
    public class UploadDbContext : BaseContext
    {
        public UploadDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AttachmentFile> AttachmentFiles { get; set; }
    }
}
