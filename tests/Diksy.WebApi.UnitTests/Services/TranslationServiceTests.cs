using Diksy.Translation;
using Diksy.Translation.Exceptions;
using Diksy.Translation.OpenAI;
using Diksy.Translation.Services;
using Diksy.WebApi.Models.Translation;
using Diksy.WebApi.Services;
using Microsoft.Extensions.Logging;
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
            _openAiSettings = new OpenAiSettings(ApiKey: "test-api-key", DefaultModel: AllowedModels.Gpt4O);

            _service = new TranslationService(
                translator: _translatorMock.Object,
                logger: _loggerMock.Object,
                openAiSettings: _openAiSettings);
        }

        private Mock<ITranslator> _translatorMock;
        private Mock<ILogger<TranslationService>> _loggerMock;
        private OpenAiSettings _openAiSettings;
        private TranslationService _service;

        [Test]
        public async Task TranslateAsync_WithValidInput_ReturnsSuccessfulResponse()
        {
            // Arrange
            const string phrase = "Hello";
            const string model = "gpt-4o";
            const string language = "Spanish";

            TranslationInfoModel translationInfoModel = new()
            {
                Phrase = phrase,
                Translation = "Hola",
                Transcription = "həˈloʊ",
                Example = "Hola, ¿cómo estás?",
                TranslationOfExample = "Hello, how are you?"
            };

            _translatorMock.Setup(t => t.TranslateAsync(
                    phrase, model, language, It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationInfoModel);

            // Act
            TranslationResponse result = await _service.TranslateAsync(phrase: phrase, model: model, language: language,
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
        }

        [Test]
        public async Task TranslateAsync_WithNullModel_UsesDefaultModel()
        {
            // Arrange
            const string phrase = "Hello";
            const string language = "Spanish";
            string? model = null;
            string expectedModel = _openAiSettings.DefaultModel ?? AllowedModels.Gpt4O;

            TranslationInfoModel translationInfoModel = new()
            {
                Phrase = phrase,
                Translation = "Hola",
                Transcription = "həˈloʊ",
                Example = "Hola, ¿cómo estás?",
                TranslationOfExample = "Hello, how are you?"
            };

            _translatorMock.Setup(t => t.TranslateAsync(
                    phrase, expectedModel, language, It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationInfoModel);

            // Act
            TranslationResponse result = await _service.TranslateAsync(phrase: phrase, model: model, language: language,
                cancellationToken: CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            _translatorMock.Verify(
                expression: t => t.TranslateAsync(phrase, expectedModel, language, It.IsAny<CancellationToken>()),
                times: Times.Once);
        }

        [Test]
        public async Task TranslateAsync_WithNullLanguage_UsesEnglishLanguage()
        {
            // Arrange
            const string phrase = "Hello";
            const string model = "gpt-4o";
            const string expectedLanguage = AllowedLanguages.English;
            string? language = null;
            
            TranslationInfoModel translationInfoModel = new()
            {
                Phrase = phrase,
                Translation = "Hola",
                Transcription = "həˈloʊ",
                Example = "Hola, ¿cómo estás?",
                TranslationOfExample = "Hello, how are you?"
            };

            _translatorMock.Setup(t => t.TranslateAsync(
                    phrase, model, expectedLanguage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationInfoModel);

            // Act
            TranslationResponse result = await _service.TranslateAsync(phrase: phrase, model: model, language: language,
                cancellationToken: CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            _translatorMock.Verify(
                expression: t => t.TranslateAsync(phrase, model, expectedLanguage, It.IsAny<CancellationToken>()),
                times: Times.Once);
        }

        [Test]
        public async Task TranslateAsync_WhenTranslatorThrowsException_ReturnsFailureResponse()
        {
            // Arrange
            const string phrase = "Hello";
            const string model = "gpt-4o";
            const string language = "Spanish";
            const string errorMessage = "Translation error";

            _translatorMock.Setup(t => t.TranslateAsync(
                    phrase, model, language, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TranslationException(errorMessage));

            // Act
            TranslationResponse result = await _service.TranslateAsync(phrase: phrase, model: model, language: language,
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
            const string language = "Spanish";

            TranslationInfoModel translationInfoModel = new()
            {
                Phrase = "Different",
                Translation = "Hola",
                Transcription = "həˈloʊ",
                Example = "Hola, ¿cómo estás?",
                TranslationOfExample = "Hello, how are you?"
            };

            _translatorMock.Setup(t => t.TranslateAsync(
                    phrase, model, language, It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationInfoModel);

            // Act
            TranslationResponse result = await _service.TranslateAsync(phrase: phrase, model: model, language: language,
                cancellationToken: CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Response.ShouldBeNull();
            result.Errors.ShouldNotBeNull();
        }
    }
}