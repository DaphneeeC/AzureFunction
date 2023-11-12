using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;

namespace Azure.Function
{
    public class TimerTrigger
    {
        [FunctionName("TimerTrigger")]
        public void Run([TimerTrigger("0 30 9 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
 
            // Initialize HttpClient
            using var httpClient = new HttpClient();
            // Define the request headers
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            httpClient.DefaultRequestHeaders.Host = "www.mims.com";
            httpClient.DefaultRequestHeaders.Pragma.Add(new NameValueHeaderValue("no-cache"));
            httpClient.DefaultRequestHeaders.Upgrade.Add(new ProductHeaderValue("1"));
 
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 Edg/119.0.0.0");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            //loop 26 times based on the alphabates
            List<Drug> drugObjs = new List<Drug>();
 
            for (int i = 0; i < 26; i++)
            {
                var url = "https://www.mims.com/malaysia/browse/alphabet/"+(char)('a' + i)+"?cat=drug"; //get drug url
                log.LogInformation(url);
                // Send an HTTP GET request to the URL
                var response = httpClient.GetStringAsync(url).Result;
                // Parse the HTML content
                var htmlDocument = new HtmlDocument();
 
                htmlDocument.LoadHtml(response);
                var drugs = htmlDocument.DocumentNode.SelectNodes("//li[@data-type]");
                foreach (var drug in drugs)
                {
                    bool available = true;
                    //log.LogInformation(drug.InnerHtml);
                    if(drug.SelectSingleNode(".//span[@class='discontinued discontinued-drug']").GetAttributes().Where(x=>x.Name == "title").FirstOrDefault() != null){
                        available = false;
                    }
                    string drugname = drug.SelectSingleNode(".//a").InnerText.Trim();
                    string drugtype = drug.SelectSingleNode(".//span[contains(@class, 'drug-type')]").InnerText.Trim();
                    Drug d = new(drugname,available,drugtype);
                    drugObjs.Add(d);
                }
            }
            //prevent escaping special char
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(drugObjs,serializeOptions);
            string responseMessage = json;
 
            log.LogInformation(json);
        }
    }
}
