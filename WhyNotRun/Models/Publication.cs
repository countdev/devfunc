using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhyNotRun.Models
{
    public class Publication
    {
        public Publication()
        {
            Techies = new List<ObjectId>();
            Comments = new List<Comment>();
            Likes = new List<ObjectId>();
            Dislikes = new List<ObjectId>();
        }

        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("techies")]
        public List<ObjectId> Techies { get; set; }

        [BsonElement("userId")]
        public ObjectId UserId { get; set; }

        [BsonElement("comments")]
        public List<Comment> Comments { get; set; }

        [BsonElement("likes")]
        public List<ObjectId> Likes { get; set; }

        [BsonElement("dislikes")]
        public List<ObjectId> Dislikes { get; set; }

        [BsonElement("dateCreation")]
        public DateTime DateCreation { get; set; }

        [BsonElement("deletedAt"), BsonIgnoreIfNull]
        public DateTime? DeletedAt { get; set; }
        
    }
}