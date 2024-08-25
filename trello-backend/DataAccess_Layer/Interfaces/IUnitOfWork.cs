using DataAccess_Layer.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess_Layer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<EmailVerificationToken> EmailVerificationTokenRepository { get; }
        IGenericRepository<OTPResetPassword> OTPResetPasswordRepository { get; }
        IGenericRepository<Workspace> WorkspaceRepository { get; }
        IGenericRepository<Board> BoardRepository { get; }
        IGenericRepository<Workflow> WorkflowRepository { get; }
        IGenericRepository<TaskCard> TaskCardRepository { get; }
        IGenericRepository<WorkspaceUser> WorkspaceUserRepository { get; }
        IGenericRepository<TaskCardUser> TaskCardUserRepository { get; }
        IGenericRepository<Comment> CommentRepository { get; }
        IGenericRepository<CheckListItem> CheckListItemRepository { get; }
        IGenericRepository<CheckList> CheckListRepository { get; }

        void SaveChanges();
        void Commit();
        void RollBack();
        IDbContextTransaction BeginTransaction();
        bool SaveChangesBool();
    }
}
