using Game.Common.Character;
using Game.Common.UI;
using System;

namespace Game.TextUI.UI
{
    internal class WebUserInterfaceManager : IUserInterfaceManager
    {
        public UICapabilities Capabilities => new UICapabilities(true, true);

        public void DisplayMessage(UIMessage message)
        {
            if (message.From != null)
            {
                switch (message.From.playerType)
                {
                    case PlayerType.Human:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case PlayerType.GameMaster:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case PlayerType.NonPlayerCharacter:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                }
                Console.Write(message.From?.Name);
                Console.Write(": ");
            }

            switch (message.Type)
            {
                case UIMessageType.Heading:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(message.Content);
                    Console.ResetColor();
                    break;
                case UIMessageType.ListItemTitle:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(message.Content);
                    Console.ResetColor();
                    break;
                case UIMessageType.ListItem:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(message.Content);
                    Console.ResetColor();
                    break;
                case UIMessageType.Prompt:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(message.Content);
                    Console.Write(": ");
                    Console.ResetColor();
                    break;
                case UIMessageType.Normal:
                    Console.WriteLine(message.Content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string GetInput(UIMessage prompt = null)
        {
            return "";
        }
    }
}
