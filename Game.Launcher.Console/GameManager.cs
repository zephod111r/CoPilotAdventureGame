using Game.Common.Manager;
using Game.Common.Rules;
using Game.Common.UI;
using Microsoft.Extensions.Logging;

namespace Game.TextUI
{
    public class GameManager(IGameMaster gameMaster, IUserInterfaceManager userInterfaceManager, ILogger<GameManager> logger) : IGameManager
    {
        private readonly ILogger<GameManager> logger = logger;
        private readonly IUserInterfaceManager userInterfaceManager = userInterfaceManager;
        private readonly IGameMaster gameMaster = gameMaster;

        public void Start()
        {
            // string welcomeMessage = ruleBook.GetWelcomeMessage();

            gameMaster.StartGame();
            gameMaster.AnnounceLocation(0);
            while (true)
            {
                // Game loop
                string command = userInterfaceManager.GetInput();
                if (command == "exit")
                {
                    break;
                }

                gameMaster.ReplyToPlayer(0, command);
            }
        }
    }
}
