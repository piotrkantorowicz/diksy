using OpenAI;

namespace Diksy.Translation.OpenAI.Factories
{
    internal sealed class OpenAiFactory(OpenAiSettings settings) : IOpenAiFactory
    {
        private readonly OpenAiSettings _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        public OpenAIClient CreateClient()
        {
            return new OpenAIClient(_settings.ApiKey);
        }
    }
}