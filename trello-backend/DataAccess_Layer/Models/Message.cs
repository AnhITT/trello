using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess_Layer.Models
{
     public class Message
     {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Sender")]
        public string Sender { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }  // Text, Media, Document, Link

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("File")]
        public string File { get; set; }

        [BsonElement("Fullname")]
        public string Fullname { get; set; }

        [BsonElement("Avatar")]
        public string Avatar { get; set; }

        [BsonElement("PubDate")]
        public string PubDate { get; set; }
     }
}
