using Game.Common.Character;
using Game.Common.UI;

namespace Game.Common.Rules
{
    public interface IGameMaster
    {
        Task<UIMessage[]> StartGame();
        Task<UIMessage[]> ReplyToPlayer(string playerCommand);
    }
}
