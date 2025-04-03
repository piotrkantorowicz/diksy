using OpenAI.Chat;

namespace Diksy.Translation.OpenAI.Services
{
    public interface IChatTranslationService
    {
        Task<ChatMessageContent> TranslateAsync(
            string prompt,
            string? model,
            ChatCompletionOptions options,
            CancellationToken cancellationToken = default);
    }
}