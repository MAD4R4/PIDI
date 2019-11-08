using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using PIDI.Models.Commom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PIDI.Models
{
    public class ProductModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("ProductName")]
        [Display(Name = "Nome Produto")]

        public string ProductName { get; set; }

        [BsonElement("ProductDescription")]
        [Display(Name = "Descrição")]
        public string ProductDescription{ get; set; }

        [BsonElement("Preco")]
        public float Preco { get; set; }

        [BsonElement("Quantity")]
        public string Quantity { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("Product Images")]
        public List<MongoPictureModel> productImages { get; set; }

    }
}