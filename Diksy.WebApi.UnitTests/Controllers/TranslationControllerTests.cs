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
using System.Net;

namespace Diksy.WebApi.UnitTests.Controllers
{
    [TestFixture]
    public class TranslationControllerTests
    {
        private Mock<ITranslationService> _translationServiceMock;
        private Mock<ILogger<TranslationController>> _loggerMock;
        private TranslationController _controller;

        [SetUp]
        public void SetUp()
        {
            _translationServiceMock = new Mock<ITranslationService>();
            _loggerMock = new Mock<ILogger<TranslationController>>();
            
            _controller = new TranslationController(
                translationService: _translationServiceMock.Object,
                logger: _loggerMock.Object);
        }

        [Test]
        public async Task Translate_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new TranslationRequest
            {
                Phrase = "Hello",
                Model = "gpt-4o",
                Language = "Spanish"
            };

            var translationInfo = new TranslationInfo
            {
                Phrase = "Hello",
                Translation = "Hola",
                Transcription = "həˈloʊ",
                Example = "Hola, ¿cómo estás?",
                TranslationOfExample = "Hello, how are you?"
            };

            var expectedResponse = TranslationResponse.SuccessResponse(translationInfo);

            _translationServiceMock.Setup(s => s.TranslateAsync(
                request.Phrase, request.Model, request.Language, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Translate(request);

            // Assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
            
            var responseValue = okResult.Value.ShouldBeOfType<TranslationResponse>();
            responseValue.Success.ShouldBeTrue();
            responseValue.Response.ShouldNotBeNull();
            responseValue.Response.Phrase.ShouldBe("Hello");
            responseValue.Response.Translation.ShouldBe("Hola");
            responseValue.Response.Transcription.ShouldBe("həˈloʊ");
            responseValue.Response.Example.ShouldBe("Hola, ¿cómo estás?");
            responseValue.Response.TranslationOfExample.ShouldBe("Hello, how are you?");
        }

        [Test]
        public async Task Translate_WithInvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new TranslationRequest
            {
                Phrase = "Hi",
                Model = "gpt-4o",
                Language = "Spanish"
            };

            _controller.ModelState.AddModelError("Phrase", "The field Phrase must be a string with a minimum length of 3 and a maximum length of 30.");

            // Act
            var result = await _controller.Translate(request);

            // Assert
            var badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
            
            var problemDetails = badRequestResult.Value.ShouldBeOfType<ApiProblemDetails>();
            problemDetails.Title.ShouldBe("Validation Failed");
            problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
            problemDetails?.Errors?.ShouldContainKey("Phrase");
        }

        [Test]
        public async Task Translate_WhenTranslationFails_ReturnsInternalServerError()
        {
            // Arrange
            var request = new TranslationRequest
            {
                Phrase = "Hello",
                Model = "gpt-4o",
                Language = "Spanish"
            };

            var failedResponse = new TranslationResponse
            {
                Success = false,
                Errors = ["Translation service error"]
            };

            _translationServiceMock.Setup(s => s.TranslateAsync(
                request.Phrase, request.Model, request.Language, It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResponse);

            // Act
            var result = await _controller.Translate(request);

            // Assert
            var statusCodeResult = result.ShouldBeOfType<ObjectResult>();
            statusCodeResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            
            var problemDetails = statusCodeResult.Value.ShouldBeOfType<ApiProblemDetails>();
            problemDetails.Title.ShouldBe("Translation Error");
            problemDetails.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        }
    }
} 