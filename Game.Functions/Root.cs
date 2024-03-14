using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
            FunctionContext context)
        {
            ILogger logger = context.GetLogger(nameof(Root));
            logger.LogInformation("C# HTTP trigger function processed a request.");

            if (nameOfRoot == null)
            {
                nameOfRoot = root();
            }

            string path = string.Concat(@nameOfRoot, @"\index.html");

            try
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Body = stream;
                response.Headers.Add("content-type", "text/html");
                return response;
            }
            catch(Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.WriteString(ex.Message);
                response.Headers.Add("content-type", "text/plain");
                return response;
            }
        }
    }
}