using OpenAI.Chat;

namespace Diksy.Translation.OpenAI.Services
{
    public interface IClientTranslationService
    {
        Task<ChatMessageContent> TranslateAsync(
            string prompt,
            string? model,
            ChatCompletionOptions options,
            CancellationToken cancellationToken = default);
    }
}