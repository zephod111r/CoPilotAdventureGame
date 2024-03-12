namespace Game.AI
{
    public class AIRequest
    {
        public string Query { get; private set; }

        public AIRequest(string query)
        {
            Query = query;
        }
    }

    public class AIResponse
    {
        public string Content { get; private set; }

        public AIResponse(string content)
        {
            Content = content;
        }
    }

    public interface IAIPlatform
    {
        public AIResponse Query(AIRequest request);
    }
}
