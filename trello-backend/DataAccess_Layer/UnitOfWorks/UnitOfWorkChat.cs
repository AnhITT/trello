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
        private IMongoRepository<Chat> _chats;

        public UnitOfWorkChat(IOptions<MongoDBSetting> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);
        }

        public IMongoRepository<Chat> ChatRepository => _chats ??= new MongoRepository<Chat>(_database, "Chats");

    }
}