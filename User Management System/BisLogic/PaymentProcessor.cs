using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Lightaplusplus.Models;

namespace Lightaplusplus.BisLogic
{
    static public class PaymentProcessor
    {
        /// <summary>
        /// Client used to make requests
        /// </summary>
        static HttpClient client = new HttpClient();
        /// <summary>
        /// Stripe Secret
        /// </summary>
        static private string secret = "sk_test_51IOyT2GsHQoDsRIPdQtyVoAL3hrGvoo5gv1Tig3Ab0hVmNqp9hagIzFdWXYNnpCm73KGvqboPVvHbhfuQ3btf0kc00NMejcBMV";

        /// <summary>
        /// processPayment takes a CreditCard object and a double (the amount being paid) as arguments, and returns whether the charge was successful or not.
        /// </summary>
        /// <param name="card">CreditCard object, all fields are required</param>
        /// <param name="paymentAmount">The amount being paid, as a double. (100.00)</param>
        /// <returns>succeeded if the charge was successful, not successful if there was an error during processing</returns>
        static public string processPayment(CreditCard card, double paymentAmount)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", secret);
            string requestUrl = "https://api.stripe.com/v1/tokens";
            string chargeUrl = "https://api.stripe.com/v1/charges";
            string postdata = "card%5Bnumber%5D=" + card.cardNumber + "&card%5Bexp_month%5D=" + card.exp_month + "&card%5Bexp_year%5D=" + card.exp_year + "&card%5Bcvc%5D=" + card.cvc;

            IDictionary<string, string> data = new Dictionary<string, string>();

            data.Add("card[number]", card.cardNumber);
            data.Add("card[exp_month]", card.exp_month);
            data.Add("card[exp_year]", card.exp_year);
            data.Add("card[cvc]", card.cvc);

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Content = new FormUrlEncodedContent(data);
            try
            {
                HttpResponseMessage responseMessage = client.PostAsync(requestUrl, httpRequestMessage.Content).GetAwaiter().GetResult();
                HttpContent content = responseMessage.Content;
                string message = content.ReadAsStringAsync().GetAwaiter().GetResult();
                var test = JsonConvert.SerializeObject(message);
                // Get Card Token
                var json = JsonConvert.DeserializeObject<dynamic>(message);
                if (message.Contains("invalid"))
                {
                    return "error";
                }
                var cardToken = json.id;
                // charge the card
                data = new Dictionary<string, string>();
                data.Add("source", cardToken.ToString());
                data.Add("amount", (Convert.ToInt32(paymentAmount * 100)).ToString()); // Gotta change it to amount in pennies (API Requirement)
                data.Add("currency", "USD");

                // Make API call
                httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Content = new FormUrlEncodedContent(data);
                responseMessage = client.PostAsync(chargeUrl, httpRequestMessage.Content).GetAwaiter().GetResult();
                content = responseMessage.Content;
                message = content.ReadAsStringAsync().GetAwaiter().GetResult();
                json = JsonConvert.DeserializeObject<dynamic>(message);
                if (message.Contains("error"))
                {
                    return "Error";
                }
                string status = json.status.ToString();

                // return whether is was successful or not. "succeeded" returns if it was successful
                return status;
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine("An HTTP request exception occurred. {0}", exception.Message);
                return "error";
            }
        }
    }
}
