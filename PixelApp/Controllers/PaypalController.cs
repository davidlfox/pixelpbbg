using PixelApp.Models;
using PixelApp.Services;
using PixelApp.Views.Paypal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PixelApp.Controllers
{
    [Authorize]
    public class PaypalController : BaseController
    {
        // GET: Paypal
        public ActionResult Index()
        {
            var vm = new PaypalViewModel();
            vm.UserId = this.UserContext.Id;
            vm.PaypalButtonId = Environment.GetEnvironmentVariable("PaypalButtonId");
            vm.PaypalButtonUrl = System.Configuration.ConfigurationManager.AppSettings["PaypalButtonUrl"];
            vm.PaypalImageUrl = System.Configuration.ConfigurationManager.AppSettings["PaypalImageUrl"];
            vm.PaypalPixelUrl = System.Configuration.ConfigurationManager.AppSettings["PaypalPixelUrl"];
            return View(vm);
        }

        public ActionResult Thanks()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpStatusCodeResult Receive()
        {
            //Store the IPN received from PayPal
            LogRequest(Request);

            //Fire and forget verification task
            Task.Run(() => VerifyTask(Request));

            //Reply back a 200 code
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void VerifyTask(HttpRequestBase ipnRequest)
        {
            var verificationResponse = string.Empty;

            try
            {
                var verificationRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["PaypalButtonUrl"]);

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";
                var param = Request.BinaryRead(ipnRequest.ContentLength);
                var strRequest = Encoding.ASCII.GetString(param);

                //Add cmd=_notify-validate to the payload
                strRequest = "cmd=_notify-validate&" + strRequest;
                verificationRequest.ContentLength = strRequest.Length;

                //Attach payload to the verification request
                var streamOut = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII);
                streamOut.Write(strRequest);
                streamOut.Close();

                //Send the request to PayPal and get the response
                var streamIn = new StreamReader(verificationRequest.GetResponse().GetResponseStream());
                verificationResponse = streamIn.ReadToEnd();
                streamIn.Close();

            }
            catch (Exception exception)
            {
                //Capture exception for manual investigation
            }

            ProcessVerificationResponse(verificationResponse);
        }


        private void LogRequest(HttpRequestBase request)
        {
            // Persist the request values into a database or temporary data store
        }

        private void ProcessVerificationResponse(string verificationResponse)
        {
            if (verificationResponse.Equals("VERIFIED"))
            {
                var req = Request.Form;
                var status = req["payment_status"];
                var product = req["item_number1"];
                var txn = req["txn_id"];
                var receiver = req["receiver_email"];
                var gross = decimal.Parse(req["mc_gross"]);
                var fee = decimal.Parse(req["payment_fee"]);
                var currency = req["mc_currency"];
                var quantity = int.Parse(req["quantity1"]);
                var userId = req["custom"];

                var db = new ApplicationDbContext();

                var user = db.Users.FirstOrDefault(x => x.Id == userId);

                if (user == null)
                {
                    throw new NotImplementedException();
                }

                // check that Txn_id has not been previously processed
                var transactionExists = db.PaypalTransactions.Any(x => x.TransactionId == txn);

                if (transactionExists)
                {
                    throw new NotImplementedException();
                }

                var payment = new PaypalTransaction
                {
                    Amount = gross,
                    Fee = fee,
                    PaymentStatus = status,
                    Product = product,
                    Quantity = quantity,
                    TransactionId = txn,
                    UserId = user.Id,
                };

                // log payment
                db.PaypalTransactions.Add(payment);

                // check that Payment_status=Completed
                // check that Receiver_email is your Primary PayPal email
                // check that Payment_amount/Payment_currency are correct
                if (status == "Completed" 
                    && receiver == Environment.GetEnvironmentVariable("PaypalReceiverEmail")
                    && currency == "USD")
                {
                    // process payment
                    var boostHours = quantity * 24;
                    user.HourlyResourceBoosts = (user.HourlyResourceBoosts ?? 0 ) + boostHours;
                    

                    var note = CommunicationService.CreateNotification(user.Id,
                        "Thanks for your boost purchase!!",
                        $"You've successfully purchased a 24-hour resource boost (Quantity: {quantity}). Each hour, you will get a HUGE boost in resource collection");

                    db.Notes.Add(note);

                }
                else
                {
                    var note = CommunicationService.CreateNotification(user.Id,
                        "There was a problem with your boost purchase",
                        $"We encountered a problem with your payment, and we're looking into it. status: {status}");

                    db.Notes.Add(note);
                }

                db.SaveChanges();
            }
            else if (verificationResponse.Equals("INVALID"))
            {
                //Log for manual investigation
            }
            else
            {
                //Log error
            }
        }
    }
}