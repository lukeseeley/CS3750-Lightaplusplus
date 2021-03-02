using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace Lightaplusplus.Models
{
    static public class PaymentProcessor
    {
        //static readonly HttpClient client = new HttpClient();
        static HttpClient client = new HttpClient();
        static private string key = "pk_test_51IOyT2GsHQoDsRIPnzHV6DcAWIGxPoonSWIvqWPWTyaY2EgtMj7ndIXotur6wwN81XtDmvtDjOF3RtfbY0o4vcss00WGYuhOQM";
        static private string secret = "sk_test_51IOyT2GsHQoDsRIPdQtyVoAL3hrGvoo5gv1Tig3Ab0hVmNqp9hagIzFdWXYNnpCm73KGvqboPVvHbhfuQ3btf0kc00NMejcBMV";
        static public async void processPayment(CreditCard card, double paymentAmount)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + secret);
            string requestUrl = "https://api.stripe.com/v1/tokens";
            string postdata = "card%5Bnumber%5D=" + card.cardNumber + "&card%5Bexp_month%5D=" + card.exp_month + "&card%5Bexp_year%5D=" + card.exp_year + "&card%5Bcvc%5D=" + card.cvc;



            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.Add("Content-type", "www-url-form-encoded");
            httpRequestMessage.Content = new StringContent(postdata);
            try
            {
                HttpResponseMessage responseMessage = await client.PostAsync(requestUrl, httpRequestMessage.Content);
                HttpContent content = responseMessage.Content;
                string message = await content.ReadAsStringAsync();
                Console.WriteLine("The output from thirdparty is: {0}", message);
                //RedirectToPage();
                Console.WriteLine("test");
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine("An HTTP request exception occurred. {0}", exception.Message);
            }
        }
    }
}
