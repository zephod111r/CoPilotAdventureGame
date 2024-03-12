
using Game.Common.Manager;
using Game.Common.Rules;
using Game.RuleBook.Character;
using Microsoft.Extensions.Logging;

namespace Game.Console
{
    public class GameManager : IGameManager
    {
        private readonly ILogger<GameManager> logger;
        private readonly IRuleBook ruleBook;

        public GameManager(IRuleBook ruleBook, ILogger<GameManager> logger)
        {
            this.ruleBook = ruleBook;
            this.logger = logger;
        }

        public void Start()
        {
            CreateCharacter();

            while (true)
            {
                // Game loop
            }
        }

        private void CreateCharacter()
        {
            System.Console.WriteLine("Choose race:");
            List<NameDescription> races = ruleBook.GetRaces();
            foreach (NameDescription race in races)
            {
                System.Console.WriteLine($"{race.Name}\n  {race.Description}");
            }

            int raceIndex = new Random().Next(0, races.Count);
            System.Console.WriteLine($"You selected a {races[raceIndex].Name}");
            System.Console.WriteLine();

            System.Console.WriteLine("Choose class:");
            List<NameDescription> classes = ruleBook.GetClasses(races[raceIndex].Name);
            foreach (NameDescription clasz in classes)
            {
                System.Console.WriteLine($"{clasz.Name}\n  {clasz.Description}");
            }
        }
    }
}
