using Azure.AI.OpenAI;
using Game.Common.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Game.AI.OpenAI
{
    public class OpenAIPlatform : IAIPlatform
    {
        private const string MODEL_NAME = "gpt-3.5-turbo";

        private readonly ILogger<OpenAIPlatform> logger;
        private readonly OpenAIClient client;

        public OpenAIPlatform(IAppConfiguration configuration, ILogger<OpenAIPlatform> logger)
        {
            this.logger = logger;

            // Initialize OpenAI
            this.logger.LogDebug("Initializing OpenAI");
            client = new OpenAIClient(configuration.Get(ConfigurationParameter.OpenAIKey));
        }

        public async Task<AIResponse> Query(AIRequest request)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = MODEL_NAME, // Use DeploymentName for "model" with non-Azure clients
                Messages =
                {
                    new ChatRequestSystemMessage(request.Context),
                    new ChatRequestUserMessage(request.Query),
                    new ChatRequestUserMessage($"Output format is {request.Format}"),
                }
            };

            return await Task.Run(() =>
            {
                StringBuilder responseContent = new StringBuilder();

                foreach (ChatChoice chatUpdate in client.GetChatCompletions(chatCompletionsOptions).Value.Choices)
                {
                    if (!string.IsNullOrEmpty(chatUpdate.Message.Content))
                    {
                        responseContent.AppendLine(chatUpdate.Message.Content);
                    }
                }

                logger.LogTrace($"OpenAI response:\n{responseContent}");

                return new AIResponse(responseContent.ToString());
            });
        }
    }
}
