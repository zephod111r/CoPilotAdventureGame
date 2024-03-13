using Game.AI;
using Game.Common.Character;
using Game.Common.Rules;
using Game.RuleBook.Character;
using Newtonsoft.Json;

namespace Game.RuleBook
{
    public class RuleBook : IRuleBook
    {
        private const string GAME_CONTEXT = "You are a game master of Dungeons&Dragons game with a theme {0}.";
        private const string PROMPT_GET_RACES = "Return the list of races a player can choose from when creating its character";
        private const string NAME_DESCRIPTION_FORMAT = "JSON text where \"result\" list has items with \"name\" and \"description\" properties.";
        private const string LOCATIONS_FORMAT = "JSON object where entries with location name as a key and items:\r\n- \"name\" - name of the location\r\n- \"description\" - description of the location\r\n- \"connections\" - list of location names where a player can go from this location.";

        private const string PROMPT_GET_CLASSES = "Return the list of classes a player can choose from when creating its character for the race {0}.";

        private readonly IAIPlatform platform;
        private readonly GameSettings gameSettings;
        private readonly string gameContext;

        public RuleBook(IAIPlatform platform, GameSettings gameSettings)
        {
            this.platform = platform;
            this.gameSettings = gameSettings;
            this.gameContext = string.Format(GAME_CONTEXT, gameSettings.Theme);
        }

        public async Task<NameDescription[]> GetRaces()
        {
            return await platform.Query(new AIRequest(GAME_CONTEXT, PROMPT_GET_RACES, NAME_DESCRIPTION_FORMAT))
                .ContinueWith(action => ConvertJsonToList<NameDescription>(action.Result.Content, "result"));
        }

        public async Task<NameDescription[]> GetClasses(string race)
        {
            return await platform.Query(new AIRequest(GAME_CONTEXT, string.Format(PROMPT_GET_CLASSES, race), NAME_DESCRIPTION_FORMAT))
                .ContinueWith(action => ConvertJsonToList<NameDescription>(action.Result.Content, "result"));
        }

        public async Task<PlayerCharacter> CreateCharacter(NameDescription race, NameDescription clasz)
        {
            string[] results = await Task.WhenAll(
                    platform.Query(new AIRequest(gameContext, $"Generate a name of the character of race {race.Name} ({race.Description}) and class {clasz.Name} ({clasz.Description}).", "text only"))
                        .ContinueWith(action => action.Result.Content.Replace(Environment.NewLine, "")),
                    platform.Query(new AIRequest(gameContext, $"Generate an ASCII image of the character of race {race.Name} ({race.Description}) and class {clasz.Name} ({clasz.Description}).", "ASCII image"))
                        .ContinueWith(action => GetMarkdownQuoteFromString(action.Result.Content))
                );

            return new PlayerCharacter(results[0], race, clasz, GetMarkdownQuoteFromString(results[1]), PlayerType.Human);
        }

        public async Task<GameMap> CreateMap(int width, int height)
        {
            return await platform.Query(new AIRequest(gameContext, $"Generate a location map for the game:\r\n- locations are in {width}x{height} grid\r\n" +
                $"- each location should have a scane to interact\r\n" +
                $"- a location can have connections to neighbour locations.\r\n", LOCATIONS_FORMAT))
                .ContinueWith(action => new GameMap(ConvertJsonToDictionary<MapLocation>(action.Result.Content, "locations")));
        }

        public async Task<string> GetGameMasterName()
        {
            return await platform.Query(new AIRequest(gameContext, "Generate a name for the game master.", "text only"))
                .ContinueWith(action => action.Result.Content.Replace(Environment.NewLine, ""));
        }

        private static T[] ConvertJsonToList<T>(string responseContent, string listKey)
        {
            // remove new lines 
            string jsonResponse = GetMarkdownQuoteFromString(responseContent).Replace(Environment.NewLine, "").ToString();

            // Parse the JSON response to a dynamic object
            dynamic? parsedJson = JsonConvert.DeserializeObject(jsonResponse);

            // Extract the "lit" list or set it to empty list if it doesn't exist
            return parsedJson?[listKey]?.ToObject<T[]>() ?? Array.Empty<T>();
        }

        private static Dictionary<string, T> ConvertJsonToDictionary<T>(string responseContent, string listKey)
        {
            // remove new lines 
            string jsonResponse = GetMarkdownQuoteFromString(responseContent).Replace(Environment.NewLine, "").ToString();

            // Parse the JSON response to a dynamic object
            Dictionary<string, T>? parsedJson = JsonConvert.DeserializeObject<Dictionary<string, T>>(jsonResponse);

            // Extract the "lit" list or set it to empty dictionary if it doesn't exist
            return parsedJson ?? [];
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
    }
}
