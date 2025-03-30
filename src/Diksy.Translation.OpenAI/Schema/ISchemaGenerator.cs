namespace Diksy.Translation.OpenAI.Schema
{
    public interface ISchemaGenerator
    {
        string GenerateSchema<T>(IEnumerable<string>? requiredProperties = null);
    }
}