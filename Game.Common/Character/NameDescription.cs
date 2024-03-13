namespace Game.RuleBook.Character
{
    public class NameDescription
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public NameDescription(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public override string ToString()
        {
            return $"Name: {Name}, Description: {Description}";
        }
    }
}
