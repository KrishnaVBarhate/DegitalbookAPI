using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace AuthorService.Models
{
    public partial class Purchase
    {
        public int PurchaseId { get; set; }
        public string? EmailId { get; set; }
        public int? BookId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? PaymentMode { get; set; }

        public virtual Book? Book { get; set; }

        public Boolean callPaymentAuzreFunPost()
        {
            bool retVal = false;
            string URL = "https://purchasefunctionapp20220920163151.azurewebsites.net/api/PurchaseFunction";
            string urlParameters = "?code=6QCbEKox0MDx88Vdgf9YrzflOAAuDmozmFCdZM--aYPGAzFufQa7bQ==";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            var myJson = "{ \"emailID\" : \"" + EmailId + "\"," +
                            "\"bookID\" : \"" + BookId + "\"," +
                            "\"paymentMode\" : \"" + PaymentMode + "\"}";

            // List data response.
            HttpResponseMessage response = client.PostAsync(urlParameters, new StringContent(myJson, Encoding.UTF8, "application/json")).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                //response.Content.ReadAsStringAsync().Result;
                retVal = true;
            }
            else
            {
                //res = response.Content.ReadAsStringAsync().Result;
            }
            client.Dispose();
            return retVal;
        }

    }
}
