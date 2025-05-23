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


        public async Task<TranslationInfo> TranslateAsync(string phrase, string model, string? sourceLanguage,
            string targetLanguage, CancellationToken cancellationToken)
        {
            string[] requiredProperties =
            [
                nameof(TranslationInfo.Phrase), nameof(TranslationInfo.Translation),
                nameof(TranslationInfo.Transcription), nameof(TranslationInfo.Example),
                nameof(TranslationInfo.TranslationOfExample), nameof(TranslationInfo.SourceLanguage),
                nameof(TranslationInfo.TargetLanguage)
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

            StringBuilder stringBuilder = new StringBuilder()
                .Append($"Translate the phrase \"{phrase}\" ");

            if (!string.IsNullOrWhiteSpace(sourceLanguage))
            {
                stringBuilder.Append($"from {sourceLanguage} ");
            }

            stringBuilder
                .Append($"into {targetLanguage}.")
                .AppendLine()
                .AppendLine("Please provide:")
                .AppendLine("1. Translation that captures the full meaning of the phrase/word")
                .AppendLine("2. Phonetic transcription (for each word if it's a phrasal verb)")
                .AppendLine("3. Example sentence showing proper usage in context")
                .AppendLine("4. Translation of the example sentence")
                .AppendLine("5. Source language (e.g., Polish)")
                .AppendLine("6. Target language (e.g., English)")
                .AppendLine()
                .AppendLine(
                    "Note: If this is a phrasal verb or multi-word expression, ensure the translation reflects the complete meaning rather than individual words.");

            string prompt = stringBuilder.ToString();

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
                throw new TranslationException(message: "Unable to deserialize translation response",
                    innerException: jsonException);
            }
        }
    }
}