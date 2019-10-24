using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIDI.Models.Commom
{
    public class CartModel
    {
        public ProductModel product { get; set; }
        public int Quantity { get; set; }
        public CartModel (ProductModel product , int Quantity)
        {
            this.product = product;
            this.Quantity = Quantity;
        }
    }
}