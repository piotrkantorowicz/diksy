using OpenAI;

namespace Diksy.Translation.OpenAI.Factories
{
    internal sealed class OpenAiFactory(OpenAiSettings settings) : IOpenAiFactory
    {
        private readonly OpenAiSettings _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        public OpenAIClient CreateClient()
        {
            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                throw new InvalidOperationException("OpenAI API key is not configured");
            }

            return new OpenAIClient(_settings.ApiKey);
        }
    }
}