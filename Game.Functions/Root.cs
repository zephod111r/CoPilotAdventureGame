using Game.Common.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Game.Functions
{
    public static class Root
    {
        static string root()
        {
            DirectoryInfo info = new DirectoryInfo(".");
            foreach (var item in info.EnumerateDirectories())
            {
                if (item.Name == "wwwroot")
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
           
            IStorage storage = context.InstanceServices.GetService(typeof(IStorage)) as IStorage;

            string nameOfRoot = root();
            string path = string.Concat(@nameOfRoot, @"\index.html");

            try
            {
                //FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

                Stream stream = await storage.LoadStatic("index.html");
                
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Body = stream;
                response.Headers.Add("content-type", "text/html");
                return response;
            }
            catch (Exception ex)
            {
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.WriteString(ex.Message);
                response.Headers.Add("content-type", "text/plain");
                return response;
            }
        }
    }
}