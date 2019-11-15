using MongoDB.Driver;
using PIDI.App_Start;
using PIDI.Models;
using PIDI.Models.Commom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PIDI.Controllers.Admin
{
    [Authorize(Roles = "Administrador")]
    public class RelatorioController : Controller
    {

        public static RelatorioController Instance
        {
            get
            {
                return GetInstace();
            }
            set { }
        }
        private static RelatorioController _instance;

        private MongoDBContext dBContext;

        private static RelatorioController GetInstace()
        {
            if (_instance == null)
            {
                _instance = new RelatorioController();
                return _instance;
            }
            else
                return _instance;
        }

        #region Collections Reference

        private IMongoCollection<ProductModel> productCollection;
        private IMongoCollection<PedidosModel> orderCollection;
        private IMongoCollection<UserModel> userCollection;

        #endregion



        public RelatorioController()
        {
            dBContext = new MongoDBContext();

            productCollection = dBContext.database.GetCollection<ProductModel>("product");
            orderCollection = dBContext.database.GetCollection<PedidosModel>("orders");
            userCollection = dBContext.database.GetCollection<UserModel>("user");
        }

       public List<PedidosModel> RelatorioPedido()
       {
            var pedidos = orderCollection.AsQueryable<PedidosModel>().ToList();
            var LimitedList = pedidos.OrderByDescending(x => x.OrderDate).Take(30);
            return LimitedList.ToList();
       }

        
    }
}