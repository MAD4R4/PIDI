using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace PIDI.Models.Commom
{
    public class PedidoElementModel
    {

        public string productID { get; set; }
        public string requiredQuantity { get; set; }
        public MongoPictureModel productImage  { get; set; }

    }
}