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
        Image,
        Audio
    }

    public class UICapabilities(bool canDisplayImages, bool canPlayAudio)
    {
        public bool CanDisplayImages { get; private set; } = canDisplayImages;
        public bool CanPlayAudio { get; private set; } = canPlayAudio;
    }

    public class UIMessage(UITargetWindow targetWindow, UIMessageType type, string content, PlayerCharacter? from = null)
    {
        public UITargetWindow TargetWindow { get; private set; } = targetWindow;
        public UIMessageType Type { get; private set; } = type;
        public string Content { get; private set; } = content;
        public PlayerCharacter? From { get; private set; } = from;
    }

    public interface IUserInterfaceManager
    {
        void DisplayMessage(UIMessage message);
        string GetInput(UIMessage? prompt = null);
        UICapabilities Capabilities { get; }
    }
}
