using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIDI.Models
{
    public class CategoryModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("CategoryName")]
        public string CategoryName { get; set; }

        [BsonElement("CategoryDescription")]
        public string CategoryDescription { get; set; }
    }
}