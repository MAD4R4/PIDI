using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PIDI.Models.Admin
{
    public class PromotionModel
    {
        [BsonId]
        public ObjectId id { get; set; }

        [BsonElement("PromotionName")]
        public string PromotionName { get; set; }

        [BsonElement("DiscountAmount")]
        public int discountAmount { get; set; }

        [BsonElement("Products")]
        public List<ProductModel> products { get; set; }

        [BsonElement("InitDate")]
        public DateTime InitDate { get; set; }

        [BsonElement("EndDate")]
        public DateTime EndDate { get; set; }

    }
}