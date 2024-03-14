using Game.AI;
using Game.AI.OpenAI;
using Game.Common.Configuration;
using Game.Common.Manager;
using Game.Common.Rules;
using Game.Common.UI;
using Game.RuleBook;
using Game.TextUI.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Game.Functions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var gameSettings = new GameSettings();

            var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                // Use 'settings.json' as a configuration source
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile("local.appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices(services =>
            {
                services.AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .AddSingleton<IAIPlatform, OpenAIPlatform>()
                .AddSingleton<IAppConfiguration, HostedConfiguration>()
                .AddSingleton<IRuleBook, RuleBook.RuleBook>()
                .AddSingleton<IUserInterfaceManager, WebUserInterfaceManager>()
                .AddSingleton<IGameMaster, GameMaster>()
                .AddHostedService<GameManager>()
                .AddSingleton(gameSettings);
            })
            .Build();

            await host.RunAsync();
        }
    }
}
