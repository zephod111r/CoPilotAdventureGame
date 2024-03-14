using Game.Common.Manager;
using Game.Common.Rules;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Functions
{
    public class GameManager(IGameMaster gameMaster, ILogger<GameManager> logger) : IGameManager, IHostedService
    {
        private readonly ILogger<GameManager> logger = logger;
        private readonly IUserInterfaceManager userInterfaceManager = userInterfaceManager;
        private readonly IGameMaster gameMaster = gameMaster;

        public void Start()
        {
            logger.LogInformation("Starting game....");
            gameMaster.StartGame();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
