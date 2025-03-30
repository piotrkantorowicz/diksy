using Diksy.WebApi.Controllers;
using Diksy.WebApi.Models;
using Diksy.WebApi.Models.Translation;
using Diksy.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Diksy.WebApi.UnitTests.Controllers
{
    [TestFixture]
    public class TranslationControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            _translationServiceMock = new Mock<ITranslationService>();
            _loggerMock = new Mock<ILogger<TranslationController>>();

            _controller = new TranslationController(
                translationService: _translationServiceMock.Object,
                logger: _loggerMock.Object);
        }

        private Mock<ITranslationService> _translationServiceMock;
        private Mock<ILogger<TranslationController>> _loggerMock;
        private TranslationController _controller;

        [Test]
        public async Task Translate_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            TranslationRequest request = new() { Phrase = "Hello", Model = "gpt-4o", Language = "Spanish" };

            TranslationInfo translationInfo = new()
            {
                Phrase = "Hello",
                Translation = "Hola",
                Transcription = "həˈloʊ",
                Example = "Hola, ¿cómo estás?",
                TranslationOfExample = "Hello, how are you?"
            };

            TranslationResponse expectedResponse = TranslationResponse.SuccessResponse(translationInfo);

            _translationServiceMock.Setup(s => s.TranslateAsync(
                    request.Phrase, request.Model, request.Language, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            IActionResult result = await _controller.Translate(request);

            // Assert
            OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
            okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);

            TranslationResponse responseValue = okResult.Value.ShouldBeOfType<TranslationResponse>();
            responseValue.Success.ShouldBeTrue();
            responseValue.Response.ShouldNotBeNull();
            responseValue.Response.Phrase.ShouldBe("Hello");
            responseValue.Response.Translation.ShouldBe("Hola");
            responseValue.Response.Transcription.ShouldBe("həˈloʊ");
            responseValue.Response.Example.ShouldBe("Hola, ¿cómo estás?");
            responseValue.Response.TranslationOfExample.ShouldBe("Hello, how are you?");

            _translationServiceMock.Verify(expression: s => s.TranslateAsync(
                    request.Phrase, request.Model, request.Language, It.IsAny<CancellationToken>()),
                times: Times.Once);
        }

        [Test]
        public async Task Translate_WithInvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            TranslationRequest request = new() { Phrase = "Hi", Model = "gpt-4o", Language = "Spanish" };

            _controller.ModelState.AddModelError(key: "Phrase",
                errorMessage:
                "The field Phrase must be a string with a minimum length of 3 and a maximum length of 30.");

            // Act
            IActionResult result = await _controller.Translate(request);

            // Assert
            BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);

            ApiProblemDetails problemDetails = badRequestResult.Value.ShouldBeOfType<ApiProblemDetails>();
            problemDetails.Title.ShouldBe("Validation Failed");
            problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
            problemDetails?.Errors?.ShouldContainKey("Phrase");

            _translationServiceMock.Verify(expression: s => s.TranslateAsync(
                    request.Phrase, request.Model, request.Language, It.IsAny<CancellationToken>()),
                times: Times.Never);
        }

        [Test]
        public async Task Translate_WhenTranslationFails_ReturnsInternalServerError()
        {
            // Arrange
            TranslationRequest request = new() { Phrase = "Hello", Model = "gpt-4o", Language = "Spanish" };

            TranslationResponse failedResponse = new() { Success = false, Errors = ["Translation service error"] };

            _translationServiceMock.Setup(s => s.TranslateAsync(
                    request.Phrase, request.Model, request.Language, It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResponse);

            // Act
            IActionResult result = await _controller.Translate(request);

            // Assert
            ObjectResult statusCodeResult = result.ShouldBeOfType<ObjectResult>();
            statusCodeResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);

            ApiProblemDetails problemDetails = statusCodeResult.Value.ShouldBeOfType<ApiProblemDetails>();
            problemDetails.Title.ShouldBe("Translation Error");
            problemDetails.Status.ShouldBe(StatusCodes.Status500InternalServerError);

            _translationServiceMock.Verify(expression: s => s.TranslateAsync(
                    request.Phrase, request.Model, request.Language, It.IsAny<CancellationToken>()),
                times: Times.Once);
        }
    }
}