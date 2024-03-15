using Game.AI;
using Game.Common.Character;
using Game.Common.Rules;
using Game.RuleBook.Character;
using Newtonsoft.Json;

namespace Game.RuleBook
{
    public class RuleBook : IRuleBook
    {
        private const string GAME_CONTEXT = "You are a game master of Dungeons & Dragons style game. The game has the specific theme.";
        private const string PROMPT_GET_RACES = "Return the list of races a player can choose from when creating its character";
        private const string NAME_DESCRIPTION_FORMAT = "JSON text where \"result\" list has items with \"name\" and \"description\" properties.";

        private const string PROMPT_GET_CLASSES = "Return the list of classes a player can choose from when creating its character for the race {0}.";

        private const string CHARACTER_NAME_PROMPT = "Generate a name of the character of race {0} ({1}) and class {2} ({3}).";
        private const string CHARACTER_IMAGE_PROMPT = "Generate an avatar image of the character of race {0} ({1}) and class {2} ({3}). The image must contain ncharacter only UI.";

        private const string ABILITIES_PROMPT = "Generate a list of abilities of the character with random values but adjusted to race {2} ({3}) anf class {0} ({1})";
        private const string ABILITIES_FORMAT = "JSON object with ability name as a key and an integer value";

        private const string INVENTORY_PROMPT = "Generate the initial inventory and wallet contents for the character of race {0} ({1}) and class {2} ({3})";
        private const string INVENTORY_FORMAT = "JSON object with bag and wallet properties. Bag property has items names and count. Wallet property has valueables name and count";

        private const string CREATE_MAP_PROMPT = "Generate a location map for the game:\n- locations are in {0}x{1} grid\n" +
            "- each location should have a scane to interact\n" +
            "- a location can have connections to neighbour locations.\n";
        private const string CREATE_MAP_FORMAT = "JSON  with \"locations\" object containing entries with a descriptive location name as a key and properties:\r\n- \"name\" - the name of the location, same as the key\r\n- \"description\" - description of the location\r\n- \"connections\" - list of location names where a player can go from this location.";

        private const string GAME_MASTER_NAME_PROMPT = "Generate a name for the game master and return only name value.";
        private const string TEXT_FORMAT = "Return value in message property";

        private readonly IAIPlatform platform;
        private readonly GameSettings gameSettings;
        private object gameContext { get { return GetContext(); } }

        public RuleBook(IAIPlatform platform, GameSettings gameSettings)
        {
            this.platform = platform;
            this.gameSettings = gameSettings;
        }

        public async Task<NameDescription[]> GetRaces()
        {
            return await platform.Query(AIRequestBuilder.ForJson(GetPrompt(PROMPT_GET_RACES, NAME_DESCRIPTION_FORMAT)).WithContext(gameContext).Build())
                .ContinueWith(action => Util.ConvertJsonToList<NameDescription>(action.Result.Content, "result"));
        }

        public async Task<NameDescription[]> GetClasses(string race)
        {
            return await platform.Query(AIRequestBuilder.ForJson(GetPrompt(string.Format(PROMPT_GET_CLASSES, race), NAME_DESCRIPTION_FORMAT)).WithContext(gameContext).Build())
                .ContinueWith(action => Util.ConvertJsonToList<NameDescription>(action.Result.Content, "result"));
        }

        public async Task<PlayerCharacter> CreateCharacter(NameDescription race, NameDescription clasz)
        {
            string[] results = await Task.WhenAll(
                platform.Query(AIRequestBuilder.ForJson(GetPrompt(string.Format(CHARACTER_NAME_PROMPT, race.Name, race.Description, clasz.Name, clasz.Description), TEXT_FORMAT)).WithContext(gameContext).Build())
                        .ContinueWith(action => action.Result.Content),
//                platform.Query(AIRequestBuilder.ForText(string.Format(CHARACTER_IMAGE_PROMPT, race.Name, race.Description, clasz.Name, clasz.Description)).WithContext(gameContext).Build())
//                        .ContinueWith(action => action.Result.Content),
                platform.Query(AIRequestBuilder.ForJson(GetPrompt(string.Format(ABILITIES_PROMPT, clasz.Name, clasz.Description, race.Name, race.Description), ABILITIES_FORMAT)).WithContext(gameContext).Build())
                        .ContinueWith(action => action.Result.Content),
                platform.Query(AIRequestBuilder.ForJson(GetPrompt(string.Format(INVENTORY_PROMPT, clasz.Name, clasz.Description, race.Name, race.Description), INVENTORY_FORMAT)).WithContext(gameContext).Build())
                        .ContinueWith(action => action.Result.Content),
                platform.GenerateImage(AIRequestBuilder.ForText(string.Format(CHARACTER_IMAGE_PROMPT, race.Name, race.Description, clasz.Name, clasz.Description)).WithContext($"{GAME_CONTEXT}. Theme of the game is {gameSettings.Theme}").Build())
                        .ContinueWith(action => action.Result.AbsoluteUri.ToString())
            );

            string name = Util.ConvertJsonToValue(results[0], "message");
            Dictionary<string, int> abilities = Util.ConvertJsonToDictionary<int>(results[1], "abilities");
            Inventory inventory = JsonConvert.DeserializeObject<Inventory>(results[2])!;
            string avatar = GetMarkdownQuoteFromString(results[3]);

            return new PlayerCharacter(name, race, clasz, avatar, PlayerType.Human, new Dictionary<string, int>(abilities), inventory);
        }

        public async Task<GameMap?> CreateMap(int width, int height)
        {
            return await platform.Query(AIRequestBuilder.ForJson(GetPrompt(string.Format(CREATE_MAP_PROMPT, width, height), CREATE_MAP_FORMAT)).WithContext(gameContext).Build())
                .ContinueWith(action => JsonConvert.DeserializeObject<GameMap>(action.Result.Content));
        }

        public async Task<string> GetGameMasterName()
        {
            return await platform.Query(AIRequestBuilder.ForJson(GetPrompt(GAME_MASTER_NAME_PROMPT, TEXT_FORMAT)).WithContext(gameContext).Build())
                .ContinueWith(action => Util.ConvertJsonToValue(action.Result.Content, "message"));
        }

        public async Task<string> GetImage(string query)
        {
            return await platform.GenerateImage(AIRequestBuilder.ForText(query).WithContext($"{GAME_CONTEXT}. Theme of the game is {gameSettings.Theme}").Build())
        .ContinueWith(action => action.Result.AbsoluteUri.ToString());

        }

        private static string GetMarkdownQuoteFromString(string content)
        {
            string[] lines = content.Split("\n");
            int startIndex = Array.FindIndex(lines, line => line.StartsWith("```"));
            int endIndex = Array.FindLastIndex(lines, line => line.StartsWith("```"));
            if (startIndex == -1 || endIndex == -1 || startIndex == endIndex)
            {
                return content;
            }

            return string.Join(Environment.NewLine, lines, startIndex + 1, endIndex - startIndex - 1);
        }

        private object GetContext()
        {
            return new
            {
                context = GAME_CONTEXT,
                theme = gameSettings.Theme,
            };
        }

        private object GetPrompt(string prompt, string format)
        {
            return new
            {
                prompt,
                format
            };
        }
    }
}
