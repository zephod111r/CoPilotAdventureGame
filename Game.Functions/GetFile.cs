
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;
using Game.Common.Rules;
using Game.Common.UI;
using Microsoft.Extensions.Logging;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Azure;
using Game.Common.Storage;
using System.IO;

namespace Game.Functions
{
    public partial class GetFile
    {
        [Function(nameof(GetFile))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(GetFile) + "/{filename}")] HttpRequestData req,
            string filename,
            FunctionContext context)
        {
            ILogger logger = context.GetLogger(nameof(GetFile));
            logger.LogInformation("C# HTTP trigger function processed a request.");
            // parse query parameter

            if (filename == null)
            {
                HttpResponseData responseBad = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                responseBad.WriteString("Invalid name, you can pass a message through a query or body of your request");
                return responseBad;
            }

            IStorage storage = context.InstanceServices.GetService(typeof(IStorage)) as IStorage;
            Stream stream = await storage.GetFile(filename);

            HttpResponseData res;
            res = req.CreateResponse(System.Net.HttpStatusCode.OK);
            res.Headers.Add("content-type", "audio/mpeg");
            res.Body = stream;
            return res;

        }
    }
}
