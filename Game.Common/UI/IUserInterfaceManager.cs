using Game.Common.Character;

namespace Game.Common.UI
{
    public enum UITargetWindow
    {
        Main,
        Log,
    }

    public enum UIMessageType
    {
        Prompt,
        Heading,
        ListItemTitle,
        ListItem,
        Normal,
    }

    public class UIMessage(UITargetWindow targetWindow, UIMessageType type, string content)
    {
        public UITargetWindow TargetWindow { get; private set; } = targetWindow;
        public UIMessageType Type { get; private set; } = type;
        public string Content { get; private set; } = content;
    }

    public interface IUserInterfaceManager
    {
        void DisplayMessage(UIMessage message, PlayerCharacter? from = null);
        string GetInput(UIMessage? prompt = null);
    }
}
