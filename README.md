# Diksy - AI-Powered Translation Service

A modern web API service that provides translation capabilities using OpenAI's language models.

## Features

- Translate phrases between multiple languages
- Get phonetic transcriptions
- Receive example usage in context
- Support for multiple AI models
- RESTful API with OpenAPI documentation
- Built with .NET 9.0

## Supported Languages

- English
- Spanish
- French
- German
- Italian
- Portuguese
- Russian
- Chinese
- Japanese
- Korean
- Arabic
- Hindi
- Dutch
- Polish
- Turkish

## Prerequisites

- .NET 9.0 SDK
- OpenAI API key

## Configuration

1. Clone the repository
2. Navigate to the `Diksy.WebApi` directory
3. Update `appsettings.json` with your OpenAI API key:

```json
{
  "OpenAI": {
    "ApiKey": "your-api-key-here",
    "DefaultModel": "gpt-4o"
  }
}
```

## Running the Application

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the application
cd Diksy.WebApi
dotnet run
```

The API will be available at:

- Swagger UI: https://localhost:5001/swagger
- ReDoc: https://localhost:5001/redoc

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
```

## API Usage

### Translate Endpoint

```http
POST /api/Translation
Content-Type: application/json

{
    "phrase": "Hello world",
    "model": "gpt-4o",
    "language": "Spanish"
}
```

### Response

```json
{
    "success": true,
    "result": {
        "phrase": "Hello world",
        "translation": "Hola mundo",
        "transcription": "ˈoʊlɑ ˈmundoʊ",
        "example": "¡Hola mundo! ¿Cómo estás?"
    },
    "errors": []
}
```

## Project Structure

- `Diksy.WebApi` - Main API project
- `Diksy.Translation` - Core translation models and interfaces
- `Diksy.Translation.OpenAI` - OpenAI integration implementation
- `Diksy.Translation.OpenAI.UnitTests` - Unit tests for OpenAI integration

## Development

The project uses:

- EditorConfig for consistent code style
- NSwag for API documentation
- Dependency Injection
- Logging with ILogger
- NUnit for testing
- Moq for mocking
- Shouldly for assertions

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details. 