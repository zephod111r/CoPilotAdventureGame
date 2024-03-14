
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using Game.Common.Manager;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Game.Functions
{
    public class StartGame
    {
        IGameManager _game { get; }

        public StartGame(IGameManager manager)
        {
            _game = manager;
        }

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
                string content = await _game.Start();

                Message message = new Message();
                message.message = content;

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
