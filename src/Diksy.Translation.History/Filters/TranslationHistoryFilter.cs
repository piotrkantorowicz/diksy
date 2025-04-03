namespace Diksy.Translation.History.Filters
{
    public class TranslationHistoryFilter
    {
        public string? UserId { get; init; }
        public string? SourceLanguage { get; init; }
        public string? TargetLanguage { get; init; }
        public DateTime? FromDate { get; init; }
        public DateTime? ToDate { get; init; }
    }
}