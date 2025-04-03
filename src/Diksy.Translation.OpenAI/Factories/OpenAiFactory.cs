using Microsoft.Extensions.Options;
using OpenAI;

namespace Diksy.Translation.OpenAI.Factories
{
    internal sealed class OpenAiFactory(IOptions<OpenAiOptions> openAiOptions) : IOpenAiFactory
    {
        private readonly IOptions<OpenAiOptions> _openAiOptions =
            openAiOptions ?? throw new ArgumentNullException(nameof(openAiOptions));

        public OpenAIClient CreateClient()
        {
            if (string.IsNullOrEmpty(_openAiOptions.Value.ApiKey))
            {
                throw new InvalidOperationException("OpenAI API key is not configured");
            }

            return new OpenAIClient(_openAiOptions.Value.ApiKey);
        }
    }
}