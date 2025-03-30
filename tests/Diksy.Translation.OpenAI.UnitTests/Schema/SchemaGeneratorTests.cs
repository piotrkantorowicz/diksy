using Diksy.Translation.Models;
using Diksy.Translation.OpenAI.Schema;
using NUnit.Framework;
using Shouldly;
using System.Text.Json.Nodes;

namespace Diksy.Translation.OpenAI.UnitTests.Schema
{
    [TestFixture]
    public class SchemaGeneratorTests
    {
        [SetUp]
        public void SetUp()
        {
            _schemaGenerator = new SchemaGenerator();
        }

        private ISchemaGenerator _schemaGenerator;

        private readonly string[] _requiredProperties =
        [
            nameof(TranslationInfo.Phrase),
            nameof(TranslationInfo.Translation),
            nameof(TranslationInfo.Transcription),
            nameof(TranslationInfo.Example)
        ];

        [Test]
        public void GenerateSchema_WithRequiredProperties_SetsPropertiesAsRequired()
        {
            // Arrange
            // Act
            string schema = _schemaGenerator.GenerateSchema<TranslationInfo>(_requiredProperties);

            // Assert
            AssertRequiredPropertiesToContain(schema: schema, requiredProperties: _requiredProperties);
        }

        [Test]
        public void GenerateSchema_WithoutRequiredProperties_SetsAllPropertiesAsRequired()
        {
            // Act
            string schema = _schemaGenerator.GenerateSchema<TranslationInfo>();

            // Assert
            AssertRequiredPropertiesToBeNullOrEmpty(schema);
        }

        [Test]
        public void GenerateSchema_WithEmptyRequiredProperties_SetsAllPropertiesAsRequired()
        {
            // Act
            string schema = _schemaGenerator.GenerateSchema<TranslationInfo>([]);

            // Assert
            AssertRequiredPropertiesToBeNullOrEmpty(schema);
        }

        [Test]
        public void GenerateSchema_WithNullRequiredProperties_SetsAllPropertiesAsRequired()
        {
            // Act
            string schema = _schemaGenerator.GenerateSchema<TranslationInfo>();

            // Assert
            AssertRequiredPropertiesToBeNullOrEmpty(schema);
        }

        [Test]
        public void GenerateSchema_ShouldIncludeCorrectPropertyTypes()
        {
            // Act
            string schema = _schemaGenerator.GenerateSchema<TranslationInfo>();

            // Assert
            JsonNode? jsonSchema = JsonNode.Parse(schema);
            JsonNode? properties = jsonSchema?["properties"];

            properties?["phrase"]?["type"]?.GetValue<string>().ShouldBe("string");
            properties?["translation"]?["type"]?.GetValue<string>().ShouldBe("string");
            properties?["transcription"]?["type"]?.GetValue<string>().ShouldBe("string");
            properties?["example"]?["type"]?.GetValue<string>().ShouldBe("string");
        }


        [Test]
        public void GenerateSchema_ShouldReturnValidJsonString()
        {
            // Act
            string schema = _schemaGenerator.GenerateSchema<TranslationInfo>();

            // Assert
            _ = Should.NotThrow(() => JsonNode.Parse(schema));
        }

        private static void AssertRequiredPropertiesToContain(string schema, string[] requiredProperties)
        {
            JsonNode? jsonSchema = JsonNode.Parse(schema);
            JsonArray? requiredPropertiesNode = jsonSchema?["required"]?.AsArray();
            List<string?>? requiredPropertiesCollection =
                requiredPropertiesNode?.Select(x => x?.ToString().Trim()).ToList();

            requiredPropertiesNode.ShouldNotBeNull();

            foreach (string? requiredProperty in requiredProperties)
            {
                requiredPropertiesCollection?.ShouldContain(requiredProperty.ToLower());
            }
        }

        private static void AssertRequiredPropertiesToBeNullOrEmpty(string schema)
        {
            JsonNode? jsonSchema = JsonNode.Parse(schema);
            JsonNode? requiredPropertiesNode = jsonSchema?["required"];
            requiredPropertiesNode.ShouldBeNull();
        }
    }
}