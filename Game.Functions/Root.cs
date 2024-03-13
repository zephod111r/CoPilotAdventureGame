using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Game.Functions
{
    public static class Root
    {
        static string nameOfRoot { get; set; }

        static string root() {
            DirectoryInfo info = new DirectoryInfo(".");
            foreach (var item in info.EnumerateDirectories())
            {
                if(item.Name == "wwwroot")
                {
                    return item.FullName;
                }
            };
            return ".";
        }

        [Function(nameof(Root))]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "/")] HttpRequestData req,
            ExecutionContext context)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);

            if(nameOfRoot == null)
            {
                nameOfRoot = root();
            }

            string path = string.Concat(@nameOfRoot, @"\index.html");

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            response.Body = stream;
            response.Headers.Add("content-type", "text/html");
            return response;
        }
    }
}