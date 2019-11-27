using MongoDB.Driver;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using PIDI.App_Start;
using PIDI.Models;
using PIDI.Models.Commom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

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
        public static List<PedidosModel> lastOrderRequest = new List<PedidosModel>();
        public static List<UserModel> lastUserRequest = new List<UserModel>();
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
            userCollection = dBContext.database.GetCollection<UserModel>("users");

        }

        #region Relatorio Pedido
        /// <summary>
        /// Gerar relatório de pedidos
        /// </summary>
        /// <returns></returns>
        public ActionResult GerarRelatorioPedido(DateTime dtInicio, DateTime dtFinal)
        {
            var pedidos = orderCollection.AsQueryable().ToList();
            var filtered = pedidos.FindAll(x => x.OrderDate.Date <= dtFinal && x.OrderDate.Date >= dtInicio);
            lastOrderRequest = filtered;

            ViewBag.dtInicio = dtInicio.ToString("dd/MM/yyyy");
            ViewBag.dtFinal = dtFinal.ToString("dd/MM/yyyy");

            return View(filtered);
        }

        [HttpPost]
        public ActionResult GerarRelatorioPedido(DateTime dtInicio, DateTime dtFinal, string orderState)
        {
            var pedidos = orderCollection.AsQueryable().ToList();
            var filtered = pedidos.FindAll(x => x.OrderDate.Date <= dtFinal && x.OrderDate.Date >= dtInicio && x.orderState == orderState);
            lastOrderRequest = filtered;

            ViewBag.dtInicio = dtInicio.ToString("dd/MM/yyyy");
            ViewBag.dtFinal = dtFinal.ToString("dd/MM/yyyy");

            return View(filtered);
        }

        public void DownloadPedidosExcel(string horaGerado)
        {
            var collection = lastOrderRequest;//db.GetCollection<EmployeeDetails>("EmployeeDetails").Find(new BsonDocument()).ToList();

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "Dia Geração Relatório";
            Sheet.Cells["B1"].Value = "Order ID";
            Sheet.Cells["C1"].Value = "Requested Products";
            Sheet.Cells["D1"].Value = "Order State";
            Sheet.Cells["E1"].Value = "Total";

            int row = 2;
            Sheet.Cells[string.Format("A{0}", row)].Value = horaGerado;

            foreach (var item in collection)
            {
                Sheet.Cells[string.Format("B{0}", row)].Value = item.OrderId;

                string produtos = "";
                for (int i = 0; i < item.produtosRequisitados.Count; i++)
                {
                    produtos += item.produtosRequisitados[i].produtoRequisitado.ProductName;
                    produtos += " x " + item.produtosRequisitados[i].Quantity + " \n ";
                }

                Sheet.Cells[string.Format("C{0}", row)].Style.WrapText = true;
                Sheet.Cells[string.Format("C{0}", row)].Value = produtos;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.orderState;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Total;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "RelatorioPedido.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        #endregion

        #region Relatório Clientes
        /// <summary>
        /// Gerar relatório de pedidos
        /// </summary>
        /// <returns></returns>
        public ActionResult GerarRelatorioCliente(DateTime dtInicio, DateTime dtFinal)
        {
            var pedidos = userCollection.AsQueryable().ToList();
            var filtered = pedidos.FindAll(x => x.dtCriacao.Date <= dtFinal && x.dtCriacao.Date >= dtInicio);
            lastUserRequest = filtered;

            ViewBag.dtInicio = dtInicio.ToString("dd/MM/yyyy");
            ViewBag.dtFinal = dtFinal.ToString("dd/MM/yyyy");

            return View(filtered);
        }

        [HttpPost]
        public ActionResult GerarRelatorioCliente(DateTime dtInicio, DateTime dtFinal, string clientState)
        {
            var clientes = userCollection.AsQueryable().ToList();
            var filtered = clientes.FindAll(x => x.dtCriacao.Date <= dtFinal.Date && x.dtCriacao.Date >= dtInicio.Date);
            lastUserRequest = filtered;
            
            ViewBag.dtInicio = dtInicio.ToString("dd/MM/yyyy");
            ViewBag.dtFinal = dtFinal.ToString("dd/MM/yyyy");

            return View(filtered);
        }

        public void DownloadClienteExcel(string horaGerado)
        {
            var collection = lastUserRequest;//db.GetCollection<EmployeeDetails>("EmployeeDetails").Find(new BsonDocument()).ToList();

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "Dia Geração Relatório";
            Sheet.Cells["B1"].Value = "User ID";
            Sheet.Cells["C1"].Value = "Nome";
            Sheet.Cells["D1"].Value = "Sexo";
            Sheet.Cells["E1"].Value = "Email";
            Sheet.Cells["F1"].Value = "Data Criação";
            Sheet.Cells["G1"].Value = "Data Nascimento";

            int row = 2;
            Sheet.Cells[string.Format("A{0}", row)].Value = horaGerado;

            foreach (var item in collection)
            {
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Id;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.nome;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.sexo;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.email;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.dtCriacao;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.dtNascimento;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "RelatorioPedido.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        #endregion


        #region Relatório Financeiro
        /// <summary>
        /// Gerar relatório de pedidos
        /// </summary>
        /// <returns></returns>
        public ActionResult GerarRelatorioFinanceiro(DateTime dtInicio, DateTime dtFinal)
        {
            var orders = orderCollection.AsQueryable().ToList();
            var filtered = orders.FindAll(x => x.OrderDate.Date <= dtFinal && x.OrderDate.Date >= dtInicio && x.orderState == "Pago");

            List<relatorioFinanceiroElement> produtos = new List<relatorioFinanceiroElement>();

            for (int i = 0; i < filtered.Count; i++)
            {
                var order = filtered[i];
                var produtosPedido = order.produtosRequisitados;
                for (int k = 0; k < produtosPedido.Count; k++)
                {
                    var produto = produtosPedido[k];
                    bool contains = false;
                    int index = 0;

                    for (int l = 0; l < produtos.Count; l++)
                    {
                        var target = produtos[l].produto.Id;
                        if (target == produto.produtoRequisitado.Id)
                        {
                            contains = true;
                            index = l;
                            break;
                        }
                    }

                    if (!contains)
                    {
                        relatorioFinanceiroElement newProduct = new relatorioFinanceiroElement()
                        {
                            produto = produto.produtoRequisitado,
                            quantidadeVendidas = produto.Quantity,
                            dinheiroGanho = (decimal)(produto.produtoRequisitado.Preco * (float)produto.Quantity)
                        };

                        var containKey = newProduct.diasVendas.ContainsKey(order.OrderDate.Date);

                        if (containKey)
                            newProduct.diasVendas[order.OrderDate.Date] += produto.Quantity;
                        else
                            newProduct.diasVendas.Add(order.OrderDate.Date, produto.Quantity);

                        produtos.Add(newProduct);
                    }
                    else
                    {
                        var target = produtos[index];

                        var containKey = target.diasVendas.ContainsKey(order.OrderDate.Date);

                        if (containKey)
                            target.diasVendas[order.OrderDate.Date] += produto.Quantity;
                        else
                            target.diasVendas.Add(order.OrderDate.Date, produto.Quantity);

                        target.dinheiroGanho += (decimal)(produto.produtoRequisitado.Preco * (float)produto.Quantity);
                        target.quantidadeVendidas += produto.Quantity;

                    }

                }

            }


            ViewBag.dtInicio = dtInicio.ToString("dd/MM/yyyy");
            ViewBag.dtFinal = dtFinal.ToString("dd/MM/yyyy");


            return View(produtos);
        }

        [HttpPost]
        public ActionResult GerarRelatorioFinanceiro(DateTime dtInicio, DateTime dtFinal, string clientState)
        {
            var orders = orderCollection.AsQueryable().ToList();
            var filtered = orders.FindAll(x => x.OrderDate.Date <= dtFinal && x.OrderDate.Date >= dtInicio && x.orderState == "Pago");

            List<relatorioFinanceiroElement> produtos = new List<relatorioFinanceiroElement>();

            for (int i = 0; i < filtered.Count; i++)
            {
                var order = filtered[i];
                var produtosPedido = order.produtosRequisitados;
                for (int k = 0; k < produtosPedido.Count; k++)
                {
                    var produto = produtosPedido[k];
                    bool contains = false;
                    int index = 0;

                    for (int l = 0; l < produtos.Count; l++)
                    {
                        var target = produtos[l].produto.Id;
                        if (target == produto.produtoRequisitado.Id)
                        {
                            contains = true;
                            index = l;
                            break;
                        }
                    }

                    if (!contains)
                    {
                        relatorioFinanceiroElement newProduct = new relatorioFinanceiroElement()
                        {
                            produto = produto.produtoRequisitado,
                            quantidadeVendidas = produto.Quantity,
                            dinheiroGanho = (decimal)(produto.produtoRequisitado.Preco * (float)produto.Quantity)
                        };

                        var containKey = newProduct.diasVendas.ContainsKey(order.OrderDate.Date);

                        if (containKey)
                            newProduct.diasVendas[order.OrderDate.Date] += newProduct.quantidadeVendidas;
                        else
                            newProduct.diasVendas.Add(order.OrderDate.Date, newProduct.quantidadeVendidas);

                        produtos.Add(newProduct);
                    }
                    else
                    {
                        var target = produtos[index];

                        target.quantidadeVendidas += produto.Quantity;

                        var containKey = target.diasVendas.ContainsKey(order.OrderDate.Date);

                        if (containKey)
                            target.diasVendas[order.OrderDate.Date] += target.quantidadeVendidas;
                        else
                            target.diasVendas.Add(order.OrderDate.Date, target.quantidadeVendidas);

                        target.dinheiroGanho += (decimal)(produto.produtoRequisitado.Preco * (float)produto.Quantity);
                    }

                }

            }


            ViewBag.dtInicio = dtInicio.ToString("dd/MM/yyyy");
            ViewBag.dtFinal = dtFinal.ToString("dd/MM/yyyy");


            return View(produtos);
        }

        public ActionResult DetalhesRelatorio(string vendas )
        {
            var a = vendas.Split(';');
            List<DataPoint> dataPoints = new List<DataPoint>();

            for (int i = 0; i < a.Length; i++)
            {
                DateTime.TryParse(a[i], out DateTime date);
                //int dia = date.Day;
                string dia = date.Day + "/" + date.Month;
                int.TryParse(a[i + 1], out int value);

                dataPoints.Add(new DataPoint(dia, value));
                i++;
            }

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(dataPoints);
            ViewBag.DataPoints = json;
            return View();
        }

        public string DictionaryToString(Dictionary<DateTime, int> vendas)
        {
            string s = string.Join(";", vendas.Select(x => x.Key + ";" + x.Value).ToArray());
            return s;
        }

        public void DownloadFinanceiroExcel(string horaGerado)
        {
            var collection = lastUserRequest;//db.GetCollection<EmployeeDetails>("EmployeeDetails").Find(new BsonDocument()).ToList();

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            Sheet.Cells["A1"].Value = "Dia Geração Relatório";
            Sheet.Cells["A1"].Value = "User ID";
            Sheet.Cells["B1"].Value = "Nome";
            Sheet.Cells["C1"].Value = "Sexo";
            Sheet.Cells["D1"].Value = "Email";
            Sheet.Cells["E1"].Value = "Data Criação";
            Sheet.Cells["F1"].Value = "Data Nascimento";

            int row = 2;
            Sheet.Cells[string.Format("A{0}", row)].Value = horaGerado;

            foreach (var item in collection)
            {
                Sheet.Cells[string.Format("B{0}", row)].Value = item.nome;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.sexo;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.email;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.dtCriacao;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.dtNascimento;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "RelatorioPedido.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        #endregion
    }

    public class relatorioFinanceiroElement
    {
        public ProductModel produto;
        public int quantidadeVendidas;
        public decimal dinheiroGanho;
        public Dictionary<DateTime, int> diasVendas = new Dictionary<DateTime, int>();
    }

    [DataContract]
    public class DataPoint
    {
        public DataPoint(string x, int y)
        {
            this.label = x;
            this.y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "dia")]
        public string label { get; set; }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "vendas")]
        public Nullable<int> y = null;
    }
}