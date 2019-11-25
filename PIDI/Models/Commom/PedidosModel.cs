
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
        [BsonElement("OrderId")]
        [Display(Name = "ID do Pedido")]
        public ObjectId OrderId { get; set; }
        [BsonElement("OrderDate")]
        [Display(Name = "Data do Pedido")]
        public System.DateTime OrderDate { get; set; }
        [BsonElement("userId")]
        [Display(Name = "ID do usuário")]
        public string userId { get; set; }
        [BsonElement("Address")]
        [Display(Name = "Endereço")]
        public string Address { get; set; }
        [BsonElement("City")]
        [Display(Name = "Cidade")]
        public string City { get; set; }
        [BsonElement("State")]
        [Display(Name = "UF")]
        public string State { get; set; }
        [BsonElement("PostalCode")]
        [Display(Name = "CEP")]
        public string PostalCode { get; set; }
        [BsonElement("Country")]
        [Display(Name = "País")]
        public string Country { get; set; }
        [BsonElement("produtosRequisitados")]
        [Display(Name = "Produtos")]
        public List<PedidoElementModel> produtosRequisitados { get; set; }
        public decimal Total { get; set; }
        public string paymentType { get; set; }
        [BsonElement("orderState")]
        [Display(Name = "Status do Pedido")]
        public string orderState { get; set; }

    }
}