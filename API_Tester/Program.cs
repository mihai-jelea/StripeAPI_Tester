using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stripe;
using Newtonsoft.Json.Linq;

namespace API_Tester
{
    class Program
    {
        public static HttpClient httpClient { get; set; }
        static async Task Main(string[] args)
        {
            // Initiate the HTTP Client
            InitiateHttpClient();

            // Get Balance
            await GetBalance();

            httpClient.Dispose();
            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }

        #region Connection Setup
        // Initiate HTTP Client and set the required headers
        private static void InitiateHttpClient()
        {
            // This is the private key (should be stored elsewhere, in production
            string auth_key = "sk_test_rtifaQn2xu8zulrcUk83Amhm00jk80eVv7";

            // Instantiate a new HTTP Client
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth_key);

            Console.WriteLine("Program initiated...\n");
        }
        #endregion

        #region Get Balanace
        // Get Account Balance
        private static async Task GetBalance()
        {
            // Set the API url
            Uri balanceApiUrl = new Uri(@"https://api.stripe.com/v1/balance");

            // Send the request
            HttpResponseMessage response = await httpClient.GetAsync(balanceApiUrl);

            // Get the response
            var message = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();

            // Extract the contents
            Balance obj = JsonConvert.DeserializeObject<Balance>(message);
            double amount = obj.Available[0].Amount / 100.0;

            Console.WriteLine("+------------------------------------------------------+");
            Console.WriteLine("API url: " + balanceApiUrl.ToString());
            Console.WriteLine("Sent HTTP GET Request\n");
            Console.WriteLine("Received HTTP Response:");
            Console.WriteLine(response);
            Console.WriteLine("\n_____________________");
            Console.WriteLine("\nBalance is " + amount + " EUR");
            Console.WriteLine("_____________________");

            response.Dispose();
        }
        #endregion
    }
}
