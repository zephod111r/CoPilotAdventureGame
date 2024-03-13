using Game.RuleBook.Character;

namespace Game.Common.Character
{
    public enum PlayerType
    {
        Human,
        GameMaster,
        NonPlayerCharacter
    }

    public class PlayerCharacter(string name, NameDescription? race, NameDescription? clasz, string? avatar, PlayerType playerType)
    {
        public string Name { get; private set; } = name;
        public NameDescription? Race { get; private set; } = race;
        public NameDescription? Class { get; private set; } = clasz;
        public string? Avatar { get; private set; } = avatar;
        public PlayerType playerType { get; private set; } = playerType;
        public string Location { get; set; } = "";
    }
}
