using Game.Common.Character;

namespace Game.Common.Rules
{
    public class GameState(GameMap map, PlayerCharacter[] players)
    {
        public GameMap Map { get; private set; } = map;
        public PlayerCharacter[] Players { get; private set; } = players;
        public int CurrentPlayer { get; set; } = 0;
    }
}
