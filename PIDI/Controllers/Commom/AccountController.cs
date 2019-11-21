using MongoDB.Driver;
using PIDI.App_Start;
using PIDI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using System.Web.Mvc.Filters;
using System.Web.Security;

namespace PIDI.Controllers
{
    public class AccountController : Controller 
    {
        private MongoDBContext dBContext;
        private IMongoCollection<UserModel> userCollection;

        public AccountController()
        {
            dBContext = new MongoDBContext();
            userCollection = dBContext.database.GetCollection<UserModel>("users");
        }


        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowProfile()
        {
            var user = SessionContext.Instance.GetUserData();
            if (user != null)
                return View(user);
            else
            return RedirectToAction("Index", "Home");

        }

        [Authorize]
        public ActionResult EditProfile()
        {
            var user = SessionContext.Instance.GetUserData();
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserModel user)
        {
            var query = userCollection.AsQueryable<UserModel>().FirstOrDefault(x => x.Id == SessionContext.Instance.GetUserData().Id);
            if (query != null)
            {
                var update = userCollection.FindOneAndUpdateAsync(Builders<UserModel>.Filter.Eq("_id", query.Id),
                    Builders<UserModel>.Update
                    .Set("nome", user.nome)
                    .Set("dtNascimento", user.dtNascimento)
                    .Set("sexo", user.sexo));


                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Verifique se está tudo correto");
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AccountModel user)
        {
            var validateUser = userCollection.AsQueryable<UserModel>().FirstOrDefault(x => x.email == user.email && x.senha == user.senha);
            // TODO: Add insert logic here
            if (validateUser != null)
            {
                //userCollection.InsertOne(user);
                SessionContext.Instance.SetAuthenticationToken(validateUser.Id.ToString(), false, validateUser);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email ou senha estão errados!");
                return View();
            }

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //AINDA PRECISA CHAMAR
        public void RegistrarEndereco(Endereco endereco)
        {
            var user = userCollection.AsQueryable<UserModel>().FirstOrDefault(x => x.Id == SessionContext.Instance.GetUserData().Id);
            user.enderecos.Add(endereco);

            if (user != null)
            {
                var update = userCollection.FindOneAndUpdateAsync(Builders<UserModel>.Filter.Eq("_id", user.Id),
                    Builders<UserModel>.Update
                    .Set("Enderecos", user.enderecos));
            }
        }
    }
}