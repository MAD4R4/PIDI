using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIDI.Models
{
    public class OrderModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("cpf")]
        public string cpf { get; set; }

        [BsonElement("dtOrder")]
        public string dateOrder { get; set; }

        [BsonElement("paymentType")]
        public string paymentType { get; set; }

        [BsonElement("totalPrice")]
        public string totalPrice { get; set; }

        [BsonElement("products")]
        public string products { get; set; }
    }
}