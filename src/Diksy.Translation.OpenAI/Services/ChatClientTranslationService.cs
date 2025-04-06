using Diksy.Translation.Exceptions;
using Diksy.Translation.OpenAI.Factories;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.ClientModel;

namespace Diksy.Translation.OpenAI.Services
{
    internal sealed class ChatTranslationService(IOpenAiFactory openAiFactory, IOptions<OpenAiOptions> openAiOptions)
        : IChatTranslationService
    {
        private readonly IOpenAiFactory _openAiFactory =
            openAiFactory ?? throw new ArgumentNullException(nameof(openAiFactory));

        private readonly IOptions<OpenAiOptions> _openAiOptions =
            openAiOptions ?? throw new ArgumentNullException(nameof(openAiOptions));

        public async Task<ChatMessageContent> TranslateAsync(string prompt, string? model,
            ChatCompletionOptions options, CancellationToken cancellationToken = default)
        {
            string modelToUse = model ?? _openAiOptions.Value.DefaultModel ?? AllowedModels.Gpt4O;
            ChatClient? chatClient = _openAiFactory.CreateClient().GetChatClient(modelToUse);

            try
            {
                ClientResult<ChatCompletion>? openAiResponse = await chatClient.CompleteChatAsync(
                                                                   messages: [prompt],
                                                                   options: options,
                                                                   cancellationToken: cancellationToken) ??
                                                               throw new TranslationException(
                                                                   "Translation response is empty");

                return openAiResponse.Value.Content;
            }
            catch (Exception ex) when (ex is not TranslationException)
            {
                throw new TranslationException(message: $"Failed to translate using model {modelToUse}",
                    innerException: ex);
            }
        }
    }
}