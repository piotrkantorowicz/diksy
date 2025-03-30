using OpenAI;

namespace Diksy.Translation.OpenAI.Factories
{
    public interface IOpenAiFactory
    {
        OpenAIClient CreateClient();
    }
}