namespace Game.AI
{
    public enum MessageType
    {
        User,
        Assistant
    }

    public enum ContentType
    {
        Text,
        Json
    }

    public class AIRequest
    {
        public object Context { get; private set; }
        public IReadOnlyCollection<KeyValuePair<MessageType, object>> History { get; private set; }
        public object Query { get; private set; }
        public ContentType ContentType { get; private set; }

        internal AIRequest(object context, IReadOnlyCollection<KeyValuePair<MessageType, object>> history, object query, ContentType contentType)
        {
            Context = context;
            History = history;
            Query = query;
            ContentType = contentType;
        }
    }

    public class AIRequestBuilder
    {
        private object _context = "";
        private IReadOnlyCollection<KeyValuePair<MessageType, object>> _history = [];
        private readonly object _query;
        private ContentType _contentType;

        public static AIRequestBuilder ForText(string query)
        {
            return new AIRequestBuilder(query, ContentType.Text);
        }
        public static AIRequestBuilder ForJson(object query)
        {
            return new AIRequestBuilder(query, ContentType.Json);
        }

        internal AIRequestBuilder(object query, ContentType contentType)
        {
            _contentType = contentType;
            _query = query;
        }

        public AIRequestBuilder WithContext(object context)
        {
            _context = context;
            return this;
        }

        public AIRequestBuilder WithHistory(IReadOnlyCollection<KeyValuePair<MessageType, object>> history)
        {
            _history = history;
            return this;
        }

        public AIRequest Build()
        {
            return new AIRequest(_context, _history, _query, _contentType);
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
        Task<AIResponse> Query(AIRequest request);
        Task<Uri> GenerateImage(AIRequest request);
        Task<Stream> GenerateAudio(AIRequest request);
    }
}
