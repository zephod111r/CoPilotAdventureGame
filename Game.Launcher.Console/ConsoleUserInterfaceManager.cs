using Game.Common.Character;
using Game.Common.UI;

namespace Game.TextUI
{
    internal class ConsoleUserInterfaceManager : IUserInterfaceManager
    {
        public UICapabilities Capabilities => new UICapabilities(false, false);

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
                Console.Write(message.From.Name);
                Console.ResetColor();
                Console.Write(": ");
            }

            switch (message.Type)
            {
                case UIMessageType.Heading:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(message.Content);
                    break;
                case UIMessageType.ListItemTitle:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(message.Content);
                    break;
                case UIMessageType.ListItem:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(message.Content);
                    break;
                case UIMessageType.Prompt:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(message.Content);
                    Console.Write(": ");
                    break;
                case UIMessageType.Normal:
                    Console.WriteLine(message.Content);
                    break;
            }
            Console.ResetColor();
        }

        public string GetInput(UIMessage? prompt = null)
        {
            if (prompt != null)
            {
                DisplayMessage(prompt);
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("> ");
            Console.ResetColor();

            return Console.ReadLine() ?? "";
        }
    }
}
