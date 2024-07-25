using DataAccess_Layer.Utilities;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;

namespace DataAccess_Layer.Repositories
{
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;
        private readonly IHttpContextAccessor _contextAccessor;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _contextAccessor = new HttpContextAccessor();
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<T> GetByIdAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
        public async Task<IEnumerable<T>> Find(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }
        public async Task AddAsync(T entity)
        {
            var userId = GetUserIdCurrent();
            if (entity is TrackableEntryMongo trackableEntity)
            {
                trackableEntity.CreatedDate = DateTime.Now;
                trackableEntity.CreatedBy = userId;
                trackableEntity.IsDeleted = false;
            }
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(ObjectId id, T entity)
        {
            var userId = GetUserIdCurrent();
            if (entity is TrackableEntryMongo trackableEntity)
            {
                trackableEntity.ModifiedDate = DateTime.Now;
                trackableEntity.ModifiedBy = userId;
            }
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(ObjectId id, T entity)
        {
            var userId = GetUserIdCurrent();
            if (entity is TrackableEntryMongo trackableEntity)
            {
                trackableEntity.DeletedDate = DateTime.Now;
                trackableEntity.DeletedBy = userId;
                trackableEntity.IsDeleted = true;
            }
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }
        #region Get UserCurrent
        public string GetAccessToken()
        {
            var context = _contextAccessor.HttpContext;

            if (context != null && context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return authHeader.Substring("Bearer ".Length).Trim();
                }
            }

            return null;
        }
        public string GetUserIdCurrent()
        {
            var jwt = GetAccessToken();
            if (jwt == null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var userIdClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Id");

            return userIdClaim?.Value;
        }
        #endregion
    }
}