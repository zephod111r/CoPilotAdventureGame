using Game.Common.Manager;
using Game.Common.Rules;
using Game.Common.UI;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Game.Functions
{
    internal class GameManager(IGameMaster gameMaster, IUserInterfaceManager userInterfaceManager, ILogger<GameManager> logger) : IGameManager
    {
        private readonly ILogger<GameManager> logger = logger;
        private readonly IUserInterfaceManager userInterfaceManager = userInterfaceManager;
        private readonly IGameMaster gameMaster = gameMaster;

        public async Task<string> Start()
        {
            // string welcomeMessage = ruleBook.GetWelcomeMessage();

            await gameMaster.StartGame();
            return await gameMaster.AnnounceLocation();
            


        }
    }
}
