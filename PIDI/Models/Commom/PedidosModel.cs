using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace PIDI.Models.Commom
{
    public class PedidosModel
    {
        [BsonId]
        public string id { get; set; }

        public string description { get; set; }

        public string status { get; set; }

        public string precoTotal { get; set; }

        public string dtPeiddo { get; set; }






    }
}