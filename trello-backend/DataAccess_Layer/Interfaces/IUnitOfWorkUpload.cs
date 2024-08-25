using DataAccess_Layer.Models;
using DataAccess_Layer.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess_Layer.Interfaces
{
    public interface IUnitOfWorkUpload : IDisposable
    {
        IGenericRepository<AttachmentFile> AttachmentFileRepository { get; }
        void SaveChanges();
        void Commit();
        void RollBack();
        IDbContextTransaction BeginTransaction();
    }
}
