using MongoDB.Driver;
using PIDI.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIDI.Models.Admin;
using MongoDB.Bson;
using PIDI.Models;
using System.Web.Helpers;

namespace PIDI.Controllers.Admin
{
    public class PromotionController : Controller
    {

        private MongoDBContext dBContext;
        private IMongoCollection<PromotionModel> promotionCollection;

        public static PromotionModel promotionModel;
        private static PromotionController _instance;

        public static PromotionController Instance
        {
            get
            {
                return GetInstace();
            }
            set { }
        }
        private static PromotionController GetInstace()
        {
            if (_instance == null)
            {
                _instance = new PromotionController();
                return _instance;
            }
            else
                return _instance;
        }

        public PromotionController()
        {
            dBContext = new MongoDBContext();
            promotionCollection = dBContext.database.GetCollection<PromotionModel>("promotions");
        }

        // GET: Promotion
        public ActionResult Index()
        {
            List<PromotionModel> promotions = promotionCollection.AsQueryable<PromotionModel>().ToList();
            return View(promotions);
        }

        public ActionResult EditPromotion(string id)
        {
            var productID = new ObjectId(id);
            var promotion = promotionCollection.AsQueryable<PromotionModel>().SingleOrDefault(x => x.id == productID);
            return View(promotion);
        }

        [HttpPost]
        public ActionResult EditPromotion(string id ,PromotionModel promotion, string productsString)
        {
            string[] productsId = productsString.Split(',');
            List<ProductModel> products = new List<ProductModel>();
            for (int i = 0; i < productsId.Length; i++)
            {
                var target = productsId[i];
                var product = ProductController.Instance.GetProduct(target);
                products.Add(product);
            }
            promotion.products = products;

            var filter = Builders<PromotionModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<PromotionModel>.Update
                .Set("PromotionName", promotion.PromotionName)
                .Set("DiscountAmount", promotion.discountAmount)
                .Set("Products", products)
                .Set("InitDate", promotion.InitDate)
                .Set("EndDate", promotion.EndDate);

            var result = promotionCollection.UpdateOne(filter, update);
            return null;
        }

        public ActionResult PromotionDetails(string id)
        {
            var productID = new ObjectId(id);
            var promotion = promotionCollection.AsQueryable<PromotionModel>().SingleOrDefault(x => x.id == productID);
            return View(promotion);
        }

        public ActionResult CreatePromotion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePromotion(PromotionModel promotion , string productsString)
        {
            try
            {
                string[] productsId = productsString.Split(',');
                List<ProductModel> products = new List<ProductModel>();
                for (int i = 0; i < productsId.Length; i++)
                {
                    var target = productsId[i];
                    var product = ProductController.Instance.GetProduct(target);
                    products.Add(product);
                }
                promotion.products = products;

                promotionCollection.InsertOne(promotion);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeletePromotion(string id)
        {
            var promotionID = new ObjectId(id);
            var promotion = promotionCollection.AsQueryable<PromotionModel>().SingleOrDefault(x => x.id == promotionID);
            return View(promotion);
        }

        [HttpPost]
        public ActionResult DeletePromotion(string id , PromotionModel promotion)
        {
            try
            {
                var filter = Builders<PromotionModel>.Filter.Eq("_id", ObjectId.Parse(id));
                promotionCollection.DeleteOne(filter);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public float GetProductPrice(string id)
        {
            var productID = new ObjectId(id);
            var promotions = promotionCollection.AsQueryable<PromotionModel>().ToList();

            foreach (var item in promotions)
            {
                foreach (var product in item.products)
                {
                    if(product.Id == productID)
                    {
                        float precoAtual = product.GetPrice(false);
                        float desconto = precoAtual * (item.discountAmount/100f);
                        float newPrice = precoAtual - desconto;
                        return newPrice;
                    }
                }
            }

            return 0;
        }
    }
}