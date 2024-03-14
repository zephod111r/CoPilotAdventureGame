
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Game.Common.Manager;
using Game.Common.Rules;
using Newtonsoft.Json;
using System.Net;
using System;

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

            if(text == null)
            {
                text = bodyText.message;
            }

            if (text == null)
            {
                HttpResponseData responseBad = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                responseBad.WriteString("Invalid name, you can pass a message through a query or body of your request");
                return responseBad;
            }

            try {
                Message message = new Message();
                message.message = await _game.ReplyToPlayer(text);

                HttpResponseData res = null;
                res = req.CreateResponse(System.Net.HttpStatusCode.OK);
                res.Headers.Add("content-type", "application/json");
                res.WriteString(JsonConvert.SerializeObject(message));
                return res;
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
