using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIDI.Models.Commom;
using PIDI.Models;
using MongoDB.Bson;
using PIDI.App_Start;
using PIDI.Controllers.Admin;

namespace PIDI.Controllers.Commom
{
    public class ShoppingCartController : Controller
    {
        private string cartName = "Cart";


        //private ShoppingCartController()
        //{
        //    productController = new ProductController();
        //}

        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult OrderNow(string id)
        {
            if (SessionManager.ReturnSessionObject(cartName) == null)
            {
                List<CartModel> cartList = new List<CartModel> {

                    new CartModel(ProductController.Instance.GetProduct(id),1)
                };

                SessionManager.RegisterSession(cartName, cartList);
            }
            else
            {
                List<CartModel> cart = (List<CartModel>) SessionManager.ReturnSessionObject(cartName);
                int index = CheckIfExist(id);

                if (index == -1)
                    cart.Add(new CartModel(ProductController.Instance.GetProduct(id), 1));
                else
                    cart[index].Quantity++;

                SessionManager.RegisterSession(cartName, cart);

            }

            return View("Index");
        }

        public int CheckIfExist(string id)
        {
            var productID = new ObjectId(id);

            List<CartModel> lsCart = (List<CartModel>) SessionManager.ReturnSessionObject(cartName);
            for (int i = 0; i < lsCart.Count; i++)
            {
                if (lsCart[i].product.Id == productID) return i;
            }

            return -1;
        }
    }
}