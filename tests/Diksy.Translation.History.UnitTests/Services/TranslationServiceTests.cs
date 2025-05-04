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

namespace Diksy.Translation.History.UnitTests.Services
{
    [TestFixture]
    public class TranslationServiceTests
    {
        private Mock<ITranslator> _mockTranslator;
        private Mock<ILogger<TranslationService>> _mockLogger;
        private Mock<ITranslationHistoryService> _mockTranslationHistoryService;
        private Mock<IOptions<OpenAiOptions>> _mockOptions;
        private TranslationService _translationService;

        [SetUp]
        public void Setup()
        {
            _mockTranslator = new Mock<ITranslator>();
            _mockLogger = new Mock<ILogger<TranslationService>>();
            _mockTranslationHistoryService = new Mock<ITranslationHistoryService>();
            _mockOptions = new Mock<IOptions<OpenAiOptions>>();

            _mockOptions.Setup(o => o.Value)
                .Returns(new OpenAiOptions { DefaultModel = AllowedModels.Gpt4O, ApiKey = "test-key" });

            _translationService = new TranslationService(
                _mockTranslator.Object,
                _mockLogger.Object,
                _mockTranslationHistoryService.Object,
                _mockOptions.Object);
        }


        [Test]
        public async Task TranslateAsync_ValidInput_ReturnsSuccessResponseAndSavesHistory()
        {
            // Arrange
            const string phrase = "hello";
            const string model = AllowedModels.Gpt4O;
            const string sourceLanguage = AllowedLanguages.English;
            const string targetLanguage = AllowedLanguages.Spanish;
            CancellationToken cancellationToken = CancellationToken.None;
            TranslationInfoModel translationInfoModel = CreateValidTranslationInfoModel();

            _mockTranslator.Setup(t =>
                    t.TranslateAsync(phrase, model, sourceLanguage, targetLanguage, cancellationToken))
                .ReturnsAsync(translationInfoModel);

            _mockTranslationHistoryService.Setup(h =>
                    h.SaveTranslationAsync(It.IsAny<TranslationInfoModel>(), string.Empty, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            TranslationResponse response =
                await _translationService.TranslateAsync(phrase, model, sourceLanguage, targetLanguage,
                    cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeTrue();
            response.Response.ShouldNotBeNull();
            response.Response.Phrase.ShouldBe(phrase);
            response.Response.Translation.ShouldBe("hola");
            response.Response.SourceLanguage.ShouldBe(sourceLanguage);
            response.Response.TargetLanguage.ShouldBe(targetLanguage);
            response.Errors.ShouldBeNull();

            _mockTranslator.Verify(
                t => t.TranslateAsync(phrase, model, sourceLanguage, targetLanguage, cancellationToken), Times.Once);

            _mockTranslationHistoryService.Verify(
                h => h.SaveTranslationAsync(It.Is<TranslationInfoModel>(ti => ti.Phrase == phrase), string.Empty,
                    cancellationToken), Times.Once);
        }

        [Test]
        public async Task TranslateAsync_UsesDefaultModelAndLanguage_WhenNullProvided()
        {
            // Arrange
            const string phrase = "hello";
            const string defaultTargetLanguage = AllowedLanguages.English;
            string? model = null;
            string? sourceLanguage = null;
            string? targetLanguage = null;
            string? defaultModel = _mockOptions.Object.Value.DefaultModel;
            CancellationToken cancellationToken = CancellationToken.None;

            TranslationInfoModel translationInfoModel =
                CreateValidTranslationInfoModel(phrase, "bonjour", AllowedLanguages.English);

            _mockTranslator.Setup(t =>
                    t.TranslateAsync(phrase, defaultModel!, sourceLanguage, defaultTargetLanguage, cancellationToken))
                .ReturnsAsync(translationInfoModel);
            _mockTranslationHistoryService.Setup(h =>
                    h.SaveTranslationAsync(It.IsAny<TranslationInfoModel>(), string.Empty, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            TranslationResponse response =
                await _translationService.TranslateAsync(phrase, model, sourceLanguage, targetLanguage,
                    cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeTrue();
            response.Response.ShouldNotBeNull();
            response.Response.TargetLanguage.ShouldBe(defaultTargetLanguage);

            _mockTranslator.Verify(
                t => t.TranslateAsync(phrase, defaultModel!, sourceLanguage, defaultTargetLanguage, cancellationToken),
                Times.Once);

            _mockTranslationHistoryService.Verify(
                h => h.SaveTranslationAsync(It.IsAny<TranslationInfoModel>(), string.Empty, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task TranslateAsync_TranslatorThrowsException_ReturnsErrorResponse()
        {
            // Arrange
            const string phrase = "hello";
            const string model = AllowedModels.Gpt4O;
            const string sourceLanguage = AllowedLanguages.English;
            const string targetLanguage = AllowedLanguages.Spanish;
            CancellationToken cancellationToken = CancellationToken.None;
            string exceptionMessage = "Translator failed";

            _mockTranslator.Setup(t =>
                    t.TranslateAsync(phrase, model, sourceLanguage, targetLanguage, cancellationToken))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            TranslationResponse response =
                await _translationService.TranslateAsync(phrase, model, sourceLanguage, targetLanguage,
                    cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.Response.ShouldBeNull();
            response.Errors!.First().ShouldBe($"Translation error: {exceptionMessage}");

            _mockTranslator.Verify(
                t => t.TranslateAsync(phrase, model, sourceLanguage, targetLanguage, cancellationToken), Times.Once);

            _mockTranslationHistoryService.Verify(
                h => h.SaveTranslationAsync(It.IsAny<TranslationInfoModel>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task TranslateAsync_SanitizationThrowsTranslationException_ReturnsErrorResponse()
        {
            // Arrange
            string phrase = "hello";
            string model = AllowedModels.Gpt4O;
            string sourceLanguage = AllowedLanguages.English;
            string targetLanguage = AllowedLanguages.Spanish;
            CancellationToken cancellationToken = CancellationToken.None;

            TranslationInfoModel invalidTranslationInfoModel = CreateValidTranslationInfoModel(phrase, "");

            _mockTranslator.Setup(t =>
                    t.TranslateAsync(phrase, model, sourceLanguage, targetLanguage, cancellationToken))
                .ReturnsAsync(invalidTranslationInfoModel);

            // Act
            TranslationResponse response =
                await _translationService.TranslateAsync(phrase, model, sourceLanguage, targetLanguage,
                    cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.Response.ShouldBeNull();
            response.Errors!.First().ShouldStartWith("Translation error: Translation is null or empty");


            _mockTranslator.Verify(
                t => t.TranslateAsync(phrase, model, sourceLanguage, targetLanguage, cancellationToken), Times.Once);
            _mockTranslationHistoryService.Verify(
                h => h.SaveTranslationAsync(It.IsAny<TranslationInfoModel>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()), Times.Never); // History should not be saved
        }

        [Test]
        public async Task TranslateAsync_HistoryServiceThrowsException_ReturnsErrorResponse()
        {
            // Arrange
            const string phrase = "hello";
            string model = AllowedModels.Gpt4O;
            string sourceLanguage = AllowedLanguages.English;
            string targetLanguage = AllowedLanguages.Spanish;
            CancellationToken cancellationToken = CancellationToken.None;
            TranslationInfoModel translationInfoModel = CreateValidTranslationInfoModel();
            string historyExceptionMessage = "Failed to save history";

            _mockTranslator.Setup(t =>
                    t.TranslateAsync(phrase, model, sourceLanguage, targetLanguage, cancellationToken))
                .ReturnsAsync(translationInfoModel);

            _mockTranslationHistoryService.Setup(h =>
                    h.SaveTranslationAsync(It.IsAny<TranslationInfoModel>(), string.Empty, cancellationToken))
                .ThrowsAsync(new Exception(historyExceptionMessage));

            // Act
            TranslationResponse response =
                await _translationService.TranslateAsync(phrase, model, sourceLanguage, targetLanguage,
                    cancellationToken);

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.Response.ShouldBeNull();
            response.Errors?.First().ShouldBe($"Translation error: {historyExceptionMessage}");
            
            _mockTranslator.Verify(
                t => t.TranslateAsync(phrase, model, sourceLanguage, targetLanguage, cancellationToken), Times.Once);
            
            _mockTranslationHistoryService.Verify(
                h => h.SaveTranslationAsync(It.Is<TranslationInfoModel>(ti => ti.Phrase == phrase), string.Empty,
                    cancellationToken), Times.Once);
        }

        private static TranslationInfoModel CreateValidTranslationInfoModel(string phrase = "hello",
            string translation = "hola", string targetLanguage = AllowedLanguages.Spanish)
        {
            return new TranslationInfoModel
            {
                Phrase = phrase,
                Translation = translation,
                Transcription = "həˈloʊ",
                Example = "Say hello",
                TranslationOfExample = "Di hola",
                SourceLanguage = AllowedLanguages.English,
                TargetLanguage = targetLanguage
            };
        }
    }
}