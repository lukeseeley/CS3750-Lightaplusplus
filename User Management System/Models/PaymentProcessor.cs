using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Lightaplusplus.Models
{
    static public class PaymentProcessor
    {
        static HttpClient client = new HttpClient();
        static private string key = "pk_test_51IOyT2GsHQoDsRIPnzHV6DcAWIGxPoonSWIvqWPWTyaY2EgtMj7ndIXotur6wwN81XtDmvtDjOF3RtfbY0o4vcss00WGYuhOQM";
        static private string secret = "sk_test_51IOyT2GsHQoDsRIPdQtyVoAL3hrGvoo5gv1Tig3Ab0hVmNqp9hagIzFdWXYNnpCm73KGvqboPVvHbhfuQ3btf0kc00NMejcBMV";
        static public async void processPayment(CreditCard card, double paymentAmount)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + secret);
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
                HttpResponseMessage responseMessage = await client.PostAsync(requestUrl, httpRequestMessage.Content);
                HttpContent content = responseMessage.Content;
                string message = await content.ReadAsStringAsync();
                var test = JsonConvert.SerializeObject(message);
                // Get Card Token
                var json = JsonConvert.DeserializeObject<dynamic>(message);
                var cardToken = json.id;
                // charge the card
                data = new Dictionary<string, string>();
                data.Add("source", cardToken.ToString());
                data.Add("amount", (Convert.ToInt32(paymentAmount * 100)).ToString()); // Gotta change it to amount in pennies (API Requirement)
                data.Add("currency", "USD");

                // Make API call
                httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Content = new FormUrlEncodedContent(data);
                responseMessage = await client.PostAsync(chargeUrl, httpRequestMessage.Content);
                content = responseMessage.Content;
                message = await content.ReadAsStringAsync();
                json = JsonConvert.DeserializeObject<dynamic>(message);
                string status = json.status.ToString();

                // update DB if successful
                if (status == "succeeded")
                {
                    
                }

                Console.WriteLine("test");
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine("An HTTP request exception occurred. {0}", exception.Message);
            }
        }
    }
}
