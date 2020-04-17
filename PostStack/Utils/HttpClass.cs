using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PostStack.Utils
{
    public class HttpClass
    {
        public HttpClient ApiClient { get; set; }

        public HttpClass(IConfiguration configuration)
        {
            ApiClient = new HttpClient();

            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ApiClient.BaseAddress = new Uri(configuration.GetSection("BaseUrl")["companyApi"]);
        }
    }
}
