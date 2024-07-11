using DataAccess_Layer.DTOs;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using DataAccess_Layer.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DataAccess_Layer.UnitOfWorks
{
    public class UnitOfWorkChat : IUnitOfWorkChat
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDBSetting _settings;
        private IMongoRepository<Message> _messages;
        private IMongoRepository<GroupChat> _groupChats;

        public UnitOfWorkChat(IOptions<MongoDBSetting> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);
        }

        public IMongoRepository<Message> MessageRepository => _messages ??= new MongoRepository<Message>(_database, "Messages");
        public IMongoRepository<GroupChat> GroupChatRepository => _groupChats ??= new MongoRepository<GroupChat>(_database, "GroupChats");

        public async Task<int> SaveChangesAsync()
        {
            return await Task.FromResult(0);
        }

        public void Dispose()
        {
        }
    }
}