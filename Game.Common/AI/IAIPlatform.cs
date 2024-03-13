namespace Game.AI
{
    public class AIRequest
    {
        public string Context { get; private set; }
        public string Query { get; private set; }
        public string Format { get; private set; }

        public AIRequest(string context, string query, string format)
        {
            Context = context;
            Query = query;
            Format = format;
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
        public Task<AIResponse> Query(AIRequest request);
    }
}
