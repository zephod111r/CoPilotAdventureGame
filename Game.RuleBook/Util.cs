using Newtonsoft.Json;

namespace Game.RuleBook
{
    public class Util
    {
        public static Dictionary<string, T> ConvertJsonToDictionary<T>(string responseContent, string listKey)
        {
            Dictionary<string, T>? parsedJson = JsonConvert.DeserializeObject<Dictionary<string, T>>(responseContent);
            return parsedJson ?? [];
        }

        public static T[] ConvertJsonToList<T>(string responseContent, string listKey)
        {
            // Parse the JSON response to a dynamic object
            dynamic? parsedJson = JsonConvert.DeserializeObject(responseContent);
            return parsedJson?[listKey]?.ToObject<T[]>() ?? Array.Empty<T>();
        }

        public static string ConvertJsonToValue(string responseContent, string key)
        {
            // Parse the JSON response to a dynamic object
            dynamic? parsedJson = JsonConvert.DeserializeObject(responseContent);
            return parsedJson?[key] ?? "";
        }
    }
}
