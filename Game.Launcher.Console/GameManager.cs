﻿using Game.Common.Manager;
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

            string theme = userInterfaceManager.GetInput(new UIMessage(UITargetWindow.Main, UIMessageType.Prompt, "Choose a theme for the game, or skip to use default"));

            UIMessage[] welcomeMessage = gameMaster.StartGame(theme).Result;
            foreach (var message in welcomeMessage)
            {
                userInterfaceManager.DisplayMessage(message);
            }

            while (true)
            {
                // Game loop
                string command = userInterfaceManager.GetInput(new UIMessage(UITargetWindow.Main, UIMessageType.Prompt, "You"));
                if (command == "exit")
                {
                    break;
                }

                UIMessage[] reply = gameMaster.ReplyToPlayer(command).Result;
                foreach (var message in reply)
                {
                    userInterfaceManager.DisplayMessage(message);
                }
            }
        }
    }
}
