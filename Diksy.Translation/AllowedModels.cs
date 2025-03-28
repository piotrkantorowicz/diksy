namespace Diksy.Translation
{
    public static class AllowedModels
    {
        public const string Gpt4O = "gpt-4o";
        public const string Gpt4OMini = "gpt-4o-mini";

        public const string AllModelsString = $"{Gpt4O}, {Gpt4OMini}";
        public const string ModelRegex = $"^({Gpt4O}|{Gpt4OMini})$";
        private static readonly string[] AllModels = [Gpt4O, Gpt4OMini];
    }
}