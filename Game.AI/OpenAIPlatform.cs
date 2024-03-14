﻿using Azure.AI.OpenAI;
using Game.Common.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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
    }
}
