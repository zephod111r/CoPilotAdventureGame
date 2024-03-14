using System.Threading.Tasks;
using Game.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Game.RuleBook;
using Game.Common.Configuration;
using Game.Common.Rules;
using Game.Common.Manager;
using Game.AI.OpenAI;
using Game.AI;
using Game.Common.UI;

namespace Game.Function
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var gameSettings = new GameSettings();
            var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(service => {
                service.AddSingleton<IGameManager, GameManager>();
                service.AddSingleton<IGameMaster, GameMaster>();
                service.AddSingleton(gameSettings);
                service.AddSingleton<IAIPlatform, OpenAIPlatform>();
                service.AddSingleton<IAppConfiguration, Configuration>();
                service.AddSingleton<IRuleBook, Game.RuleBook.RuleBook>();
                service.AddSingleton<IUserInterfaceManager, WebInterfaceManager>();
                service.AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                });
            }).Build();

            await host.RunAsync();
        }
    }
}
