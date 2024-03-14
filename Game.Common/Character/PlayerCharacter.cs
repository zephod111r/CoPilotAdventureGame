using Game.RuleBook.Character;

namespace Game.Common.Character
{
    public enum PlayerType
    {
        Human,
        GameMaster,
        NonPlayerCharacter
    }

    public class PlayerCharacter(string name, NameDescription? race, NameDescription? clasz, string? avatar, PlayerType playerType, Dictionary<string, int> abilities, Inventory inventory)
    {
        public string Name { get; private set; } = name;
        public NameDescription? Race { get; private set; } = race;
        public NameDescription? Class { get; private set; } = clasz;
        public string? Avatar { get; set; } = avatar;
        public PlayerType playerType { get; private set; } = playerType;
        public Dictionary<string, int> Abilities { get; private set; } = abilities;
        public Inventory Inventory { get; set; } = inventory;
        public string Location { get; set; } = "";
        public int Level { get;  set; } = 1;
        public int HitPoints { get;  set; } = 10;
    }
}
