
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

namespace Game.Functions
{
    public partial class TextToSpeech
    {
        [Function(nameof(TextToSpeech))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = nameof(TextToSpeech))] HttpRequestData req,
            [FromBody] Message bodyText,
            FunctionContext context)
        {
            ILogger logger = context.GetLogger(nameof(TextToSpeech));
            logger.LogInformation("C# HTTP trigger function processed a request.");
            // parse query parameter
            string text = req.Query["message"];

            if (text == null)
            {
                text = bodyText.message;
            }

            if (text == null)
            {
                HttpResponseData responseBad = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                responseBad.WriteString("Invalid name, you can pass a message through a query or body of your request");
                return responseBad;
            }
            /*
            Game.AI.IAIPlatform ai = context.InstanceServices.GetService(typeof(Game.AI.IAIPlatform)) as Game.AI.IAIPlatform;
            byte[] mpegFile = await ai.TextToSpeech(text);

            IStorage storage = context.InstanceServices.GetService(typeof(IStorage)) as IStorage;
            System.Uri uri = await storage.Upload(text.GetHashCode().ToString() + ".mp3", mpegFile);

            var replyJson = new
            {
                audio = uri.AbsoluteUri
            };

            HttpResponseData res;
            res = req.CreateResponse(System.Net.HttpStatusCode.OK);
            res.Headers.Add("content-type", "application/json");
            res.WriteString(JsonConvert.SerializeObject(replyJson));
            
            return res;
            */


            HttpResponseData res;
            res = req.CreateResponse(System.Net.HttpStatusCode.OK);
            return res;

        }
    }
}
