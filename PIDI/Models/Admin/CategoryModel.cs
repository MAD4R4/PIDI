using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PIDI.Models
{
    public class CategoryModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("CategoryName")]
        [Display(Name ="Nome da Categoria")]
        public string CategoryName { get; set; }

        [BsonElement("CategoryDescription")]
        [Display(Name = "Descrição")]

        public string CategoryDescription { get; set; }
    }
}