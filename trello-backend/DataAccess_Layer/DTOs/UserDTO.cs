using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DataAccess_Layer.DTOs
{
    public class UserDTO
    {
        [BsonElement("Id")]
        public string Id { get; set; }
        [BsonElement("FirstName")]
        public string FirstName { get; set; }
        [BsonElement("LastName")]
        public string LastName { get; set; }
    }
}
