using Diksy.Translation.Exceptions;
using Diksy.Translation.Models;
using Diksy.Translation.OpenAI.Schema;
using Diksy.Translation.OpenAI.Services;
using Diksy.Translation.Services;
using OpenAI.Chat;
using System.Text;
using System.Text.Json;

namespace Diksy.Translation.OpenAI
{
    internal sealed class OpenAiTranslator(
        IChatTranslationService chatChatTranslationService,
        ISchemaGenerator schemaGenerator) : ITranslator
    {
        private readonly IChatTranslationService _chatChatTranslationService =
            chatChatTranslationService ?? throw new ArgumentNullException(nameof(chatChatTranslationService));

        private readonly ISchemaGenerator _schemaGenerator =
            schemaGenerator ?? throw new ArgumentNullException(nameof(schemaGenerator));


        public async Task<TranslationInfo> TranslateAsync(string phrase, string model, string language,
            CancellationToken cancellationToken)
        {
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
                    "TranslatorResponse",
                    BinaryData.FromString(jsonSchema),
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

            ChatMessageContent openAiResponse =
                await _chatChatTranslationService.TranslateAsync(prompt: prompt, model: model,
                    options: chatCompletionOptions,
                    cancellationToken: cancellationToken);

            if (openAiResponse.Count == 0)
            {
                throw new TranslationException("No content returned in translation response");
            }

            string responseText = openAiResponse[0].Text;

            string jsonResponse = !string.IsNullOrWhiteSpace(responseText)
                ? responseText
                : throw new TranslationException("Translation response text is empty");

            try
            {
                TranslationInfo translation = JsonSerializer.Deserialize<TranslationInfo>(jsonResponse) ??
                                              throw new TranslationException(
                                                  "Unable to deserialize translation response");

                return translation;
            }
            catch (JsonException jsonException)
            {
                throw new TranslationException("Unable to deserialize translation response",
                    jsonException);
            }
        }
    }
}