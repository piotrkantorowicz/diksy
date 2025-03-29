using Diksy.Translation.Exceptions;
using Diksy.Translation.Models;
using Diksy.Translation.OpenAI.Factories;
using Diksy.Translation.OpenAI.Schema;
using Diksy.Translation.Services;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text;
using System.Text.Json;

namespace Diksy.Translation.OpenAI
{
    internal sealed class OpenAiTranslator(IOpenAiFactory openAiFactory, ISchemaGenerator schemaGenerator) : ITranslator
    {
        private readonly IOpenAiFactory _openAiFactory =
            openAiFactory ?? throw new ArgumentNullException(nameof(openAiFactory));

        private readonly ISchemaGenerator _schemaGenerator =
            schemaGenerator ?? throw new ArgumentNullException(nameof(schemaGenerator));

        public async Task<TranslationInfo> TranslateAsync(string phrase, string model, string language)
        {
            OpenAIClient openAiClient = _openAiFactory.CreateClient();
            ChatClient? chatClient = openAiClient.GetChatClient(model);

            string[] requiredProperties =
            [
                nameof(TranslationInfo.Phrase), nameof(TranslationInfo.Translation),
                nameof(TranslationInfo.Transcription), nameof(TranslationInfo.Example),
                nameof(TranslationInfo.TranslationOfExample)
            ];

            string jsonSchema = _schemaGenerator.GenerateSchema<TranslationInfo>(requiredProperties);

            ChatCompletionOptions chatCompletionOptions = new()
            {
                Temperature = 0.15f,
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "TranslatorResponse",
                    jsonSchema: BinaryData.FromString(jsonSchema),
                    jsonSchemaIsStrict: true)
            };

            string prompt = new StringBuilder()
                .Append($"Translate the phrase \"{phrase}\" ")
                .Append($"into {language}.")
                .AppendLine()
                .AppendLine("Please provide:")
                .AppendLine("1. Translation that captures the full meaning of the phrase/word")
                .AppendLine("2. Phonetic transcription (for each word if it's a phrasal verb)")
                .AppendLine("3. Example sentence showing proper usage in context")
                .AppendLine("4. Translation of the example sentence")
                .AppendLine()
                .AppendLine(
                    "Note: If this is a phrasal verb or multi-word expression, ensure the translation reflects the complete meaning rather than individual words.")
                .ToString();

            ClientResult<ChatCompletion> openAiResponse =
                await chatClient.CompleteChatAsync(messages: [prompt], options: chatCompletionOptions) ??
                throw new TranslationException("Translation response is empty");

            if (openAiResponse.Value.Content.Count == 0)
            {
                throw new TranslationException("No content returned in translation response");
            }

            string jsonResponse = openAiResponse.Value.Content[0].Text ??
                                  throw new TranslationException("Translation response text is empty");

            TranslationInfo translation = JsonSerializer.Deserialize<TranslationInfo>(jsonResponse) ??
                                          throw new TranslationException("Unable to deserialize translation response");

            return translation;
        }
    }
}