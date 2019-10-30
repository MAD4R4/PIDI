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

        public List<ProductModel> GetProducts(int quantity= 999, CategoryModel category = null , bool all = false)
        {
            if(all)
            {
                List<ProductModel> products = productCollection.AsQueryable<ProductModel>().ToList();
                return products;
            }

            if(category == null)
            {
                List<ProductModel> products = productCollection.AsQueryable<ProductModel>().ToList();
                var LimitedList = products.OrderByDescending(x => x.ProductName).Take(quantity);
                return LimitedList.ToList();
            }
            else
            {
                //TESTAR
                List<ProductModel> products = productCollection.AsQueryable<ProductModel>().OrderByDescending(x => (x.Category.CategoryName == category.CategoryName)).ToList();
                var LimitedList = products.OrderByDescending(x => x.ProductName).Take(quantity);
                return LimitedList.ToList();
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

        public ActionResult SearhcProduct(string productToFind)
        {
            var allProducts = GetProducts(all:true);
            var products = new List<ProductModel>();
            var minValue = 5;

            if (productToFind == null)
                productToFind = "bbb";
            for (int i = 0; i < allProducts.Count; i++)
            {
                var target = allProducts[i];
                var matchPoints = CheckStringMatchScore(productToFind, target.ProductName);

                if (matchPoints > minValue)
                    products.Add(target);
            }

            return View(products);
        }

        /// <summary>
        /// Checa o quão parecido são as duas strings 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int CheckStringMatchScore(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}
