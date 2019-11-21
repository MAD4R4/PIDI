using MongoDB.Bson;
using MongoDB.Driver;
using PIDI.App_Start;
using PIDI.Models.Commom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PIDI.Controllers.Commom
{
    public class PedidoController : Controller
    {
        private MongoDBContext dBContext;
        private IMongoCollection<PedidosModel> orderCollection;

        public PedidoController()
        {
            dBContext = new MongoDBContext();
            orderCollection = dBContext.database.GetCollection<PedidosModel>("orders");
        }


        // GET: Pedido
        public ActionResult Index()
        {
            return View();
        }

        // GET: Pedido/Details/5
        public ActionResult Details(string id)
        {
            var orderID = new ObjectId(id);
            var order = orderCollection.AsQueryable<PedidosModel>().SingleOrDefault(x => x.OrderId == orderID);
            return View(order);
        }

        // GET: Pedido/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pedido/Create
        [HttpPost]
        public ActionResult Create(PedidosModel pedido)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public void UpdateOrder(PedidosModel order)
        {
            var orderFinded = orderCollection.AsQueryable<PedidosModel>().SingleOrDefault(x => x.OrderId == order.OrderId);

            var filter = Builders<PedidosModel>.Filter.Eq("_id", order.OrderId);
            var update = Builders<PedidosModel>.Update
                .Set("orderState", order.orderState);

            var result = orderCollection.UpdateOne(filter, update);
        }

        // GET: Pedido/Edit/5
        public ActionResult Edit(string id)
        {
            return View();
        }

        // POST: Pedido/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, PedidosModel pedido)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Pedido/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Pedido/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, PedidosModel pedido)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public PedidosModel GetOrder(string id)
        {
            var orderID = new ObjectId(id);
            var order = orderCollection.AsQueryable<PedidosModel>().SingleOrDefault(x => x.OrderId == orderID);
            return order;
        }

        [HttpPost]
        public async Task<ActionResult> VerPedido(Endereco endereco)
        {
            PedidosModel pedido = await GerarPedido(endereco);
            return View(pedido);
        }

        public ActionResult VerPedido(string id)
        {
            var orderID = new ObjectId(id);
            var order = orderCollection.AsQueryable<PedidosModel>().SingleOrDefault(x => x.OrderId == orderID);
            return View(order);
        }

        public async Task<PedidosModel> GerarPedido(Endereco endereco)
        {
            var user = App_Start.SessionContext.Instance.GetUserData();
            List<PedidoElementModel> cartItems = (List<PedidoElementModel>)PIDI.App_Start.SessionManager.ReturnSessionObject("items");

            var generatedOrder = new PedidosModel();
            generatedOrder.userId = user.userId;
            generatedOrder.OrderDate = DateTime.Now;
            //generatedOrder.OrderDate = DateTime.Parse("16/10/2019 20:31:19"); usei para teste de filtragem
            generatedOrder.paymentType = "paypal";
            generatedOrder.State = endereco.uf;
            generatedOrder.City = endereco.cidade;
            generatedOrder.Country = "Brasil";
            generatedOrder.Address = endereco.rua;
            generatedOrder.orderState = "Aguardando Pagamento";
            generatedOrder.PostalCode = endereco.cep;
            generatedOrder.produtosRequisitados = cartItems;
            generatedOrder.Total = GerarTotal(cartItems);
            PedidosModel x = await CriarPedido(generatedOrder);
            return x;
        }

        private async Task<PedidosModel> CriarPedido(PedidosModel pedido)
        {
            await orderCollection.InsertOneAsync(pedido);
            return pedido;
        }

        private decimal GerarTotal(List<PedidoElementModel> items)
        {
            float valorTotal = 0f;
            for (int i = 0; i < items.Count; i++)
            {
                valorTotal += items[i].produtoRequisitado.Preco * items[i].Quantity;
            }

            return (decimal) valorTotal;
        }
    }
}
