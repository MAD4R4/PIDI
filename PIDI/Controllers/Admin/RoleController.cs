using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIDI.Models.Admin;
using PIDI.App_Start;
using MongoDB.Driver;
using MongoDB.Bson;

namespace PIDI.Controllers.Admin
{
    [Authorize(Roles = "Administrador")]
    public class RoleController : Controller
    {
        private MongoDBContext dBContext;
        private IMongoCollection<RoleModel> roleCollection;

        public RoleController()
        {
            dBContext = new MongoDBContext();
            roleCollection = dBContext.database.GetCollection<RoleModel>("roles");
        }


        // GET: Role
        public ActionResult Index()
        {
            List<RoleModel> roles = GetRoles();

            return View(roles);
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(RoleModel role)
        {
            try
            {
                var query = roleCollection.AsQueryable<RoleModel>().FirstOrDefault(x => x.RoleName == role.RoleName);

                if (query == null)
                    roleCollection.InsertOne(role);

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
            var role = roleCollection.AsQueryable<RoleModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(id));
            return View(role);
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, RoleModel role)
        {
            try
            {
                // TODO: Add delete logic here
                var filter = Builders<RoleModel>.Filter.Eq("_id", ObjectId.Parse(id));
                roleCollection.DeleteOne(filter);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(string id)
        {
            var roleID = ObjectId.Parse(id);
            var role = roleCollection.AsQueryable<RoleModel>().SingleOrDefault(x => x.Id == roleID);
            return View(role);
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, RoleModel role)
        {
            try
            {
                // TODO: Add update logic here
                var filter = Builders<RoleModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<RoleModel>.Update
                    .Set("RoleName", role.RoleName);

                var result = roleCollection.UpdateOne(filter, update);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }




        public List<RoleModel> GetRoles()
        {
            var roles = roleCollection.AsQueryable<RoleModel>().ToList();
            return roles;
        }
    }
}