
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;
using Game.Common.Rules;
using Game.Common.UI;

namespace Game.Functions
{
    public partial class ParseText
    {
        IGameMaster _game { get; }

        public ParseText(IGameMaster manager)
        {
            _game = manager;
        }

        [Function(nameof(ParseText))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = nameof(ParseText))] HttpRequestData req,
            [FromBody] Message bodyText,
            FunctionContext context)
        {
            ILogger logger = context.GetLogger(nameof(ParseText));
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

            IGameMaster gameMaster = executionContext.InstanceServices.GetService(typeof(IGameMaster)) as IGameMaster;
            UIMessage reply = gameMaster.ReplyToPlayer(text);

            var replyJson = new
            {
                message = reply.Content,
                from = reply.From?.Name ?? "System",
            };

            HttpResponseData res;
            res = req.CreateResponse(System.Net.HttpStatusCode.OK);
            res.WriteString(JsonConvert.SerializeObject(replyJson));
            return res;

        }
    }
}
