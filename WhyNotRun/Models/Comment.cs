using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhyNotRun.Models
{
    public class Comment
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("user")]
        public ObjectId UserId { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }
        
        [BsonElement("dateCreation")]
        public DateTime DateCreation { get; set; }

        [BsonElement("UserName")]
        public string UserName { get; set; }

        [BsonElement("UserPicture")]
        public string UserPicture { get; set; }

        [BsonElement("UserProfession")]
        public string UserProfession { get; set; }
        
        
    }
}