using NJsonSchema;
using NJsonSchema.Generation;
using System.Text.Json;

namespace Diksy.Translation.OpenAI.Schema
{
    internal sealed class SchemaGenerator : ISchemaGenerator
    {
        public string GenerateSchema<T>(IEnumerable<string>? requiredProperties = null)
        {
            SystemTextJsonSchemaGeneratorSettings schemaGeneratorSettings = new()
            {
                DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
                SerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            };

            JsonSchema schema = JsonSchemaGenerator.FromType<T>(schemaGeneratorSettings);
            List<string> requiredPropertiesToCompare = requiredProperties?.Select(x => x.ToLower()).ToList() ?? [];

            // Set required properties by marking them as required in the schema
            foreach (KeyValuePair<string, JsonSchemaProperty> property in schema.Properties)
            {
                property.Value.IsRequired = requiredPropertiesToCompare.Contains(property.Key);
            }

            return schema.ToJson();
        }
    }
}