using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Game.Functions
{
    public class ParseText
    {
        private readonly ILogger _logger;

        public ParseText(ILogger<ParseText> logger)
        {
            _logger = logger;
        }

        [Function("ParseText")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.Query["name"];

            if (name == null)
            {
                // Get request body
                dynamic data = await req.ReadFromJsonAsync<object>();
                name = data?.name;
            }

            return (name == null)
            ? new BadRequestObjectResult("Invalid name, you can pass a name through a query or body of your request")
            : new OkObjectResult("Hello " + name);
        }
    }
}
