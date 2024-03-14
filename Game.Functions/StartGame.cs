
using Game.Common.Manager;
using Game.Common.Rules;
using Game.Common.UI;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Game.Functions
{
    public class StartGame
    {
        [Function(nameof(StartGame))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(StartGame))] HttpRequestData req,
            FunctionContext context)
        {
            ILogger logger = context.GetLogger(nameof(StartGame));
            logger.LogInformation("C# HTTP trigger function processed a request.");
            // parse query parameter
            try
            {
                IGameMaster gameMaster = context.InstanceServices.GetService(typeof(IGameMaster)) as IGameMaster;
                UIMessage welcomeMessage = await gameMaster.StartGame();
                
                var replyJson = new Message
                {
                    message = welcomeMessage.Content,
                    from = welcomeMessage.From?.Name ?? "System",
                };

                HttpResponseData res = null;
                res = req.CreateResponse(HttpStatusCode.OK);
                res.Headers.Add("content-type", "application/json");
                res.WriteString(JsonConvert.SerializeObject(replyJson));
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
