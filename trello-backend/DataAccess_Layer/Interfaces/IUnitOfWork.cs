using DataAccess_Layer.Models;
using DataAccess_Layer.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess_Layer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        GenericRepository<User> UserRepository { get; }
        GenericRepository<EmailVerificationToken> EmailVerificationTokenRepository { get; }
        GenericRepository<OTPResetPassword> OTPResetPasswordRepository { get; }
        GenericRepository<Workspace> WorkspaceRepository { get; }
        GenericRepository<Board> BoardRepository { get; }
        GenericRepository<Workflow> WorkflowRepository { get; }
        GenericRepository<TaskCard> TaskCardRepository { get; }
        GenericRepository<WorkspaceUser> WorkspaceUserRepository { get; }
        GenericRepository<TaskCardUser> TaskCardUserRepository { get; }
        GenericRepository<Comment> CommentRepository { get; }
        GenericRepository<CheckListItem> CheckListItemRepository { get; }
        GenericRepository<CheckList> CheckListRepository { get; }

        void SaveChanges();
        void Commit();
        void RollBack();
        IDbContextTransaction BeginTransaction();
        bool SaveChangesBool();
    }
}
