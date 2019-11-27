using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PIDI.Models.Admin
{
    public class PromotionModel
    {
        [BsonId]
        public ObjectId id { get; set; }

        [BsonElement("PromotionName")]
        [Display(Name = "Promoções")]
        public string PromotionName { get; set; }

        [BsonElement("DiscountAmount")]
        [Display(Name = "Desconto")]
        public int discountAmount { get; set; }

        [BsonElement("Products")]
        [Display(Name = "Produtos")]
        public List<ProductModel> products { get; set; }

        [BsonElement("InitDate")]
        [Display(Name = "Data Início")]
        public DateTime InitDate { get; set; }

        [BsonElement("EndDate")]
        [Display(Name = "Data Final")]
        public DateTime EndDate { get; set; }

    }
}