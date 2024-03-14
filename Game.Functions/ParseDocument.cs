using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Game.Functions
{
    public static class ParseDocument
    {
        [Function(nameof(ParseDocument))]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = nameof(ParseDocument))] HttpRequestData req,
            FunctionContext context)
        {
            ILogger logger = context.GetLogger(nameof(ParseDocument));
            logger.LogInformation("C# HTTP trigger function processed a request.");
            // parse query parameter
            string name = req.Query["name"];

            HttpResponseData res = null;
            if (name == null)
            {
                res = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                res.WriteString("Invalid name, you can pass a name through a query or body of your request");
            }
            else
            {
                res = req.CreateResponse(System.Net.HttpStatusCode.OK);
                res.WriteString("Hello " + name);
            }

            return res;
        }
    }
}
