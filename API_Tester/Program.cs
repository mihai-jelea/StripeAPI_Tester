using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stripe;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Collections.Generic;

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
            //await GetBalance();

            // Get a Charge
            //await GetCharge();

            // Get all Charges
            //await GetAllCharges();

            // Update a Charge Description
            await UpdateChargeDescription();

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

            Console.WriteLine("Program initiated...");
        }
        #endregion

        #region Get Balance
        // Get Account Balance
        private static async Task GetBalance()
        {
            // Set the API url
            Uri balanceApiUrl = new Uri(@"https://api.stripe.com/v1/balance");

            // Send the request
            HttpResponseMessage response = await httpClient.GetAsync(balanceApiUrl);

            // Get the response and extract the contents
            Balance obj = JsonConvert.DeserializeObject<Balance>(response.Content.ReadAsStringAsync().Result);
            double amount = obj.Available[0].Amount / 100.0;

            Console.WriteLine("\n\n+----------------------------------/// Get Balance ///-----------------------------------------+");
            Console.WriteLine("+----------------------------------------------------------------------------------------------+");
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

        #region Retrieve a Charge
        // Get a Charge
        private static async Task GetCharge()
        {
            // Store the id of the charge
            string chargeId = "ch_1Fy0C8IH6I6LxPXfb1jZAXcr";

            // Set the endpoint
            Uri chargesApiUrl = new Uri(@"https://api.stripe.com/v1/charges/" + chargeId);

            // Send the request
            HttpResponseMessage response = await httpClient.GetAsync(chargesApiUrl);

            // Get the response and extract the contents
            var obj = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
            double chargeAmount = (double)((JObject)obj)["amount"] / 100;
            string chargeDescription = (string)((JObject)obj)["description"];
            string chargeCurrency = (string)((JObject)obj)["currency"];
            string chargePostalCode = (string)((JObject)obj)["billing_details"]["address"]["postal_code"];
            string chargeCardType = (string)((JObject)obj)["payment_method_details"]["card"]["brand"];
            string chargeStatus = (string)((JObject)obj)["status"];

            Console.WriteLine("\n\n+----------------------------------/// Retrieve a Charge ///-----------------------------------+");
            Console.WriteLine("+----------------------------------------------------------------------------------------------+");
            Console.WriteLine("API url: " + chargesApiUrl.ToString());
            Console.WriteLine("Sent HTTP GET Request for charge: " + chargeId.ToString() + "\n");
            Console.WriteLine("Received HTTP Response:");
            Console.WriteLine(response);
            Console.WriteLine("\n_____________________");
            Console.WriteLine("\nCharge details: ");
            Console.WriteLine("   amount:       " + chargeAmount);
            Console.WriteLine("   currency:     " + chargeCurrency);
            Console.WriteLine("   description:  " + chargeDescription);
            Console.WriteLine("   postal code:  " + chargePostalCode);
            Console.WriteLine("   card type:    " + chargeCardType);
            Console.WriteLine("   status:       " + chargeStatus);
            Console.WriteLine("_____________________");

            response.Dispose();
        }
        #endregion

        #region List all the charges
        private static async Task GetAllCharges()
        {
            // Set the endpoint
            string url = "https://api.stripe.com/v1/charges?limit={0}";
            // Set the limit (max 100)
            int limit = 30;
            url = String.Format(url, limit);

            Uri chargesApiUrl = new Uri(url);

            // Send the request
            HttpResponseMessage response = await httpClient.GetAsync(chargesApiUrl);

            // Get the response and extract the contents
            var obj = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
            List<Charge> charges = JsonConvert.DeserializeObject<List<Charge>>(((JObject)obj)["data"].ToString());

            Console.WriteLine("\n\n+----------------------------------/// List all Charges ///-----------------------------------+");
            Console.WriteLine("+----------------------------------------------------------------------------------------------+");
            Console.WriteLine("API url: " + chargesApiUrl.ToString());
            Console.WriteLine("Sent HTTP GET Request\n");
            Console.WriteLine("Received HTTP Response:");
            Console.WriteLine(response);
            Console.WriteLine("\n_____________________");
            Console.WriteLine("ID                                 AMOUNT       STATUS          DESCRIPTION         ");

            // Print out the details of all the charges
            foreach (Charge charge in charges)
            {
                double chargeAmount = charge.Amount / 100.0;
                Console.WriteLine(charge.Id + "         " + chargeAmount + "        " + charge.Status + "       " + charge.Description);
            }

            Console.WriteLine("_____________________");

            response.Dispose();
        }
        #endregion

        #region UpdateChargeDescription
        // Update a Charge Description
        private static async Task UpdateChargeDescription()
        {
            // Store the id of the charge
            string chargeId = "ch_1FiJ6YIH6I6LxPXfQnP59vda";

            // Set the new description
            string newDescription = "Description updated programatically from .NET | " + DateTime.Now.ToString();

            // Set the Charges API endpoint for updating the description field of a Charge
            string chargesApiUrl = "https://api.stripe.com/v1/charges/{0}?description={1}";
            chargesApiUrl = String.Format(chargesApiUrl, chargeId, newDescription);

            // Create a new HTTP request
            HttpRequestMessage request = new HttpRequestMessage();
            
            // Set the endpoint and the HTTP method
            request.RequestUri = new Uri(chargesApiUrl);
            request.Method = HttpMethod.Post;

            // Send the request
            HttpResponseMessage response = await httpClient.SendAsync(request);

            // Get the response and extract the contents
            var obj = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
            string updatedDescription = ((JObject)obj)["description"].ToString();


            Console.WriteLine("\n\n+----------------------------------/// Update a Charge ///-----------------------------------+");
            Console.WriteLine("+----------------------------------------------------------------------------------------------+");
            Console.WriteLine("API url: " + request.RequestUri);
            Console.WriteLine("Sent HTTP POST Request for charge: " + chargeId.ToString());
            Console.WriteLine("Received HTTP Response:");
            Console.WriteLine(response);
            Console.WriteLine("\n_____________________");
            Console.WriteLine("\nCharge " + chargeId + " was updated successfully.");
            Console.WriteLine("   updated description:       " + updatedDescription);
            Console.WriteLine("_____________________");

            request.Dispose();
            response.Dispose();
        }
        #endregion


    }
}
