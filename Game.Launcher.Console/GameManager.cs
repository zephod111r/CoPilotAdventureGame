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
            gameMaster.StartGame();

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
