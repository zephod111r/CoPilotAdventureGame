using Game.Common.Character;
using Game.Common.UI;

namespace Game.Common.Rules
{
    public interface IGameMaster
    {
        void StartGame();
        UIMessage ReplyToPlayer(string playerCommand);
        
    }
}
