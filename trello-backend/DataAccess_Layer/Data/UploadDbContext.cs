using Microsoft.EntityFrameworkCore;
using DataAccess_Layer.Models;
using Microsoft.AspNetCore.Http;

namespace DataAccess_Layer.Data
{
    public class UploadDbContext : BaseContext
    {
        public UploadDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options, httpContextAccessor)
        {
        }
        public DbSet<AttachmentFile> AttachmentFiles { get; set; }
    }
}
