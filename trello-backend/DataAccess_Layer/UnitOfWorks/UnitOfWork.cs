using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Http;
using DataAccess_Layer.Data;
using DataAccess_Layer.Repositories;

namespace DataAccess_Layer.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MainDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDbContextTransaction _transaction;

        private IGenericRepository<User> userRepository;
        private IGenericRepository<EmailVerificationToken> emailRepository;
        private IGenericRepository<OTPResetPassword> otpRepository;
        private IGenericRepository<Workspace> workspaceRepository;
        private IGenericRepository<Board> boardRepository;
        private IGenericRepository<Workflow> workflowRepository;
        private IGenericRepository<TaskCard> taskCardRepository;
        private IGenericRepository<WorkspaceUser> workspaceUserRepository;
        private IGenericRepository<TaskCardUser> taskCardUserRepository;
        private IGenericRepository<Comment> commentRepository;
        private IGenericRepository<CheckListItem> checkListItemRepository;
        private IGenericRepository<CheckList> checkListRepository;

        public UnitOfWork(MainDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IGenericRepository<User> UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<User>(_context, _httpContextAccessor);
                }
                return userRepository;
            }
        }

        public IGenericRepository<EmailVerificationToken> EmailVerificationTokenRepository
        {
            get
            {
                if (this.emailRepository == null)
                {
                    this.emailRepository = new GenericRepository<EmailVerificationToken>(_context, _httpContextAccessor);
                }
                return emailRepository;
            }
        }

        public IGenericRepository<OTPResetPassword> OTPResetPasswordRepository
        {
            get
            {
                if (this.otpRepository == null)
                {
                    this.otpRepository = new GenericRepository<OTPResetPassword>(_context, _httpContextAccessor);
                }
                return otpRepository;
            }
        }

        public IGenericRepository<Workspace> WorkspaceRepository
        {
            get
            {
                if (this.workspaceRepository == null)
                {
                    this.workspaceRepository = new GenericRepository<Workspace>(_context, _httpContextAccessor);
                }
                return workspaceRepository;
            }
        }

        public IGenericRepository<Board> BoardRepository
        {
            get
            {
                if (this.boardRepository == null)
                {
                    this.boardRepository = new GenericRepository<Board>(_context, _httpContextAccessor);
                }
                return boardRepository;
            }
        }

        public IGenericRepository<Workflow> WorkflowRepository
        {
            get
            {
                if (this.workflowRepository == null)
                {
                    this.workflowRepository = new GenericRepository<Workflow>(_context, _httpContextAccessor);
                }
                return workflowRepository;
            }
        }

        public IGenericRepository<TaskCard> TaskCardRepository
        {
            get
            {
                if (this.taskCardRepository == null)
                {
                    this.taskCardRepository = new GenericRepository<TaskCard>(_context, _httpContextAccessor);
                }
                return taskCardRepository;
            }
        }

        public IGenericRepository<WorkspaceUser> WorkspaceUserRepository
        {
            get
            {
                if (this.workspaceUserRepository == null)
                {
                    this.workspaceUserRepository = new GenericRepository<WorkspaceUser>(_context, _httpContextAccessor);
                }
                return workspaceUserRepository;
            }
        }

        public IGenericRepository<TaskCardUser> TaskCardUserRepository
        {
            get
            {
                if (this.taskCardUserRepository == null)
                {
                    this.taskCardUserRepository = new GenericRepository<TaskCardUser>(_context, _httpContextAccessor);
                }
                return taskCardUserRepository;
            }
        }

        public IGenericRepository<Comment> CommentRepository
        {
            get
            {
                if (this.commentRepository == null)
                {
                    this.commentRepository = new GenericRepository<Comment>(_context, _httpContextAccessor);
                }
                return commentRepository;
            }
        }

        public IGenericRepository<CheckListItem> CheckListItemRepository
        {
            get
            {
                if (this.checkListItemRepository == null)
                {
                    this.checkListItemRepository = new GenericRepository<CheckListItem>(_context, _httpContextAccessor);
                }
                return checkListItemRepository;
            }
        }

        public IGenericRepository<CheckList> CheckListRepository
        {
            get
            {
                if (this.checkListRepository == null)
                {
                    this.checkListRepository = new GenericRepository<CheckList>(_context, _httpContextAccessor);
                }
                return checkListRepository;
            }
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public void RollBack()
        {
            _transaction.Rollback();
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

        public IDbContextTransaction BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
            return _transaction;
        }
    }
}
