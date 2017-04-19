using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace rest_sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Options options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                MainAsync(options).Wait();
            }

            // Keep the window open when running in debugger
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        static async Task MainAsync(Options options)
        {
            if (!options.IsValid())
            {
                Console.WriteLine(options.GetUsage());
                return;
            }

            string token = await GetAccessToken(options.GetQualifiedScopes());
            if (options.OutputToken)
            {
                Output.WriteLine(Output.Info, "Access token: {0}", token);
            }

            await DoRequest(options, token);
        }

        static async Task<string> GetAccessToken(string[] scopes)
        {
            PublicClientApplication authClient = new PublicClientApplication(ConfigurationManager.AppSettings.Get("applicationId"));

            try
            {
                var result = await authClient.AcquireTokenAsync(scopes);
                return result.Token;
            }
            catch(MsalException ex)
            {
                Output.WriteLine(Output.Error, "Could not acquire access token: Error code: {0}, Error message: ", ex.ErrorCode, ex.Message);
                return string.Empty;
            }
        }

        static async Task DoRequest(Options options, string token)
        {
            using (var httpClient = new HttpClient())
            {
                // Summary stats for paging
                int numRequests = 0;
                List<int> resultsPerPage = new List<int>();
                List<bool> successPerPage = new List<bool>();

                bool doPaging = options.PageResults;

                // Handle escaping characters in URL
                UriBuilder builder = new UriBuilder(options.RequestUrl);
                Output.WriteLine(Output.Info, "Sending {0} request to {1}", options.Method, builder.Uri.ToString());
                Uri requestUri = builder.Uri;

                do
                {
                    var request = new HttpRequestMessage(new HttpMethod(options.Method), requestUri);

                    // Headers
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    request.Headers.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("rest-sender", "1.0"));
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var result = await httpClient.SendAsync(request);
                    string response = await result.Content.ReadAsStringAsync();

                    numRequests++;
                    successPerPage.Add(result.IsSuccessStatusCode);

                    Output.WriteLine("");
                    Output.WriteLine(result.IsSuccessStatusCode ? Output.Success : Output.Error, "{0} - {1}", result.StatusCode, result.ReasonPhrase);
                    Output.WriteLine(Output.Info, "BEGIN RESPONSE:");
                    Output.WriteLine(PrettyPrint(response));
                    Output.WriteLine(Output.Info, "END RESPONSE");

                    JObject responseJson = JObject.Parse(response);

                    // Is this a collection?
                    JArray collection = (JArray)responseJson["value"];
                    if (result.IsSuccessStatusCode && collection != null)
                    {
                        // Output stats about collection
                        Output.WriteLine(Output.Info, "Request returned {0} entities.", collection.Count);
                        resultsPerPage.Add(collection.Count);
                        if (responseJson["@odata.nextLink"] != null)
                        {
                            Output.WriteLine(Output.Info, "Next page URL: {0}", responseJson["@odata.nextLink"].ToString());
                            requestUri = new Uri(responseJson["@odata.nextLink"].ToString());
                        }
                        else
                        {
                            doPaging = false;
                        }
                    }
                    else
                    {
                        resultsPerPage.Add(0);
                    }
                }
                while (doPaging);

                if (options.PageResults)
                {
                    // Do paging summary
                    Output.WriteLine("");
                    Output.WriteLine("PAGING SUMMARY:");
                    Output.WriteLine(Output.Info, "Number of pages: {0}", numRequests);
                    for (int i = 0; i < numRequests; i++)
                    {
                        Output.Write(Output.Info, "Request #{0} returned: ", i+1);
                        if (successPerPage[i])
                        {
                            Output.WriteLine(Output.Success, "{0} entities", resultsPerPage[i]);
                        }
                        else
                        {
                            Output.WriteLine(Output.Error, "ERROR");
                        }
                    }
                }
            }
        }

        static string PrettyPrint(string rawJson)
        {
            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(rawJson);
                return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
