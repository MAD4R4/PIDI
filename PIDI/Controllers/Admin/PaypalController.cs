using PayPal.Api;
using PIDI.Models.Commom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PIDI.Controllers.Admin
{
    public class PaypalController : Controller
    {

        List<PedidoElementModel> cartItems = new List<PedidoElementModel>();

        public PaypalController()
        {
            cartItems = (List<PedidoElementModel>)PIDI.App_Start.SessionManager.ReturnSessionObject("items");

            
        }
        public ActionResult PaymentWithPaypal(string Cancel = null)
    {
        //getting the apiContext
        APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal
                //Payer Id will be returned when payment proceeds or click to pay
                string payerId = Request.Params["PayerID"];

            if (string.IsNullOrEmpty(payerId))
            {
                //this section will be executed first because PayerID doesn't exist
                //it is returned by the create function call of the payment class

                // Creating a payment
                // baseURL is the url on which paypal sendsback the data.
                string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                            "/Paypal";

                //here we are generating guid for storing the paymentID received in session
                //which will be used in the payment execution

                var guid = Convert.ToString((new Random()).Next(100000));

                //CreatePayment function gives us the payment approval url
                //on which payer is redirected for paypal account payment

                var createdPayment = this.CreatePayment(apiContext, baseURI + "/Payment{0}?guid=" + guid);

                //get links returned from paypal in response to Create function call

                var links = createdPayment.links.GetEnumerator();

                string paypalRedirectUrl = null;

                while (links.MoveNext())
                {
                    Links lnk = links.Current;

                    if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                    {
                        //saving the payapalredirect URL to which user will be redirected for payment
                        paypalRedirectUrl = lnk.href;
                    }
                }

                // saving the paymentID in the key guid
                Session.Add(guid, createdPayment.id);

                return Redirect(paypalRedirectUrl);
            }
            else
            {

                // This function exectues after receving all parameters for the payment

                var guid = Request.Params["guid"];

                var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                //If executed payment failed then we will show payment failure message to user
                if (executedPayment.state.ToLower() != "approved")
                {
                    return View(PaymentFailure());
                }
            }
        }
            catch (Exception ex)
            {
                return View("FailureView");
    }

            //on successful payment, show success page to user.
            return View("SuccessView");
    }

    private PayPal.Api.Payment payment;
    private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
    {
        var paymentExecution = new PaymentExecution() { payer_id = payerId };
        this.payment = new Payment() { id = paymentId };
        return this.payment.Execute(apiContext, paymentExecution);
    }

    private Payment CreatePayment(APIContext apiContext, string redirectUrl)
    {
            var subTotal = 0f;
            //create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };

            for (int i = 0; i < cartItems.Count; i++)
            {
                var target = cartItems[i];
                string productname = target.produtoRequisitado.ProductName.ToString();
                string productprice = target.produtoRequisitado.Preco.ToString();
                string qtd = target.Quantity.ToString();
                string skuname = target.produtoRequisitado.Id.ToString();

                subTotal += target.produtoRequisitado.Preco * target.Quantity;

                itemList.items.Add(new Item()
                {
                    name = productname,
                    currency = "USD",
                    price = productprice,
                    quantity = qtd,
                    sku = "sku"
                });
            }

            string taxa = "3";
            string frete = "15";
            //subTotal = 1;
            string valorTotal = (subTotal + float.Parse(taxa) + float.Parse(frete)).ToString();

            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var cancelURL = string.Format(redirectUrl, "Failure");
            var sucessURL = string.Format(redirectUrl, "Sucess");

            var redirUrls = new RedirectUrls()
        {
            cancel_url = cancelURL + "&Cancel=true",
            return_url = sucessURL
            };

        // Adding Tax, shipping and Subtotal details
        var details = new Details()
        {
            tax = taxa,
            shipping = frete,
            subtotal = subTotal.ToString()
        };

        //Final amount with details
        var amount = new Amount()
        {
            currency = "USD",
            total = valorTotal, // Total must be equal to sum of tax, shipping and subtotal.
            details = details
        };

        var transactionList = new List<Transaction>();
        // Adding description about the transaction
        transactionList.Add(new Transaction()
        {
            description = "Transaction description",
            invoice_number = "your generated invoice number", //Generate an Invoice No
            amount = amount,
            item_list = itemList
        });


        this.payment = new Payment()
        {
            intent = "sale",
            payer = payer,
            transactions = transactionList,
            redirect_urls = redirUrls
        };

            PIDI.App_Start.SessionManager.FreeSession("items");

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);
    }

        public ActionResult PaymentSucess()
        {
            return View();
        }

        public ActionResult PaymentFailure()
        {
            return View();
        }


    }











    //public class PaypalController : Controller
    //{
    //    public ActionResult PaymentWithPaypal(string Cancel = null)
    //    {
    //        //getting the apiContext as earlier
    //        APIContext apiContext = PaypalConfiguration.GetAPIContext();

    //        try
    //        {
    //            string payerId = Request.Params["PayerID"];

    //            if (string.IsNullOrEmpty(payerId))
    //            {
    //                //this section will be executed first because PayerID doesn't exist
    //                //it is returned by the create function call of the payment class

    //                // Creating a payment
    //                // baseURL is the url on which paypal sendsback the data.
    //                // So we have provided URL of this controller only
    //                string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
    //                            "/Home/PaymentWithPayPal?";

    //                //guid we are generating for storing the paymentID received in session
    //                //after calling the create function and it is used in the payment execution

    //                var guid = Convert.ToString((new Random()).Next(100000));

    //                //CreatePayment function gives us the payment approval url
    //                //on which payer is redirected for paypal account payment

    //                var createdPayment = this.CreatePayment(apiContext , baseURI + "guid=" + guid);

    //                //get links returned from paypal in response to Create function call

    //                var links = createdPayment.links.GetEnumerator();

    //                string paypalRedirectUrl = null;

    //                while (links.MoveNext())
    //                {
    //                    Links lnk = links.Current;

    //                    if (lnk.rel.ToLower().Trim().Equals("approval_url"))
    //                    {
    //                        //saving the payapalredirect URL to which user will be redirected for payment
    //                        paypalRedirectUrl = lnk.href;
    //                    }
    //                }

    //                // saving the paymentID in the key guid
    //                Session.Add(guid, createdPayment.id);

    //                return Redirect(paypalRedirectUrl);
    //            }
    //            else
    //            {

    //                // This function exectues after receving all parameters for the payment

    //                var guid = Request.Params["guid"];

    //                var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

    //                //If executed payment failed then we will show payment failure message to user
    //                if (executedPayment.state.ToLower() != "approved")
    //                {
    //                    return View(PaymentFailure());
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return View(PaymentFailure());
    //        }

    //        //on successful payment, show success page to user.
    //        return View(PaymentSucess());
    //    }

    //    private PayPal.Api.Payment payment;
    //    private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
    //    {
    //        var paymentExecution = new PaymentExecution()
    //        {
    //            payer_id = payerId
    //        };
    //        this.payment = new Payment()
    //        {
    //            id = paymentId
    //        };
    //        return this.payment.Execute(apiContext, paymentExecution);
    //    }
    //    private Payment CreatePayment(APIContext apiContext, string redirectUrl)
    //    {
    //        //create itemlist and add item objects to it  
    //        var itemList = new ItemList()
    //        {
    //            items = new List<Item>()
    //        };

    //var cartItems = (List<PedidoElementModel>)PIDI.App_Start.SessionManager.ReturnSessionObject("items");

    //        for (int i = 0; i<cartItems.Count; i++)
    //        {
    //            var target = cartItems[i];
    //var product = PIDI.Controllers.Admin.ProductController.Instance.GetProduct(target.ProductId);
    //itemList.items.Add(new Item()
    //{
    //    name = product.ProductName,
    //                currency = "BRL",
    //                price = product.Preco.ToString(),
    //                quantity = target.Quantity.ToString(),
    //                sku = product.Id.ToString()
    //            });
    //        }
    //        //Adding Item Details like name, currency, price etc  
    //        //itemList.items.Add(new Item()
    //        //{
    //        //    name = "ItemDeTeste",
    //        //    currency = "BRL",
    //        //    price = "1",
    //        //    quantity = "1",
    //        //    sku = "sku"
    //        //});
    //        var payer = new Payer()
    //        {
    //            payment_method = "paypal"
    //        };
    //        // Configure Redirect Urls here with RedirectUrls object  
    //        var redirUrls = new RedirectUrls()
    //        {
    //            cancel_url = redirectUrl + "&Cancel=true",
    //            return_url = redirectUrl
    //        };
    //        // Adding Tax, shipping and Subtotal details  
    //        var details = new Details()
    //        {
    //            tax = "1",
    //            shipping = "1",
    //            subtotal = "1"
    //        };
    //        //Final amount with details  
    //        var amount = new Amount()
    //        {
    //            currency = "BRL",
    //            total = "3", // Total must be equal to sum of tax, shipping and subtotal.  
    //            details = details
    //        };
    //        var transactionList = new List<Transaction>();
    //        // Adding description about the transaction  
    //        transactionList.Add(new Transaction()
    //        {
    //            description = "Transaction description",
    //            invoice_number = "your generated invoice number", //Generate an Invoice No  
    //            amount = amount,
    //            item_list = itemList
    //        });
    //        this.payment = new Payment()
    //        {
    //            intent = "sale",
    //            payer = payer,
    //            transactions = transactionList,
    //            redirect_urls = redirUrls
    //        };
    //        // Create a payment using a APIContext  
    //        return this.payment.Create(apiContext);
    //    }

    //    public ActionResult PaymentSucess()
    //    {
    //        return View();
    //    }

    //    public ActionResult PaymentFailure()
    //    {
    //        return View();
    //    }
    //}
}