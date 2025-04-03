using Diksy.Translation.Exceptions;
using Diksy.Translation.Models;
using Diksy.Translation.OpenAI.Schema;
using Diksy.Translation.OpenAI.Services;
using Diksy.Translation.Services;
using Moq;
using NUnit.Framework;
using OpenAI.Chat;
using Shouldly;
using System.Text.Json;

namespace Diksy.Translation.OpenAI.UnitTests
{
    [TestFixture]
    public class OpenAiTranslatorTests
    {
        [SetUp]
        public void SetUp()
        {
            _chatClientTranslationServiceMock = new Mock<IChatTranslationService>();
            _schemaGeneratorMock = new SchemaGenerator();
            _translator = new OpenAiTranslator(chatChatTranslationService: _chatClientTranslationServiceMock.Object,
                schemaGenerator: _schemaGeneratorMock);
        }

        private Mock<IChatTranslationService> _chatClientTranslationServiceMock;
        private ISchemaGenerator _schemaGeneratorMock;
        private ITranslator _translator;

        [Test]
        public async Task TranslateAsync_ShouldReturnTranslationInfo_WhenResponseIsValid()
        {
            // Arrange
            const string expectedJsonResponse =
                "{\"phrase\":\"Hello\",\"translation\":\"Hola\",\"transcription\":\"həˈloʊ\",\"example\":\"Hola, ¿cómo estás?\", \"translationOfExample\":\"Hello, how are you?\"}";

            TranslationInfo? expectedTranslationInfo =
                JsonSerializer.Deserialize<TranslationInfo>(expectedJsonResponse);

            _chatClientTranslationServiceMock.Setup(c => c.TranslateAsync(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<ChatCompletionOptions>(), CancellationToken.None))
                .ReturnsAsync(new ChatMessageContent(expectedJsonResponse));

            // Act
            TranslationInfo result = await _translator.TranslateAsync(word: "Hello", model: "gpt-4o",
                language: "Spanish", cancellationToken: It.IsAny<CancellationToken>());

            // Assert
            result.ShouldNotBeNull();
            result.Phrase.ShouldBe(expectedTranslationInfo?.Phrase);
            result.Translation.ShouldBe(expectedTranslationInfo?.Translation);
            result.Transcription.ShouldBe(expectedTranslationInfo?.Transcription);
            result.Example.ShouldBe(expectedTranslationInfo?.Example);
            result.TranslationOfExample.ShouldBe(expectedTranslationInfo?.TranslationOfExample);
        }

        [Test]
        public async Task TranslateAsync_ShouldThrowTranslationException_WhenResponseIsEmpty()
        {
            // Arrange
            _chatClientTranslationServiceMock.Setup(c => c.TranslateAsync(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<ChatCompletionOptions>(), CancellationToken.None))
                .ReturnsAsync(new ChatMessageContent(string.Empty));

            // Act & Assert
            await Should.ThrowAsync<TranslationException>(async () =>
                await _translator.TranslateAsync(word: "Hello", model: "gpt-4o", language: "Spanish",
                    cancellationToken: It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task TranslateAsync_ShouldThrowTranslationException_WhenResponseIsInvalid()
        {
            // Arrange
            _chatClientTranslationServiceMock.Setup(c => c.TranslateAsync(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<ChatCompletionOptions>(), CancellationToken.None))
                .ReturnsAsync(new ChatMessageContent("Invalid response"));

            // Act & Assert
            await Should.ThrowAsync<TranslationException>(async () =>
                await _translator.TranslateAsync(word: "Hello", model: "gpt-4o", language: "Spanish",
                    cancellationToken: It.IsAny<CancellationToken>()));
        }
    }
}