using Game.Common.Character;
using Game.Common.UI;

namespace Game.Function
{
    internal class WebInterfaceManager : IUserInterfaceManager
    {
        public string GetInput(UIMessage? prompt = null)
        {
            return "";
        }

        public void DisplayMessage(UIMessage message, PlayerCharacter? from = null)
        {
            
        }
    }
}