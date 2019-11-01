using MongoDB.Driver;
using PIDI.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PIDI.Models;
using Ext.Net;
using System.Web.UI;

namespace PIDI.Controllers.Admin
{
    public class UserController : Controller
    {
        private MongoDBContext dBContext;
        private IMongoCollection<UserModel> userCollection;

        public UserController()
        {
            dBContext = new MongoDBContext();
            userCollection = dBContext.database.GetCollection<UserModel>("users");
        }


        // GET: User
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(UserModel user , string roleName = "user")
        {
            var query = userCollection.AsQueryable<UserModel>().FirstOrDefault(x => x.cpf == user.cpf || x.email == user.email);
            // TODO: Add insert logic here
            if (query == null)
            {
                user.Perfil = roleName;
                userCollection.InsertOne(user);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (query.cpf == user.cpf)
                    ModelState.AddModelError("cpf", "CPF já está sendo utilizado!");
                if (query.email == user.email)
                    ModelState.AddModelError("Email", "Email já está sendo utilizado!");

                return View();
            }
        }

        public void CreateDefaultAdminAccount()
        {
            UserModel user = new UserModel();
            user.email = "admin@admin.com";
            user.senha = "admin";

            var query = userCollection.AsQueryable<UserModel>().FirstOrDefault(x => x.email == user.email);
            // TODO: Add insert logic here
            if (query == null)
            {
                user.Perfil = "Administrador";
                userCollection.InsertOne(user);
            }
        }

    }
}