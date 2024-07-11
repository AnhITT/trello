using DataAccess_Layer.Models;
using DataAccess_Layer.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess_Layer.Interfaces
{
    public interface IUnitOfWorkChat
    {
        IMongoRepository<Message> MessageRepository { get; }
        IMongoRepository<GroupChat> GroupChatRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
