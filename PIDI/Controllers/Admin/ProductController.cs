using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;
using PIDI.App_Start;
using PIDI.Models;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using PIDI.Models.Commom;

namespace PIDI.Controllers.Admin
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

        public List<ProductModel> GetProducts(int quantity = 999, bool all = false)
        {
            var deleted = CheckNullProducts();

            if (all)
            {
                List<ProductModel> products = productCollection.AsQueryable<ProductModel>().ToList();
                return products;
            }
            else
            {
                List<ProductModel> products = productCollection.AsQueryable<ProductModel>().ToList();
                var LimitedList = products.OrderByDescending(x => x.ProductName).Take(quantity);
                return LimitedList.ToList();
            }
        }

        public List<ProductModel> GetProducts(List<ProductModel> products)
        {
            var deleted = CheckNullProducts();
            List<ProductModel> Allproducts = productCollection.AsQueryable<ProductModel>().ToList();
            var filteredList = Allproducts.Where(p => !products.Any(l => p.Id == l.Id)).ToList();
            return filteredList;
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
        public async Task<ActionResult> Create()
        {
            var produto = await GenerateProduct();
            return View(produto);
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(string id, ProductModel product)
        {
            try
            {
                // TODO: Add insert logic here
                //productCollection.InsertOne(product);
                var productID = new ObjectId(id);

                var filter = Builders<ProductModel>.Filter.Eq("_id", productID);
                var update = Builders<ProductModel>.Update
                    .Set("ProductName", product.ProductName)
                    .Set("Preco", product.GetPrice())
                    .Set("ProductDescription", product.ProductDescription)
                    .Set("Category", product.Category)
                    .Set("Quantity", product.Quantity);

                var result = productCollection.UpdateOne(filter, update);

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
                    .Set("Preco", product.GetPrice())
                    .Set("ProductDescription", product.ProductDescription)
                    .Set("Category", product.Category)
                    .Set("Quantity", product.Quantity);

                var result = productCollection.UpdateOne(filter, update);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ProductModel> GenerateProduct()
        {
            ProductModel product = new ProductModel();
            await productCollection.InsertOneAsync(product);
            return product;
        }

        [HttpPost]
        public JsonResult UpdateProductPhotos(string id)
        {
            try
            {
                List<MongoPictureModel> pictures = new List<MongoPictureModel>();

                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        //var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        //var fileName = Path.GetFileName(file);
                        //var path = Path.Combine(Server.MapPath("~/App_Data/Images"), fileName);
                        //using (var fileStream = File.Create(path))
                        //{
                        //    stream.CopyTo(fileStream);
                        //}

                        var image = TransformToImage(fileContent);
                        pictures.Add(image);
                    }
                }

                var sucess = UploadImage(id, pictures);

            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            return Json("File uploaded successfully");
        }

        public FileContentResult ShowPicture(string productId, string id)
        {
            //get picture document from db
            var product = productCollection.Find(x => x.Id == new ObjectId(productId)).SingleOrDefault();
            var thePicture = product.productImages.Find(x => x.id == new ObjectId(id));

            //transform the picture's data from string to an array of bytes
            var thePictureDataAsBytes = Convert.FromBase64String(thePicture.PictureDataAsString);

            var fileResult = new FileContentResult(thePictureDataAsBytes, "image/jpeg");

            //return array of bytes as the image's data to action's response. 
            //We set the image's content mime type to image/jpeg
            return fileResult;
        }

        public bool UploadImage(string id, List<MongoPictureModel> pictures)
        {
            try
            {
                var productID = new ObjectId(id);
                var product = productCollection.AsQueryable<ProductModel>().SingleOrDefault(x => x.Id == productID);

                var filter = Builders<ProductModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<ProductModel>.Update
                    .Set("ProductImages", pictures);

                var result = productCollection.UpdateOne(filter, update);
                return true;
            }
            catch
            {
                return false;
            }

        }


        public MongoPictureModel TransformToImage(HttpPostedFileBase theFile)
        {
            if (theFile.ContentLength > 0)
            {
                //get the file's name 
                string theFileName = Path.GetFileName(theFile.FileName);

                //get the bytes from the content stream of the file
                byte[] thePictureAsBytes = new byte[theFile.ContentLength];
                using (BinaryReader theReader = new BinaryReader(theFile.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(theFile.ContentLength);
                }

                //convert the bytes of image data to a string using the Base64 encoding
                string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);

                //create a new mongo picture model object to insert into the db
                MongoPictureModel thePicture = new MongoPictureModel()
                {
                    FileName = theFileName,
                    PictureDataAsString = thePictureDataAsString
                };

                return thePicture;
            }
            else
                return null;

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

        public void DeleteProdutct(string id)
        {
            var filter = Builders<ProductModel>.Filter.Eq("_id", ObjectId.Parse(id));
            productCollection.DeleteOne(filter);
        }

        public ProductModel GetProduct(string id)
        {
            var productID = new ObjectId(id);
            var product = productCollection.AsQueryable<ProductModel>().SingleOrDefault(x => x.Id == productID);
            return (product);
        }

        public ActionResult SearchProduct(string productToFind)
        {
            var allProducts = GetProducts(all: true);
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

        public ActionResult FilterProducts(string category, int quantity = 100)
        {
            var products = productCollection.Find(x => x.Category == category).ToList();
            var LimitedList = products.OrderByDescending(x => x.ProductName).Take(quantity);

            return View(LimitedList.ToList());
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

        public bool CheckNullProducts()
        {
            List<ProductModel> products = productCollection.AsQueryable<ProductModel>().ToList();
            bool deletedNullProducts = false;
            foreach (var item in products)
            {
                if (item.ProductName == null || item.productImages == null || item.Category == null)
                {
                    DeleteProdutct(item.Id.ToString());
                    deletedNullProducts = true;
                }
            }

            return deletedNullProducts;
        }
    }
}
