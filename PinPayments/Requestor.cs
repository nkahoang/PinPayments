using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

namespace PinPayments
{
    public class Requestor
    {
        PinPaymentsOptions _options;
        HttpClient _httpClient;
        public Requestor(PinPaymentsOptions option)
        {
			_options = option;

			_httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue(
                    "Basic", 
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ApiKey}:"))
                );
        }
        
        public string GetString(string url)
        {
            return GetWebRequest(url, "GET","").Result;
        }

        public string PostString(string url, string parameters)
        {
            return GetWebRequest(url, "POST", parameters).Result;
        }

        public string PutString(string url, string parameters)
        {
            return GetWebRequest(url, "PUT", parameters).Result;
        }

        public string Delete(string url, string apiKey = null)
        {
            return GetWebRequest(url, "DELETE", apiKey).Result;
        }

        protected virtual async Task<string> GetWebRequest(string url, string method, string postData)
        {

            switch (method.ToUpper())
            {
                case "GET":
                    return await _httpClient.GetStringAsync(url);
				case "POST":
				case "PUT":
                    var content = new StringContent(postData,
                                                    Encoding.UTF8,
                                                    "application/x-www-form-urlencoded");

                    var response = method.ToUpper() == "POST" ?
                        await _httpClient.PostAsync(url, content) :
                        await _httpClient.PutAsync(url, content);

                    var responseContent = await response.Content.ReadAsStringAsync();

                    return responseContent;
				case "DELETE":
                    await _httpClient.DeleteAsync(url);
                    return string.Empty;
            }

            throw new Exception($"Unknown {method} method");
        }
    }
}
