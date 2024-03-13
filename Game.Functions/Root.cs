using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Game.Functions
{
    public static class Root
    {
        

        [Function(nameof(Root))]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "/")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            FileStream stream = new FileStream(@"wwwroot\index.html", FileMode.Open, FileAccess.Read);
            response.Body = stream;
            response.Headers.Add("content-type", "text/html");
            return response;
        }
    }
}