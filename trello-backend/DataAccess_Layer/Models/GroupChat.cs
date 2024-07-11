using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DataAccess_Layer.DTOs;

namespace DataAccess_Layer.Models
{
    public class GroupChat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("GroupId")]
        public string GroupId { get; set; }

        [BsonElement("Public")]
        public string Public { get; set; }

        [BsonElement("Host")]
        public string Host { get; set; }

        [BsonElement("Members")]
        public List<UserDTO> Members { get; set; }

        [BsonElement("Messages")]
        public List<Message> Messages { get; set; }
    }
}
