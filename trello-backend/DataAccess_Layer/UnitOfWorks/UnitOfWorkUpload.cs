using DataAccess_Layer.Data;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using DataAccess_Layer.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess_Layer.UnitOfWorks
{
    public class UnitOfWorkUpload : IUnitOfWorkUpload
    {
        private UploadDbContext _context;
        private IDbContextTransaction _transaction;
        private GenericRepository<AttachmentFile> attachmentFileRepository;

        public UnitOfWorkUpload(UploadDbContext uploadDbContext)
        {
            _context = uploadDbContext;
        }

        public IDbContextTransaction BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
            return _transaction;
        }

        
        public GenericRepository<AttachmentFile> AttachmentFileRepository
        {
            get
            {
                if (this.attachmentFileRepository == null)
                {
                    this.attachmentFileRepository = new GenericRepository<AttachmentFile>(_context);
                }
                return attachmentFileRepository;
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
    }
}
