using Correios.Net;
using MongoDB.Driver;
using PIDI.App_Start;
using PIDI.Models.Commom;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return View();
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

        public async Task<PedidosModel> GerarPedido(string cep)
        {
            var user = App_Start.SessionContext.Instance.GetUserData();
            var endereco = LocalizarCEP(cep);

            var generatedOrder = new PedidosModel();

            generatedOrder.userId = user.Id.ToString();
            generatedOrder.OrderDate = DateTime.Now;
            generatedOrder.paymentType = "paypal";
            generatedOrder.State = endereco.Estado;
            generatedOrder.City = endereco.Cidade;
            generatedOrder.Country = "Brasil";
            generatedOrder.Address = endereco.rua;
            generatedOrder.HasBeenShipped = false;
            generatedOrder.PostalCode = cep;

            PedidosModel x = await CriarPedido(generatedOrder);
            return x;
        }

        private async Task<PedidosModel> CriarPedido(PedidosModel pedido)
        {
            await orderCollection.InsertOneAsync(pedido);
            return pedido;
        }

        public EnderecoModel LocalizarCEP(string cep)
        {
            EnderecoModel enderecoTarget = new EnderecoModel();
            if (!string.IsNullOrWhiteSpace(cep))
            {
                Address endereco = SearchZip.GetAddress(cep);
                if (endereco.Zip != null)
                {
                    enderecoTarget.Estado = endereco.State;
                    enderecoTarget.Cidade = endereco.City;
                    enderecoTarget.Distrito = endereco.District;
                    enderecoTarget.rua = endereco.Street;

                    return enderecoTarget;
                }
                else
                {
                    //MessageBox.Show("Cep não localizado...");
                }
            }
            else
            {
                //MessageBox.Show("Informe um CEP válido");
            }
            return null;
        }
    }
}
