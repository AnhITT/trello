using DataAccess_Layer.Data;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using DataAccess_Layer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess_Layer.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private MainDBContext _context;
        private IDbContextTransaction _transaction;
        private GenericRepository<User> userRepository;
        private GenericRepository<EmailVerificationToken> emailRepository;
        private GenericRepository<OTPResetPassword> otpRepository;
        private GenericRepository<Workspace> workspaceRepository;
        private GenericRepository<Board> boardRepository;
        private GenericRepository<Workflow> workflowRepository;
        private GenericRepository<TaskCard> taskCardRepository;
        private GenericRepository<WorkspaceUser> workspaceUserRepository;
        private GenericRepository<TaskCardUser> taskCardUserRepository;
        private GenericRepository<Comment> commentRepository;
        private GenericRepository<CheckListItem> checkListItemRepository;
        private GenericRepository<CheckList> checkListRepository;


        public UnitOfWork(MainDBContext context)
        {
            _context = context;
            userRepository = new GenericRepository<User>(context);
            emailRepository = new GenericRepository<EmailVerificationToken>(context);
            otpRepository = new GenericRepository<OTPResetPassword>(context);
        }

        public IDbContextTransaction BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
            return _transaction;
        }

        public GenericRepository<User> UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<User>(_context);
                }
                return userRepository;
            }
        }
        public GenericRepository<EmailVerificationToken> EmailVerificationTokenRepository
        {
            get
            {
                if (this.emailRepository == null)
                {
                    this.emailRepository = new GenericRepository<EmailVerificationToken>(_context);
                }
                return emailRepository;
            }
        }

        public GenericRepository<OTPResetPassword> OTPResetPasswordRepository
        {
            get
            {
                if (this.otpRepository == null)
                {
                    this.otpRepository = new GenericRepository<OTPResetPassword>(_context);
                }
                return otpRepository;
            }
        }

        public GenericRepository<Workspace> WorkspaceRepository
        {
            get
            {
                if (this.workspaceRepository == null)
                {
                    this.workspaceRepository = new GenericRepository<Workspace>(_context);
                }
                return workspaceRepository;
            }
        }

        public GenericRepository<Board> BoardRepository
        {
            get
            {
                if (this.boardRepository == null)
                {
                    this.boardRepository = new GenericRepository<Board>(_context);
                }
                return boardRepository;
            }
        }

        public GenericRepository<Workflow> WorkflowRepository
        {
            get
            {
                if (this.workflowRepository == null)
                {
                    this.workflowRepository = new GenericRepository<Workflow>(_context);
                }
                return workflowRepository;
            }
        }

        public GenericRepository<TaskCard> TaskCardRepository
        {
            get
            {
                if (this.taskCardRepository == null)
                {
                    this.taskCardRepository = new GenericRepository<TaskCard>(_context);
                }
                return taskCardRepository;
            }
        }

        public GenericRepository<WorkspaceUser> WorkspaceUserRepository
        {
            get
            {
                if (this.workspaceUserRepository == null)
                {
                    this.workspaceUserRepository = new GenericRepository<WorkspaceUser>(_context);
                }
                return workspaceUserRepository;
            }
        }

        public GenericRepository<TaskCardUser> TaskCardUserRepository
        {
            get
            {
                if (this.taskCardUserRepository == null)
                {
                    this.taskCardUserRepository = new GenericRepository<TaskCardUser>(_context);
                }
                return taskCardUserRepository;
            }
        }

        public GenericRepository<Comment> CommentRepository
        {
            get
            {
                if (this.commentRepository == null)
                {
                    this.commentRepository = new GenericRepository<Comment>(_context);
                }
                return commentRepository;
            }
        }

        public GenericRepository<CheckListItem> CheckListItemRepository
        {
            get
            {
                if (this.checkListItemRepository == null)
                {
                    this.checkListItemRepository = new GenericRepository<CheckListItem>(_context);
                }
                return checkListItemRepository;
            }
        }

        public GenericRepository<CheckList> CheckListRepository
        {
            get
            {
                if (this.checkListRepository == null)
                {
                    this.checkListRepository = new GenericRepository<CheckList>(_context);
                }
                return checkListRepository;
            }
        }
        public void Commit()
        {
            _transaction.Commit();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RollBack()
        {
            _transaction.Rollback();
            Dispose();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public bool SaveChangesBool()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}
