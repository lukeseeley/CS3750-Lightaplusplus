using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace Lightaplusplus.Models
{
    static public class PaymentProcessor
    {
        static readonly HttpClient client = new HttpClient();
        static private string key = "pk_test_51IOyT2GsHQoDsRIPnzHV6DcAWIGxPoonSWIvqWPWTyaY2EgtMj7ndIXotur6wwN81XtDmvtDjOF3RtfbY0o4vcss00WGYuhOQM";
        static private string secret = "sk_test_51IOyT2GsHQoDsRIPdQtyVoAL3hrGvoo5gv1Tig3Ab0hVmNqp9hagIzFdWXYNnpCm73KGvqboPVvHbhfuQ3btf0kc00NMejcBMV";
        static public bool processPayment(CreditCard card, double paymentAmount)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + secret);
            string cardurl = "https://api.stripe.com/v1/tokens";
            string postdata = "card[number]=" + card.cardNumber +"card[exp_month]=" + card.exp_month + "&card[exp_year]=" + card.exp_year + "&card[cvc]=" + card.cvc;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, cardurl);
            request.Content = new StringContent(postdata);
            client.SendAsync(request)
      .ContinueWith(responseTask =>
      {
          Console.WriteLine("Response: {0}", responseTask.Result);
      });

            return false;
        }
    }
}
