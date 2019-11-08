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

        public ProductModel produtoRequisitado { get; set; }

        public int Quantity { get; set; }

        public double? UnitPrice { get; set; }

        public MongoPictureModel productImage  { get; set; }

    }
}