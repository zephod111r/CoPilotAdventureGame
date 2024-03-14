using Game.AI;
using Game.Common.Character;
using Game.Common.Rules;
using Game.Common.UI;
using Game.RuleBook.Character;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Game.RuleBook
{
    internal class GameMasterReply
    {
        public string? reply { get; set; }
        public string? location { get; set; }
        public Inventory? inventory { get; set; }
    }

    public class GameMaster(GameSettings gameSettings, IRuleBook ruleBook, IAIPlatform aIPlatform, IUserInterfaceManager userInterfaceManager, ILogger<GameMaster> logger) : IGameMaster
    {
        private const string DEFAULT_THEME = "Dungeons & Dragons";

        private static readonly string[] RULES = {
                "Everything player picks up goes to the bag if capacity not exceeded.",
                "If something was put into the bag, add an object \"inventory\" containing updated bag contents.",
                "If player changed location, add a property \"location\" with the new location.",
        };

        private readonly GameSettings gameSettings = gameSettings;
        private readonly ILogger<GameMaster> logger = logger;
        private readonly IRuleBook ruleBook = ruleBook;
        private readonly IAIPlatform platform = aIPlatform;
        private readonly IUserInterfaceManager userInterfaceManager = userInterfaceManager;

        private PlayerCharacter? gameMaster;
        private GameState? gameState;
        private Queue<KeyValuePair<MessageType, object>>? history = new Queue<KeyValuePair<MessageType, object>>();

        public async Task<string> StartGame()
        {
            string theme = userInterfaceManager.GetInput(new UIMessage(UITargetWindow.Main, UIMessageType.Prompt, $"Choose a theme for the game, or skip to use the \"{DEFAULT_THEME}\""));
            gameSettings.Theme = string.IsNullOrEmpty(theme) ? DEFAULT_THEME : theme;

            var map = CreateMap();
            var startLocation = map.Locations!.First().Value;

            var player = CreateCharacter(startLocation.Name);

            gameState = new GameState(map, [player]);
            gameState.CurrentPlayer = 0;

            gameMaster = new PlayerCharacter(ruleBook.GetGameMasterName().Result, null, null, null, PlayerType.GameMaster, [], new Inventory());

            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, GetWelcomeMessage(), gameMaster));

            gameMaster = new PlayerCharacter(ruleBook.GetGameMasterName().Result, null, null, null, PlayerType.GameMaster, new Dictionary<string, int>(), new Inventory());

            return "Let's go on an adventure together!";
        }

        public UIMessage ReplyToPlayer(string playerCommand)
        {
            var locationName = gameState?.Players[gameState.CurrentPlayer].Location ?? "unknown location";
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, $"You are in {locationName}"), gameMaster);
        }

        public void ReplyToPlayer(string playerCommand)
        {
            var context = new
            {
                rules = RULES,
                gameState,
                prompt = "Return your reply as a text in \"reply\" property. Add \"location\" or \"inventory\" properties if needed. Inventory has a bag property with items names and count."
            };

            var query = new
            {
                playerAction = playerCommand,
            };

            AIResponse response = platform.Query(AIRequestBuilder.ForJson(query)
                    .WithContext(context)
                    .WithHistory(history!)
                    .Build())
                .Result;

            string content = ParseReply(JsonConvert.DeserializeObject<GameMasterReply>(response.Content));

            // dequeue elements while the size of the history queue greater than 25
            while (history!.Count > 23)
            {
                history.Dequeue();
            }

            history.Enqueue(new KeyValuePair<MessageType, object>(MessageType.User, query));
            history.Enqueue(new KeyValuePair<MessageType, object>(MessageType.Assistant, response.Content));
        }

        private PlayerCharacter CreateCharacter(string location)
        {
            NameDescription race = GetRace();
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Normal, $"You selected a {race.Name}", gameMaster));

            NameDescription clasz = GetClass(race);
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Normal, $"You selected a {clasz.Name} of {race.Name}", gameMaster));

            PlayerCharacter playerCharacter = ruleBook.CreateCharacter(race, clasz).Result;
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading,
                $"You created the character named {playerCharacter.Name}, who is a {playerCharacter.Class!.Name} of {playerCharacter.Race!.Name} and looks like:", gameMaster));
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Normal, playerCharacter.Avatar ?? ""));

            playerCharacter.Avatar = null;
            playerCharacter.Location = location;

            return playerCharacter;
        }

        private NameDescription GetRace()
        {
            NameDescription[] races = ruleBook.GetRaces().Result;
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, "Choose race:", gameMaster));
            foreach (NameDescription race in races)
            {
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItemTitle, race.Name));
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItem, race.Description));
            }

            string playerRace = userInterfaceManager.GetInput(new UIMessage(UITargetWindow.Main, UIMessageType.Prompt, "Your race (empty for random)"));
            int raceIndex = Array.FindIndex(races, r => r.Name == playerRace);
            if (raceIndex == -1)
            {
                raceIndex = new Random().Next(0, races.Length);
            }

            return races[raceIndex];
        }

        private NameDescription GetClass(NameDescription race)
        {
            NameDescription[] classes = ruleBook.GetClasses(race.Name).Result;
            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, $"Choose class for {race.Name}:", gameMaster));
            foreach (NameDescription clasz in classes)
            {
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItemTitle, clasz.Name));
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItem, clasz.Description));
            }

            string playerRace = userInterfaceManager.GetInput(new UIMessage(UITargetWindow.Main, UIMessageType.Prompt, "Your class (empty for random)"));
            int classIndex = Array.FindIndex(classes, r => r.Name == playerRace);
            if (classIndex == -1)
            {
                classIndex = new Random().Next(0, classes.Length);
            }

            return classes[classIndex];
        }

        private GameMap CreateMap()
        {
            var map = ruleBook.CreateMap(3, 3).Result!;

            userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.Heading, "Map has the following locations:"));
            foreach (var location in map.Locations!.Values)
            {
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItemTitle, location.Name));
                userInterfaceManager.DisplayMessage(new UIMessage(UITargetWindow.Main, UIMessageType.ListItem, location.Description));
            }

            return map;
        }

        private string GetWelcomeMessage()
        {
            AIResponse response = platform.Query(AIRequestBuilder.ForText("Generate a welcome message for the game. Introduce yourselves as a game master. Describe the world considering twhere he game theme and the purpose of the quest.")
                    .WithContext($"Your name is {gameMaster?.Name ?? "Mystery"}")
                    .Build())
                .Result;
            return response.Content;
        }

        private string ParseReply(GameMasterReply? content)
        {
            if (content?.location != null)
            {
                SwitchLocation(content.location);
            }
            if (content?.inventory != null)
            {
                UpdateInventory(content.inventory);
            }
            return content?.reply ?? "";
        }

        private void SwitchLocation(string newLocation)
        {
            if (gameState!.Players[gameState.CurrentPlayer].Location != newLocation)
            {
                if (gameState!.Map.Locations!.ContainsKey(newLocation))
                {
                    gameState!.Players[gameState.CurrentPlayer].Location = newLocation;

                    // clear the chat history as we are in new location
                    history!.Clear();
                }
                else
                {
                    logger.LogError($"Location {newLocation} is the same or not in the map.");
                }
            }
        }

        private void UpdateInventory(Inventory newInventory)
        {
            gameState!.Players[gameState.CurrentPlayer].Inventory = newInventory;
        }
    }
}
