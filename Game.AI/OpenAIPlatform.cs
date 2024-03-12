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
            this.logger.LogDebug($"Initializing OpenAI");
            client = new OpenAIClient(configuration.Get(ConfigurationParameter.OpenAIKey));
        }

        public AIResponse Query(AIRequest request)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = MODEL_NAME, // Use DeploymentName for "model" with non-Azure clients
                Messages =
                {
                    new ChatRequestSystemMessage("You are a game master of Dungeons&Dragons game."),
                    new ChatRequestUserMessage(request.Query),
                }
            };

            return Task.Run(async () =>
            {
                StringBuilder responseContent = new StringBuilder();

                await foreach (StreamingChatCompletionsUpdate chatUpdate in client.GetChatCompletionsStreaming(chatCompletionsOptions))
                {
                    if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
                    {
                        responseContent.AppendLine(chatUpdate.ContentUpdate);
                    }
                }

                // remove new lines 
                string jsonResponse = responseContent.Replace("\n", "").Replace("\r", "").ToString();
                logger.LogDebug($"OpenAI response: {jsonResponse}");

                AIResponse response = new AIResponse(jsonResponse);
                return response;
            }).GetAwaiter().GetResult();
        }
    }
}
