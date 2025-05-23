using Diksy.Translation;
using Diksy.Translation.Exceptions;
using Diksy.Translation.History.Services;
using Diksy.Translation.OpenAI;
using Diksy.Translation.Services;
using Diksy.WebApi.Models.Translation;
using Diksy.WebApi.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Shouldly;
using TranslationInfoModel = Diksy.Translation.Models.TranslationInfo;

namespace Diksy.WebApi.UnitTests.Services
{
    [TestFixture]
    public class TranslationServiceTests
    {
        [SetUp]
        public void SetUp()
        {
            _translatorMock = new Mock<ITranslator>();
            _loggerMock = new Mock<ILogger<TranslationService>>();
            _openAiOptions = new Mock<IOptions<OpenAiOptions>>();
            _translationHistoryService = new Mock<ITranslationHistoryService>();

            _openAiOptions.SetupGet(x => x.Value)
                .Returns(new OpenAiOptions { ApiKey = "test-api-key", DefaultModel = AllowedModels.Gpt4O });

            _service = new TranslationService(
                translator: _translatorMock.Object,
                logger: _loggerMock.Object,
                translationHistoryService: _translationHistoryService.Object,
                openAiOptions: _openAiOptions.Object);
        }

        private Mock<ITranslator> _translatorMock;
        private Mock<ILogger<TranslationService>> _loggerMock;
        private Mock<IOptions<OpenAiOptions>> _openAiOptions;
        private Mock<ITranslationHistoryService> _translationHistoryService;
        private TranslationService _service;

        [Test]
        public async Task TranslateAsync_WithValidInput_ReturnsSuccessfulResponse()
        {
            // Arrange
            const string phrase = "Hello";
            const string model = "gpt-4o";
            const string targetLanguage = AllowedLanguages.Spanish;
            const string sourceLanguage = AllowedLanguages.English;

            TranslationInfoModel translationInfoModel = new()
            {
                Phrase = phrase,
                Translation = "Hola",
                Transcription = "həˈloʊ",
                Example = "Hola, ¿cómo estás?",
                TranslationOfExample = "Hello, how are you?",
                SourceLanguage = sourceLanguage,
                TargetLanguage = targetLanguage
            };

            _translatorMock.Setup(t => t.TranslateAsync(
                    phrase, model, sourceLanguage, targetLanguage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationInfoModel);

            // Act
            TranslationResponse result = await _service.TranslateAsync(phrase: phrase, model: model,
                targetLanguage: targetLanguage, sourceLanguage: sourceLanguage,
                cancellationToken: CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Response.ShouldNotBeNull();
            result.Response.Phrase.ShouldBe(phrase);
            result.Response.Translation.ShouldBe("Hola");
            result.Response.Transcription.ShouldBe("həˈloʊ");
            result.Response.Example.ShouldBe("Hola, ¿cómo estás?");
            result.Response.TranslationOfExample.ShouldBe("Hello, how are you?");
            result.Response.SourceLanguage.ShouldBe(AllowedLanguages.English);
            result.Response.TargetLanguage.ShouldBe(AllowedLanguages.Spanish);

            _translatorMock.Verify(
                expression: t =>
                    t.TranslateAsync(phrase, model, sourceLanguage, targetLanguage, It.IsAny<CancellationToken>()),
                times: Times.Once);

            _translationHistoryService.Verify(
                expression: t => t.SaveTranslationAsync(It.IsAny<TranslationInfoModel>(), string.Empty,
                    It.IsAny<CancellationToken>()),
                times: Times.Once);
        }

        [Test]
        public async Task TranslateAsync_WithNullModel_UsesDefaultModel()
        {
            // Arrange
            const string phrase = "Hello";
            const string targetLanguage = AllowedLanguages.Spanish;
            const string sourceLanguage = AllowedLanguages.English;
            string? model = null;
            string expectedModel = _openAiOptions.Object.Value.DefaultModel ?? AllowedModels.Gpt4O;

            TranslationInfoModel translationInfoModel = new()
            {
                Phrase = phrase,
                Translation = "Hola",
                Transcription = "həˈloʊ",
                Example = "Hola, ¿cómo estás?",
                TranslationOfExample = "Hello, how are you?",
                SourceLanguage = sourceLanguage,
                TargetLanguage = targetLanguage
            };

            _translatorMock.Setup(t => t.TranslateAsync(
                    phrase, expectedModel, sourceLanguage, targetLanguage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationInfoModel);

            // Act
            TranslationResponse result = await _service.TranslateAsync(phrase: phrase, model: model,
                targetLanguage: targetLanguage, sourceLanguage: sourceLanguage,
                cancellationToken: CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            _translatorMock.Verify(
                expression: t => t.TranslateAsync(phrase, expectedModel, sourceLanguage, targetLanguage,
                    It.IsAny<CancellationToken>()),
                times: Times.Once);

            _translationHistoryService.Verify(
                expression: t => t.SaveTranslationAsync(It.IsAny<TranslationInfoModel>(), string.Empty,
                    It.IsAny<CancellationToken>()),
                times: Times.Once);
        }

        [Test]
        public async Task TranslateAsync_WithNullLanguage_UsesEnglishLanguage()
        {
            // Arrange
            const string phrase = "Hello";
            const string model = "gpt-4o";
            const string sourceLanguage = AllowedLanguages.English;
            const string expectedTargetLanguage = AllowedLanguages.English;
            string language = null!;

            TranslationInfoModel translationInfoModel = new()
            {
                Phrase = phrase,
                Translation = "Hola",
                Transcription = "həˈloʊ",
                Example = "Hola, ¿cómo estás?",
                TranslationOfExample = "Hello, how are you?",
                SourceLanguage = sourceLanguage,
                TargetLanguage = expectedTargetLanguage
            };

            _translatorMock.Setup(t => t.TranslateAsync(
                    phrase, model, sourceLanguage, expectedTargetLanguage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationInfoModel);

            // Act
            TranslationResponse result = await _service.TranslateAsync(phrase: phrase, model: model,
                targetLanguage: language, sourceLanguage: sourceLanguage,
                cancellationToken: CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            _translatorMock.Verify(
                expression: t =>
                    t.TranslateAsync(phrase, model, sourceLanguage, expectedTargetLanguage, It.IsAny<CancellationToken>()),
                times: Times.Once);

            _translationHistoryService.Verify(
                expression: t => t.SaveTranslationAsync(It.IsAny<TranslationInfoModel>(), string.Empty,
                    It.IsAny<CancellationToken>()),
                times: Times.Once);
        }

        [Test]
        public async Task TranslateAsync_WhenTranslatorThrowsException_ReturnsFailureResponse()
        {
            // Arrange
            const string phrase = "Hello";
            const string model = "gpt-4o";
            const string targetLanguage = AllowedLanguages.Spanish;
            const string sourceLanguage = AllowedLanguages.English;
            const string errorMessage = "Translation error";

            _translatorMock.Setup(t => t.TranslateAsync(
                    phrase, model, sourceLanguage, targetLanguage, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TranslationException(errorMessage));

            // Act
            TranslationResponse result = await _service.TranslateAsync(phrase: phrase, model: model,
                targetLanguage: targetLanguage, sourceLanguage: sourceLanguage,
                cancellationToken: CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Response.ShouldBeNull();
            result.Errors.ShouldNotBeNull();
            result.Errors.ShouldContain(e => e.Contains(errorMessage));
        }

        [Test]
        public async Task TranslateAsync_WhenTranslatorReturnsInvalidResponse_ReturnsFailureResponse()
        {
            // Arrange
            const string phrase = "Hello";
            const string model = "gpt-4o";
            const string targetLanguage = AllowedLanguages.Spanish;
            const string sourceLanguage = AllowedLanguages.English;

            TranslationInfoModel translationInfoModel = new()
            {
                Phrase = "Different",
                Translation = "Hola",
                Transcription = "həˈloʊ",
                Example = "Hola, ¿cómo estás?",
                TranslationOfExample = "Hello, how are you?",
                SourceLanguage = sourceLanguage,
                TargetLanguage = targetLanguage
            };

            _translatorMock.Setup(t => t.TranslateAsync(
                    phrase, model, sourceLanguage, targetLanguage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationInfoModel);

            // Act
            TranslationResponse result = await _service.TranslateAsync(phrase: phrase, model: model,
                sourceLanguage: sourceLanguage, targetLanguage: targetLanguage,
                cancellationToken: CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Response.ShouldBeNull();
            result.Errors.ShouldNotBeNull();
        }
    }
}