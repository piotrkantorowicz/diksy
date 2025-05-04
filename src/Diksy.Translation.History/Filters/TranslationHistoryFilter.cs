using Diksy.Translation.History.Extensions;
using Diksy.Translation.History.Models;
using System.Linq.Expressions;

namespace Diksy.Translation.History.Filters
{
    public record TranslationHistoryFilter(
        string? UserId = null,
        string? SourceLanguage = null,
        string? TargetLanguage = null,
        DateTime? FromDate = null,
        DateTime? ToDate = null,
        IDictionary<string, bool>? SortFields = null)
    {
        public Expression<Func<TranslationHistoryEntry, bool>> BuildPredicate()
        {
            Expression<Func<TranslationHistoryEntry, bool>> predicate = x => true;

            if (!string.IsNullOrEmpty(UserId))
            {
                predicate = predicate.AndAlso(x => x.UserId == UserId);
            }

            if (!string.IsNullOrEmpty(SourceLanguage))
            {
                predicate = predicate.AndAlso(x => x.SourceLanguage == SourceLanguage);
            }

            if (!string.IsNullOrEmpty(TargetLanguage))
            {
                predicate = predicate.AndAlso(x => x.TargetLanguage == TargetLanguage);
            }

            if (FromDate.HasValue)
            {
                predicate = predicate.AndAlso(x => x.CreatedAt >= FromDate.Value);
            }

            if (ToDate.HasValue)
            {
                predicate = predicate.AndAlso(x => x.CreatedAt <= ToDate.Value);
            }

            return predicate;
        }
    }
}