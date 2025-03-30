using Diksy.Translation.Exceptions;
using Diksy.Translation.OpenAI.Factories;
using OpenAI.Chat;
using System.ClientModel;

namespace Diksy.Translation.OpenAI.Services
{
    internal sealed class ChatTranslationService(IOpenAiFactory openAiFactory, OpenAiSettings settings)
        : IClientTranslationService
    {
        private readonly IOpenAiFactory _openAiFactory =
            openAiFactory ?? throw new ArgumentNullException(nameof(openAiFactory));

        private readonly OpenAiSettings _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        public async Task<ChatMessageContent> TranslateAsync(string prompt, string? model,
            ChatCompletionOptions options, CancellationToken cancellationToken = default)
        {
            string modelToUse = model ?? _settings.DefaultModel ?? AllowedModels.Gpt4O;;
            ChatClient? chatClient = _openAiFactory.CreateClient().GetChatClient(modelToUse);

            try
            {
                ClientResult<ChatCompletion>? openAiResponse = await chatClient.CompleteChatAsync(
                    messages: [prompt],
                    options: options,
                    cancellationToken: cancellationToken) ?? throw new TranslationException("Translation response is empty");
                    
                return openAiResponse.Value.Content;
            }
            catch (Exception ex) when (ex is not TranslationException)
            {
                throw new TranslationException($"Failed to translate using model {modelToUse}", ex);
            }
        }
    }
}