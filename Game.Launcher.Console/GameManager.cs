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

        public override async Task<string> Start()
        {
            // string welcomeMessage = ruleBook.GetWelcomeMessage();

            await gameMaster.StartGame();
            await gameMaster.AnnounceLocation(0);
            while (true)
            {
                // Game loop
                string command = userInterfaceManager.GetInput(new UIMessage(UITargetWindow.Main, UIMessageType.Prompt, "You"));
                if (command == "exit")
                {
                    break;
                }

                UIMessage reply = gameMaster.ReplyToPlayer(command);
                userInterfaceManager.DisplayMessage(reply);
            }
        }
    }
}
