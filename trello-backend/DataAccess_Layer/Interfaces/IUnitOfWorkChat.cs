using DataAccess_Layer.Models;
using DataAccess_Layer.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess_Layer.Interfaces
{
    public interface IUnitOfWorkChat
    {
        IMongoRepository<Chat> ChatRepository { get; }
    }
}
