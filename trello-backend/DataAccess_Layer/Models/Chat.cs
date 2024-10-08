﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DataAccess_Layer.DTOs;

namespace DataAccess_Layer.Models
{
    public class Chat : TrackableEntryMongo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("NameGroup")]
        public string? NameGroup { get; set; }
        [BsonElement("AvatarGroup")]
        public string? AvatarGroup { get; set; }

        [BsonElement("IsGroup")]
        public bool IsGroup { get; set; }

        [BsonElement("Members")]
        public List<string> Members { get; set; }

        [BsonElement("Messages")]
        public List<Message> Messages { get; set; }
    }
    
}
