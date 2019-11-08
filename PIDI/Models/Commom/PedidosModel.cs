using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int OrderId { get; set; }

        public System.DateTime OrderDate { get; set; }

        public string userId { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        //public string Phone { get; set; }

        public string Email { get; set; }

        [ScaffoldColumn(false)]
        public decimal Total { get; set; }

        [ScaffoldColumn(false)]
        public string paymentType { get; set; }

        [ScaffoldColumn(false)]
        public bool HasBeenShipped { get; set; }

        //public List<PedidoElementModel> OrderDetails { get; set; }






    }
}