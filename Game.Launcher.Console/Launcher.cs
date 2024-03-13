using Game.AI;
using Game.AI.OpenAI;
using Game.Common.Configuration;
using Game.Common.Manager;
using Game.Common.Rules;
using Game.TextUI;
using Game.RuleBook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Game.TextUI.UI;
using Game.Common.UI;

public class Launcher
{
    public static void Main(string[] args)
    {
        var gameSettings = new GameSettings("Dungeons & Dragons");

        var serviceProvider = new ServiceCollection()
            .AddLogging(logging => { 
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .AddSingleton<IAIPlatform, OpenAIPlatform>()
            .AddSingleton<IAppConfiguration, LocalJsonConfiguration>()
            .AddSingleton<IRuleBook, RuleBook>()
            .AddSingleton<IUserInterfaceManager, ConsoleUserInterfaceManager>()
            .AddSingleton<IGameManager, GameManager>()
            .AddSingleton<IGameMaster, GameMaster>()
            .AddSingleton(gameSettings)
            .BuildServiceProvider();

        var logger = serviceProvider.GetService<ILoggerFactory>()!.CreateLogger<Launcher>();
        logger.LogDebug("Starting application....");

        serviceProvider.GetService<IGameManager>()?.Start();
        logger.LogDebug("Exitting application....");
    }
}
