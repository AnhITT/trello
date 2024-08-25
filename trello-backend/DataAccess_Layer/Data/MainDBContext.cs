using Microsoft.EntityFrameworkCore;
using DataAccess_Layer.Models;
using Microsoft.AspNetCore.Http;

namespace DataAccess_Layer.Data
{
    public class MainDBContext : BaseContext
    {
        public MainDBContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
            : base(options, httpContextAccessor)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
        public DbSet<OTPResetPassword> OTPResetPasswords { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<TaskCard> TaskCards { get; set; }
        public DbSet<WorkspaceUser> WorkspaceUsers { get; set; }
        public DbSet<TaskCardUser> TaskCardUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CheckListItem> CheckListItems { get; set; }
        public DbSet<CheckList> CheckLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskCardUser>()
                .HasKey(tu => new { tu.UserId, tu.TaskId });

            modelBuilder.Entity<TaskCardUser>()
                .HasOne(tu => tu.Users)
                .WithMany(tc => tc.TaskCardUsers)
                .HasForeignKey(tu => tu.UserId);

            modelBuilder.Entity<TaskCardUser>()
                .HasOne(tu => tu.TaskCards)
                .WithMany(u => u.TaskCardUsers)
                .HasForeignKey(tu => tu.TaskId);

            modelBuilder.Entity<WorkspaceUser>()
                .HasKey(tu => new { tu.UserId, tu.WorkspaceId });

            modelBuilder.Entity<WorkspaceUser>()
                .HasOne(tu => tu.Users)
                .WithMany(tc => tc.WorkspaceUsers)
                .HasForeignKey(tu => tu.UserId);

            modelBuilder.Entity<WorkspaceUser>()
                .HasOne(tu => tu.Workspaces)
                .WithMany(u => u.WorkspaceUsers)
                .HasForeignKey(tu => tu.WorkspaceId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
