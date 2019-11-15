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

namespace PIDI.Controllers.Admin
{
    public class CategoryController : Controller
    {

        public static CategoryController Instance
        {
            get
            {
                return GetInstace();
            }
            set { }
        }
        private static CategoryController _instance;

        private MongoDBContext dBContext;
        private IMongoCollection<CategoryModel> categoryCollection;

        public CategoryController()
        {
            dBContext = new MongoDBContext();
            categoryCollection = dBContext.database.GetCollection<CategoryModel>("category");
        }

        private static CategoryController GetInstace()
        {
            if (_instance == null)
            {
                _instance = new CategoryController();
                return _instance;
            }
            else
                return _instance;
        }

        // GET: category
        public ActionResult Index()
        {
            List<CategoryModel> categorys = categoryCollection.AsQueryable<CategoryModel>().ToList();
            return View(categorys);
        }

        // GET: category/Details/5
        public ActionResult Details(string id)
        {
            var categoryID = new ObjectId(id);
            var category = categoryCollection.AsQueryable<CategoryModel>().SingleOrDefault(x => x.Id == categoryID);
            return View(category);
        }

        // GET: category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: category/Create
        [HttpPost]
        public ActionResult Create(CategoryModel category)
        {
            try
            {
                // TODO: Add insert logic here
                categoryCollection.InsertOne(category);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: category/Edit/5
        public ActionResult Edit(string id)
        {
            var categoryID = new ObjectId(id);
            var category = categoryCollection.AsQueryable<CategoryModel>().SingleOrDefault(x => x.Id == categoryID);
            return View(category);
        }

        // POST: category/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, CategoryModel category)
        {
            try
            {
                // TODO: Add update logic here
                var filter = Builders<CategoryModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<CategoryModel>.Update
                    .Set("CategoryName", category.CategoryName)
                    .Set("CategoryDescription", category.CategoryDescription);
                var result = categoryCollection.UpdateOne(filter, update);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: category/Delete/5
        public ActionResult Delete(string id)
        {
            var categoryID = new ObjectId(id);
            var category = categoryCollection.AsQueryable<CategoryModel>().SingleOrDefault(x => x.Id == categoryID);
            return View(category);
        }

        // POST: category/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, CategoryModel category)
        {
            try
            {
                // TODO: Add delete logic here
                var filter = Builders<CategoryModel>.Filter.Eq("_id", ObjectId.Parse(id));
                categoryCollection.DeleteOne(filter);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public List<CategoryModel> GetCategories(int quantity)
        {
                List<CategoryModel> categories = categoryCollection.AsQueryable().ToList();
            var LimitedList = categories.OrderByDescending(x => x.CategoryName).Take(quantity);
            return categories;
        }
    }
}
