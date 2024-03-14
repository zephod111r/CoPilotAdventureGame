namespace Game.Common.Character
{
    public class Inventory
    {
        public Dictionary<string, int> Bag { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> Wallet { get; } = new Dictionary<string, int>();
    }
}
