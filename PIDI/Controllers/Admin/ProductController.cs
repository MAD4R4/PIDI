using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;
using PIDI.App_Start;

namespace PIDI.Models
{
    public class ProductController : Controller
    {
        public static ProductController Instance
        {
            get 
            {
                return GetInstace();
            }
            set { }
        }
        private static ProductController _instance;

        private MongoDBContext dBContext;
        private IMongoCollection<ProductModel> productCollection;

        private static ProductController GetInstace()
        {
            if (_instance == null)
            {
                _instance = new ProductController();
                return _instance;
            }
            else
                return _instance;
        }

        public ProductController()
        {
            dBContext = new MongoDBContext();
            productCollection = dBContext.database.GetCollection<ProductModel>("product");
        }

        // GET: Product
        [Authorize]
        public ActionResult Index()
        {
            List<ProductModel> products = productCollection.AsQueryable<ProductModel>().ToList();
            return View(products);
        }

        public List<ProductModel> GetProducts(int quantity, CategoryModel category = null)
        {
            if(category == null)
            {
                List<ProductModel> products = productCollection.AsQueryable<ProductModel>().ToList();
                var LimitedList = products.OrderByDescending(x => x.ProductName).Take(quantity);
                return products;
            }
            else
            {
                //TESTAR
                List<ProductModel> products = productCollection.AsQueryable<ProductModel>().OrderByDescending(x => (x.Categoria.CategoryName == category.CategoryName)).ToList();
                var LimitedList = products.OrderByDescending(x => x.ProductName).Take(quantity);
                return products;
            }
            
        }

        // GET: Product/Details/5
        public ActionResult Details(string id)
        {
            var productID = new ObjectId(id);
            var product = productCollection.AsQueryable<ProductModel>().SingleOrDefault(x => x.Id == productID);
            return View(product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(ProductModel product)
        {
            try
            {
                // TODO: Add insert logic here
                productCollection.InsertOne(product);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Product/Edit/5
        public ActionResult Edit(string id)
        {
            var productID = new ObjectId(id);
            var product = productCollection.AsQueryable<ProductModel>().SingleOrDefault(x => x.Id == productID);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, ProductModel product)
        {
            try
            {
                // TODO: Add update logic here
                var filter = Builders<ProductModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<ProductModel>.Update
                    .Set("ProductName", product.ProductName)
                    .Set("ProductDescripton", product.ProductDescription)
                    .Set("Quantity", product.Quantity);
                var result = productCollection.UpdateOne(filter, update);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Product/Delete/5
        public ActionResult Delete(string id)
        {
            var productID = new ObjectId(id);
            var product = productCollection.AsQueryable<ProductModel>().SingleOrDefault(x => x.Id == productID);
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, ProductModel product)
        {
            try
            {
                // TODO: Add delete logic here
                var filter = Builders<ProductModel>.Filter.Eq("_id", ObjectId.Parse(id));
                productCollection.DeleteOne(filter);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ProductModel GetProduct(string id)
        {
            var productID = new ObjectId(id);
            var product = productCollection.AsQueryable<ProductModel>().SingleOrDefault(x => x.Id == productID);
            return (product);
        }
    }
}
