namespace Game.Common.Rules
{
    public interface IGameMaster
    {
        void StartGame();
        void AnnounceLocation();
        void ReplyToPlayer(string playerCommand);
    }
}
