
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;
using Game.Common.Rules;
using Game.Common.UI;
using Microsoft.Extensions.Logging;
using System.Linq;

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

            IGameMaster gameMaster = context.InstanceServices.GetService(typeof(IGameMaster)) as IGameMaster;
            UIMessage[] replyMessages = await gameMaster.ReplyToPlayer(text);

            Message[] replyJson = replyMessages.Select(uiMessage => new Message
            {
                message = uiMessage.Type != UIMessageType.Image && uiMessage.Type != UIMessageType.Audio ? uiMessage.Content : null,
                from = uiMessage.From?.Name ?? "System",
                image = uiMessage.Type == UIMessageType.Image ? uiMessage.Content : null,
                audio = uiMessage.Type == UIMessageType.Audio ? $"/GetFile/{uiMessage.Content}" : null
            }).ToArray();

            HttpResponseData res;
            res = req.CreateResponse(System.Net.HttpStatusCode.OK);
            res.WriteString(JsonConvert.SerializeObject(replyJson));
            return res;

        }
    }
}
