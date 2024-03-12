using Game.AI;
using Game.AI.OpenAI;
using Game.Common.Configuration;
using Game.Common.Manager;
using Game.Common.Rules;
using Game.Console;
using Game.RuleBook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Launcher
{
    public static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(logging => { 
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            })
            .AddSingleton<IAIPlatform, OpenAIPlatform>()
            .AddSingleton<IAppConfiguration, LocalJsonConfiguration>()
            .AddSingleton<IRuleBook, RuleBook>()
            .AddSingleton<IGameManager, GameManager>()
            .BuildServiceProvider();

        var logger = serviceProvider.GetService<ILoggerFactory>()!.CreateLogger<Launcher>();
        logger.LogInformation("Starting application....");

        serviceProvider.GetService<IGameManager>()?.Start();
        logger.LogInformation("Exitting application....");
    }
}
