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
        [Display(Name = "Produto")]

        public string ProductName { get; set; }

        [BsonElement("ProductDescription")]
        [Display(Name = "Descrição")]
        public string ProductDescription { get; set; }

        [BsonElement("Preco")]
        [Display(Name = "Preço")]
        public float Preco { get; set; }

        [BsonElement("Quantity")]
        [Display(Name = "Quantidade")]
        public string Quantity { get; set; }

        [BsonElement("Category")]
        [Display(Name = "Categoria")]
        public string Category { get; set; }

        [BsonElement("ProductImages")]
        [Display(Name = "Imagem do produto")]
        public List<MongoPictureModel> productImages { get; set; }

        public float GetPrice(bool promotion = false)
        {
            if (!promotion)
                return Preco;
            else
            {
                var newPrice = PIDI.Controllers.Admin.PromotionController.Instance.GetProductPrice(Id.ToString());

                if (newPrice == 0)
                    return Preco;
                else
                    return newPrice;
            }
        }

    }
}