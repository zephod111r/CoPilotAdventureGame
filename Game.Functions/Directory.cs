using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace Game.Functions
{
    public static class Directory
    {
        [Function(nameof(Directory))]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = nameof(Directory))] HttpRequestData req,
            FunctionContext context)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);

            Dictionary<string, string> fileMap = new Dictionary<string, string>();

            DirectoryInfo info = new DirectoryInfo(".");
            foreach (var item in info.EnumerateDirectories())
            {
                foreach (var item1 in item.EnumerateFiles())
                {
                    fileMap.Add(item1.FullName, item.FullName);
                }
            };

            string jsonMap = JsonConvert.SerializeObject(fileMap);
            byte[] bytearray = Encoding.UTF8.GetBytes(jsonMap);
            response.Body = new MemoryStream(bytearray);
            response.Headers.Add("content-type", "text/html");
            return response;
        }
    }
}