using Game.AI;
using Game.Common.Character;
using Game.Common.Rules;
using Game.Common.UI;
using Game.RuleBook.Character;
using Microsoft.Extensions.Logging;

namespace Game.RuleBook
{
    public class GameMaster(IRuleBook ruleBook, IAIPlatform aIPlatform, IUserInterfaceManager userInterfaceManager, ILogger<GameMaster> logger) : IGameMaster
    {
        private readonly ILogger<GameMaster> logger = logger;
        private readonly IRuleBook ruleBook = ruleBook;
        private readonly IAIPlatform platform = aIPlatform;
        private readonly IUserInterfaceManager userInterfaceManager = userInterfaceManager;

        private PlayerCharacter gameMaster;
        private GameState gameState;

        public void StartGame()
        {
            var map = CreateMap();
            var startLocation = map.MapLocations.First().Value;

            var player = CreateCharacter(startLocation.Name);

            gameState = new GameState(map, [player]);

            gameMaster = new PlayerCharacter(ruleBook.GetGameMasterName().Result, null, null, null, PlayerType.GameMaster);
        }

        public void AnnounceLocation(int playerId)
        {
            var locationName = gameState.Players[playerId].Location;
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, $"You are in {locationName}"));
        }

        public void ReplyToPlayer(int playerId, string playerCommand)
        {
            var player = gameState.Players[playerId];
            AIResponse response = platform.Query(new AIRequest($"Locations are {gameState.Map}.\nPlayer is in location {player.Location}", $"Player says \"{playerCommand}\"", "text")).Result;
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, response.Content));
        }

        private PlayerCharacter CreateCharacter(string location)
        {
            NameDescription[] races = ruleBook.GetRaces().Result;
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, "Choose race:"));
            foreach (NameDescription race in races)
            {
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItemTitle, race.Name));
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItem, race.Description));
            }

            int raceIndex = new Random().Next(0, races.Length);
            string raceName = races[raceIndex].Name;
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Normal, $"You selected a {raceName}"));

            NameDescription[] classes = ruleBook.GetClasses(races[raceIndex].Name).Result;
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, $"Choose class for {raceName}:"));
            foreach (NameDescription clasz in classes)
            {
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItemTitle, clasz.Name));
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItem, clasz.Description));
            }

            int classIndex = new Random().Next(0, classes.Length);
            string className = classes[classIndex].Name;
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Normal, $"You selected a {className} of {raceName}"));

            PlayerCharacter playerCharacter = ruleBook.CreateCharacter(races[raceIndex], classes[classIndex]).Result;
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, $"You created the character named {playerCharacter.Name}, who is a {playerCharacter.Class!.Name} of {playerCharacter.Race!.Name} and looks like:"));
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Normal, playerCharacter.Avatar ?? ""));

            playerCharacter.Location = location;

            return playerCharacter;
        }

        private GameMap CreateMap()
        {
            var map = ruleBook.CreateMap(3, 3).Result;

            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, "Map has the following locations:"));
            foreach (var location in map.MapLocations.Values)
            {
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItemTitle, location.Name));
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItem, location.Description));
            }

            return map;
        }
    }
}
