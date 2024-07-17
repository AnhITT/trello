using DataAccess_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DataAccess_Layer.Models
{
    public abstract class TrackableEntryMongo
    {
        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
