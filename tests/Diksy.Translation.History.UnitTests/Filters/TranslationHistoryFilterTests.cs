using NUnit.Framework;
using Shouldly;
using Diksy.Translation.History.Filters;
using Diksy.Translation.History.Models;
using System.Linq.Expressions;

namespace Diksy.Translation.History.UnitTests.Filters
{
    [TestFixture]
    public class TranslationHistoryFilterTests
    {
        private static readonly DateTime TestDate = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        [Test]
        public void BuildPredicate_NoFilters_ReturnsTruePredicate()
        {
            // Arrange
            TranslationHistoryFilter filter = new();
            TranslationHistoryEntry entry = CreateEntry();

            // Act
            Expression<Func<TranslationHistoryEntry, bool>> predicateExpr = filter.BuildPredicate();
            Func<TranslationHistoryEntry, bool> predicate = predicateExpr.Compile();

            // Assert
            predicateExpr.ToString().ShouldBe("x => True"); // Check expression structure
            predicate(entry).ShouldBeTrue(); // Check compiled behavior
        }

        [Test]
        public void BuildPredicate_WithUserId_ReturnsCorrectPredicate()
        {
            // Arrange
            const string userId = "user123";
            TranslationHistoryFilter filter = new(UserId: userId);
            TranslationHistoryEntry matchingEntry = CreateEntry(userId: userId);
            TranslationHistoryEntry nonMatchingEntry = CreateEntry(userId: "otherUser");

            // Act
            Func<TranslationHistoryEntry, bool> predicate = filter.BuildPredicate().Compile();

            // Assert
            predicate(matchingEntry).ShouldBeTrue();
            predicate(nonMatchingEntry).ShouldBeFalse();
        }

        [Test]
        public void BuildPredicate_WithSourceLanguage_ReturnsCorrectPredicate()
        {
            // Arrange
            const string sourceLang = "de";
            TranslationHistoryFilter filter = new(SourceLanguage: sourceLang);
            TranslationHistoryEntry matchingEntry = CreateEntry(sourceLang: sourceLang);
            TranslationHistoryEntry nonMatchingEntry = CreateEntry(sourceLang: "fr");

            // Act
            Func<TranslationHistoryEntry, bool> predicate = filter.BuildPredicate().Compile();

            // Assert
            predicate(matchingEntry).ShouldBeTrue();
            predicate(nonMatchingEntry).ShouldBeFalse();
        }

        [Test]
        public void BuildPredicate_WithTargetLanguage_ReturnsCorrectPredicate()
        {
            // Arrange
            string targetLang = "it";
            TranslationHistoryFilter filter = new(TargetLanguage: targetLang);
            TranslationHistoryEntry matchingEntry = CreateEntry(targetLang: targetLang);
            TranslationHistoryEntry nonMatchingEntry = CreateEntry(targetLang: "pt");

            // Act
            Func<TranslationHistoryEntry, bool> predicate = filter.BuildPredicate().Compile();

            // Assert
            predicate(matchingEntry).ShouldBeTrue();
            predicate(nonMatchingEntry).ShouldBeFalse();
        }

        [Test]
        public void BuildPredicate_WithFromDate_ReturnsCorrectPredicate()
        {
            // Arrange
            DateTime fromDate = TestDate.AddDays(-1);
            TranslationHistoryFilter filter = new(FromDate: fromDate);
            TranslationHistoryEntry matchingEntry = CreateEntry(createdAt: TestDate);
            TranslationHistoryEntry exactMatchEntry = CreateEntry(createdAt: fromDate);

            TranslationHistoryEntry
                nonMatchingEntry = CreateEntry(createdAt: TestDate.AddDays(-2));

            // Act
            Func<TranslationHistoryEntry, bool> predicate = filter.BuildPredicate().Compile();

            // Assert
            predicate(matchingEntry).ShouldBeTrue();
            predicate(exactMatchEntry).ShouldBeTrue();
            predicate(nonMatchingEntry).ShouldBeFalse();
        }

        [Test]
        public void BuildPredicate_WithToDate_ReturnsCorrectPredicate()
        {
            // Arrange
            DateTime toDate = TestDate.AddDays(1);
            TranslationHistoryFilter filter = new(ToDate: toDate);
            TranslationHistoryEntry matchingEntry = CreateEntry(createdAt: TestDate);
            TranslationHistoryEntry exactMatchEntry = CreateEntry(createdAt: toDate);
            TranslationHistoryEntry
                nonMatchingEntry = CreateEntry(createdAt: TestDate.AddDays(2));

            // Act
            Func<TranslationHistoryEntry, bool> predicate = filter.BuildPredicate().Compile();

            // Assert
            predicate(matchingEntry).ShouldBeTrue();
            predicate(exactMatchEntry).ShouldBeTrue();
            predicate(nonMatchingEntry).ShouldBeFalse();
        }

        [Test]
        public void BuildPredicate_WithDateRange_ReturnsCorrectPredicate()
        {
            // Arrange
            DateTime fromDate = TestDate.AddDays(-1);
            DateTime toDate = TestDate.AddDays(1);
            TranslationHistoryFilter filter = new(FromDate: fromDate, ToDate: toDate);
            TranslationHistoryEntry matchingEntry = CreateEntry(createdAt: TestDate);
            TranslationHistoryEntry edgeEntry1 = CreateEntry(createdAt: fromDate);
            TranslationHistoryEntry edgeEntry2 = CreateEntry(createdAt: toDate);
            TranslationHistoryEntry nonMatchingEntry1 = CreateEntry(createdAt: TestDate.AddDays(-2));
            TranslationHistoryEntry nonMatchingEntry2 = CreateEntry(createdAt: TestDate.AddDays(2));

            // Act
            Func<TranslationHistoryEntry, bool> predicate = filter.BuildPredicate().Compile();

            // Assert
            predicate(matchingEntry).ShouldBeTrue();
            predicate(edgeEntry1).ShouldBeTrue();
            predicate(edgeEntry2).ShouldBeTrue();
            predicate(nonMatchingEntry1).ShouldBeFalse();
            predicate(nonMatchingEntry2).ShouldBeFalse();
        }

        [Test]
        public void BuildPredicate_WithAllFilters_ReturnsCorrectPredicate()
        {
            // Arrange
            const string userId = "specificUser";
            const string sourceLang = "en";
            const string targetLang = "fr";

            DateTime fromDate = TestDate.AddHours(-1);
            DateTime toDate = TestDate.AddHours(1);
            TranslationHistoryFilter filter = new(
                UserId: userId,
                SourceLanguage: sourceLang,
                TargetLanguage: targetLang,
                FromDate: fromDate,
                ToDate: toDate);

            TranslationHistoryEntry matchingEntry = CreateEntry(userId: userId, sourceLang: sourceLang,
                targetLang: targetLang,
                createdAt: TestDate);

            TranslationHistoryEntry wrongUserEntry = CreateEntry(userId: "wrongUser", sourceLang: sourceLang,
                targetLang: targetLang,
                createdAt: TestDate);

            TranslationHistoryEntry wrongSourceLangEntry = CreateEntry(userId: userId, sourceLang: "de",
                targetLang: targetLang,
                createdAt: TestDate);

            TranslationHistoryEntry wrongTargetLangEntry = CreateEntry(userId: userId, sourceLang: sourceLang,
                targetLang: "es",
                createdAt: TestDate);

            TranslationHistoryEntry wrongDateEntry = CreateEntry(userId: userId, sourceLang: sourceLang,
                targetLang: targetLang,
                createdAt: TestDate.AddDays(2));

            // Act
            Func<TranslationHistoryEntry, bool> predicate = filter.BuildPredicate().Compile();

            // Assert
            predicate(matchingEntry).ShouldBeTrue();
            predicate(wrongUserEntry).ShouldBeFalse();
            predicate(wrongSourceLangEntry).ShouldBeFalse();
            predicate(wrongTargetLangEntry).ShouldBeFalse();
            predicate(wrongDateEntry).ShouldBeFalse();
        }

        [Test]
        public void BuildPredicate_SortFields_AreIgnored()
        {
            // Arrange
            Dictionary<string, bool> sortFields = new() { { "CreatedAt", false } };
            TranslationHistoryFilter filter = new(SortFields: sortFields);
            TranslationHistoryEntry entry = CreateEntry();

            // Act
            Expression<Func<TranslationHistoryEntry, bool>> predicateExpr = filter.BuildPredicate();
            Func<TranslationHistoryEntry, bool> predicate = predicateExpr.Compile();

            // Assert
            predicateExpr.ToString().ShouldBe("x => True");
            predicate(entry).ShouldBeTrue();
        }

        private static TranslationHistoryEntry CreateEntry(
            string? userId = "testUser",
            string? sourceLang = "en",
            string? targetLang = "es",
            DateTime? createdAt = null)
        {
            return new TranslationHistoryEntry
            {
                UserId = userId ?? "testUser",
                SourceLanguage = sourceLang ?? "en",
                TargetLanguage = targetLang ?? "es",
                CreatedAt = createdAt ?? TestDate,
                Phrase = "test",
                Translation = "prueba",
                Transcription = "ˈprueβa",
                Example = "This is a test.",
                TranslationOfExample = "Esto es una prueba."
            };
        }
    }
}