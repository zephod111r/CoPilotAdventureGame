namespace Game.Common.Rules
{
    public class GameMap(Dictionary<string, MapLocation> mapLocations)
    {
        public Dictionary<string, MapLocation> MapLocations { get; private set; } = mapLocations;
    }
}
