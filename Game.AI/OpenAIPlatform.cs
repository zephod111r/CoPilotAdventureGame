using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Game.AI.OpenAI
{
    public class OpenAIPlatform : IAIPlatform
    {
        private const string TEXT_MODEL_NAME = "gpt-3.5-turbo";
        private const string IMAGE_MODEL_NAME = "dall-e-3";
        private const string SPEECH_MODEL_NAME = "tts-1";
        private const string OPENAI_API_KEY_PARAMETER = "OpenAIKey";

        private readonly ILogger<OpenAIPlatform> logger;
        private readonly OpenAIClient client;

        public OpenAIPlatform(IConfiguration configuration, ILogger<OpenAIPlatform> logger)
        {
            this.logger = logger;

            // Initialize OpenAI
            this.logger.LogDebug("Initializing OpenAI");
            client = new OpenAIClient(configuration[OPENAI_API_KEY_PARAMETER]);
        }

        public async Task<AIResponse> Query(AIRequest request)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = TEXT_MODEL_NAME, // Use DeploymentName for "model" with non-Azure clients
            };

            string context;
            string query;
            if (request.ContentType == ContentType.Json)
            {
                chatCompletionsOptions.ResponseFormat = ChatCompletionsResponseFormat.JsonObject;

                // Add a system message that contains the word 'json'
                chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage("json"));

                context = JsonSerializer.Serialize(request.Context);
                query = JsonSerializer.Serialize(request.Query);
            }
            else
            {
                context = request.Context.ToString()!;
                query = request.Query.ToString()!;
            }

            if (!string.IsNullOrEmpty(context))
            {
                chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage(context));
            }

            foreach (var msg in request.History)
            {
                ChatRequestMessage message = msg.Key == MessageType.User ? new ChatRequestUserMessage(msg.Value.ToString()) : new ChatRequestAssistantMessage(msg.Value.ToString());
                chatCompletionsOptions.Messages.Add(message);
            }

            chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(query));

            logger.LogTrace($"OpenAI request:\n ContentType: {request.ContentType}\n Context: {context}\n Query: {query}\n History: {JsonSerializer.Serialize(request.History)}");

            return await client.GetChatCompletionsAsync(chatCompletionsOptions)
                .ContinueWith(response =>
                {
                    string content = response.Result.Value.Choices[0].Message.Content;
                    logger.LogTrace($"OpenAI response: total={response.Result.Value.Usage.TotalTokens}, completion={response.Result.Value.Usage.CompletionTokens}, prompt={response.Result.Value.Usage.PromptTokens} ");
                    logger.LogTrace($"OpenAI response:\n{content}");
                    return new AIResponse(content);
                });
        }

        public async Task<Uri> GenerateImage(AIRequest request)
        {
            var imageGenerationsOptions = new ImageGenerationOptions
            {
                DeploymentName = IMAGE_MODEL_NAME,
                ImageCount = 1,
                Prompt = $"{request.Context}. {request.Query}",
            };

            logger.LogTrace($"OpenAI request:\n Context: {request.Context}\n Query: {request.Query}");

            return await client.GetImageGenerationsAsync(imageGenerationsOptions)
                .ContinueWith(response =>
                {
                    return response.Result.Value.Data[0].Url;
                });
        }

        public async Task<byte[]> GenerateAudio(AIRequest request)
        {
            var audioGenerationsOptions = new SpeechGenerationOptions
            {
                DeploymentName = SPEECH_MODEL_NAME,
                Voice = "onyx",
                Input = request.Query.ToString(),
            };

            return await client.GenerateSpeechFromTextAsync(audioGenerationsOptions)
                 .ContinueWith(response => response.Result.Value.ToArray());
        }
    }
}
