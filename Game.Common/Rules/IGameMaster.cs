namespace Game.Common.Rules
{
    public interface IGameMaster
    {
        void StartGame();
        void AnnounceLocation(int playerId);
        void ReplyToPlayer(int playerId, string playerCommand);
    }
}
