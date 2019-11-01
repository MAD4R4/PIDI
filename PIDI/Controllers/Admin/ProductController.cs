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
        [Authorize(Roles = "Administrador")]
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

        // GET: Product/Details/5
        public ActionResult VisualizeProduct(string id)
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

        public ActionResult SearchProduct(string productToFind)
        {
            var allProducts = GetProducts(all:true);
            var products = new List<ProductModel>();
            var minValue = .4f;

            for (int i = 0; i < allProducts.Count; i++)
            {
                var target = allProducts[i];
                var matchPoints = CalculateSimilarity(productToFind, target.ProductName);

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
        public static int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        /// <summary>
        /// Calculate percentage similarity of two strings
        /// <param name="source">Source String to Compare with</param>
        /// <param name="target">Targeted String to Compare</param>
        /// <returns>Return Similarity between two strings from 0 to 1.0</returns>
        /// </summary>
        double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }
    }
}
