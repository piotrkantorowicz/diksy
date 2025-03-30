namespace Diksy.Translation
{
    public static class AllowedLanguages
    {
        public const string English = "English";
        public const string Spanish = "Spanish";
        public const string French = "French";
        public const string German = "German";
        public const string Italian = "Italian";
        public const string Portuguese = "Portuguese";
        public const string Russian = "Russian";
        public const string Chinese = "Chinese";
        public const string Japanese = "Japanese";
        public const string Korean = "Korean";
        public const string Arabic = "Arabic";
        public const string Hindi = "Hindi";
        public const string Dutch = "Dutch";
        public const string Polish = "Polish";
        public const string Turkish = "Turkish";

        public const string AllLanguagesString =
            "English, Spanish, French, German, Italian, Portuguese, Russian, Chinese, Japanese, Korean, Arabic, Hindi, Dutch, Polish, Turkish";

        public const string LanguageRegex =
            "^(English|Spanish|French|German|Italian|Portuguese|Russian|Chinese|Japanese|Korean|Arabic|Hindi|Dutch|Polish|Turkish)$";

        private static readonly string[] AllLanguages =
        [
            English, Spanish, French, German, Italian, Portuguese, Russian,
            Chinese, Japanese, Korean, Arabic, Hindi, Dutch, Polish, Turkish
        ];
    }
}