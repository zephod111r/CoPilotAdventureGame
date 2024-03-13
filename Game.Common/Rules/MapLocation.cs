namespace Game.Common.Rules
{
    public class MapLocation(string name, string description, string[] connections)
    {
        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;
        public string[] Connections { get; private set; } = connections;
    }
}
