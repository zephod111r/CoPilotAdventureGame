using Game.Common.Character;
using Game.Common.UI;

namespace Game.TextUI.UI
{
    internal class ConsoleUserInterfaceManager : IUserInterfaceManager
    {
        public void DisplayMessage(UIMessage message, PlayerCharacter? from = null)
        {
            if (from != null)
            {
                switch (from.playerType)
                {
                    case PlayerType.Human:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case PlayerType.GameMaster:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case PlayerType.NonPlayerCharacter:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                }
                Console.WriteLine(from.Name);
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
                    Console.WriteLine(message.Content);
                    Console.ResetColor();
                    break;
                case UIMessageType.Normal:
                    Console.WriteLine(message.Content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
