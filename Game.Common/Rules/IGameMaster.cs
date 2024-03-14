namespace Game.Common.Rules
{
    public interface IGameMaster
    {
        async Task<string> StartGame() { return ""; }
        async Task<string> AnnounceLocation() { return ""; }
        async Task<string> ReplyToPlayer(string playerCommand) { return ""; }
    }
}
