using Game.AI;
using Game.Common.Rules;
using Game.RuleBook.Character;
using Newtonsoft.Json;

namespace Game.RuleBook
{
    public class RuleBook : IRuleBook
    {
        private const string PROMPT_GET_RACES = "What are races a player can choose from when creating its character? Return the list of races in pure JSON string where \"result\" list has items with \"name\" and \"description\" properties.";
        private const string PROMPT_GET_CLASSES = "What are classes a player can choose from when creating its character for the race {0}? Return the list of classes in pure JSON string where \"result\" list has items with \"name\" and \"description\" properties.";

        private IAIPlatform platform;

        public RuleBook(IAIPlatform platform)
        {
            this.platform = platform;
        }

        public List<NameDescription> GetRaces()
        {
            AIResponse response = platform.Query(new AIRequest(PROMPT_GET_RACES));
            return ConvertJsonToList<NameDescription>(response.Content, "result");
        }

        public List<NameDescription> GetClasses(string race)
        {
            AIResponse response = platform.Query(new AIRequest(string.Format(PROMPT_GET_CLASSES, race)));
            return ConvertJsonToList<NameDescription>(response.Content, "result");
        }

        private static List<T> ConvertJsonToList<T>(string jsonResponse, string listKey)
        {
            // Parse the JSON response to a dynamic object
            dynamic? parsedJson = JsonConvert.DeserializeObject(jsonResponse);

            // Extract the "result" list or set it to empty list if it doesn't exist
            return parsedJson?[listKey]?.ToObject<List<T>>() ?? new List<T>();
        }
    }
}
