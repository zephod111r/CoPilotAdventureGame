using Game.AI;
using Game.AI.OpenAI;
using Game.Common.Manager;
using Game.Common.Rules;
using Game.Common.Storage;
using Game.Common.UI;
using Game.RuleBook;
using Game.TextUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Launcher
{
    public static void Main(string[] args)
    {
        var environmentName = "local";

        var builder = new ConfigurationBuilder()
            // get paranet fodler of Environment.ProcessPath
            .SetBasePath(Directory.GetParent(Environment.ProcessPath!)!.FullName)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"{environmentName}.appsettings.json", optional: true, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

        var gameSettings = new GameSettings();

        var serviceProvider = new ServiceCollection()
            .AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .AddSingleton<IAIPlatform, OpenAIPlatform>()
            .AddSingleton<IStorage, NoStorage>()
            .AddSingleton<IRuleBook, RuleBook>()
            .AddSingleton<IUserInterfaceManager, ConsoleUserInterfaceManager>()
            .AddSingleton<IGameManager, GameManager>()
            .AddSingleton<IGameMaster, GameMaster>()
            .AddSingleton(configuration)
            .AddSingleton(gameSettings)
            .BuildServiceProvider();

        var logger = serviceProvider.GetService<ILoggerFactory>()!.CreateLogger<Launcher>();
        logger.LogDebug("Starting application....");

        serviceProvider.GetService<IGameManager>()?.Start();
        logger.LogDebug("Exitting application....");
    }
}
