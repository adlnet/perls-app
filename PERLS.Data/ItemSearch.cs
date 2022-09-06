using System;
using System.Collections.Generic;
using System.Linq;
using Float.Core.Definitions;
using PERLS.Data.Definition;

namespace PERLS.Data
{
    /// <summary>
    /// Implements a local search "engine" for searching a local list of corpus items.
    /// </summary>
    /// <remarks>
    /// This is not expected to be remotely as powerful as the search API.
    /// For one, the API has access to the entire corpus. Furthermore, it knows
    /// far more about each item than the app currently knows. It also has a more
    /// powerful library than we're currently interested in including into the mobile app
    /// (perhaps that last part will change some day).
    /// The approach here is basic: full matches are better than partial matches and
    /// matches on names are more valuable than matches on body.
    /// </remarks>
    public static class ItemSearch
    {
        /// <summary>
        /// Filters and sorts the receiver so it only contains corpus items
        /// which are relevant to the entered query and those items are in
        /// descending order by relevance.
        /// </summary>
        /// <param name="items">The list of corpus items to search.</param>
        /// <param name="query">The query to use to score each item on for relevance.</param>
        /// <returns>A filtered and sorted list of corpus items for the query.</returns>
        public static IEnumerable<IItem> Search(this IEnumerable<IItem> items, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return items;
            }

            return items
                .Select(item => (item, score: item.GetScore(query)))
                .Where(result => result.score > 0)
                .OrderByDescending(result => result.score)
                .Select(result => result.item);
        }

        /// <summary>
        /// Determines the score of the receiving item for the specified query.
        /// </summary>
        /// <param name="item">The item to score.</param>
        /// <param name="query">The query.</param>
        /// <returns>A score for the item.</returns>
        static double GetScore(this IItem item, string query)
        {
            // If the query is for a tag, veto anything which does not contain that tag.
            if (TagName.IsTagName(query))
            {
                var tagName = new TagName(query);

                if (item.Contains(tagName) == false)
                {
                    // Effectively vetos the item by downvoting it into oblivion. Bye bye.
                    return double.MinValue;
                }

                // Adjust the query to omit the tag prefix.
                query = tagName.Value;
            }

            return new RelevanceCalculator(query)
                .Consider(item.Name, RelevanceCalculator.Importance.High)
                .Consider(item.Topic, RelevanceCalculator.Importance.Medium)
                .Consider(item.Description, RelevanceCalculator.Importance.Medium)
                .Consider(item.Tags, RelevanceCalculator.Importance.Low)
                .Score();
        }

        /// <summary>
        /// Determines if an item contains a tag.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="tagName">The name of a tag.</param>
        /// <returns><c>true</c> if the item contains the tag.</returns>
        static bool Contains(this IItem item, TagName tagName)
        {
            return item.Tags?.Contains(tagName) ?? false;
        }

        /// <summary>
        /// Determines if a tag name is present in the receiving list of tags.
        /// </summary>
        /// <param name="tags">The list of tags.</param>
        /// <param name="tagName">The name of a tag.</param>
        /// <returns><c>true</c> if the list contains the tag name.</returns>
        static bool Contains(this IEnumerable<ITag> tags, TagName tagName)
        {
            return tags
                .Any(tag => tag.Name.Equals(tagName.Value, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// A tag name.
        /// </summary>
        struct TagName
        {
            const string TagPrefix = "#";

            internal TagName(string value)
            {
                if (IsTagName(value))
                {
                    Value = value.Substring(TagPrefix.Length);
                }
                else
                {
                    Value = value;
                }
            }

            internal string Value { get; }

            public override string ToString() => $"{TagPrefix}{Value}";

            internal static bool IsTagName(string value) => value.StartsWith(TagPrefix, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Based on a string query, allows a caller to boost a score by providing
        /// multiple factors (strings) and their relevance to determine the overall
        /// relevance score.
        /// </summary>
        /// <remarks>
        /// This calculator is only additive.
        /// The score starts at 0, and each factor has an opportunity to boost the score.
        /// The importance determines how much impact a factor has on the overall score.
        /// </remarks>
        class RelevanceCalculator
        {
            readonly string query;
            double score;

            internal RelevanceCalculator(string query)
            {
                this.query = query?.Trim() ?? throw new ArgumentNullException(nameof(query));
            }

            internal enum Importance : int
            {
                High = 1,
                Medium,
                Low,
            }

            internal RelevanceCalculator Consider(INamed factor, Importance relevance) => Consider(factor?.Name, relevance);

            internal RelevanceCalculator Consider(IEnumerable<INamed> factor, Importance relevance) => Consider(factor?.Select(element => element.Name), relevance);

            internal RelevanceCalculator Consider(string factor, Importance relevance)
            {
                if (!string.IsNullOrWhiteSpace(factor))
                {
                    score += CalculateFactorScore(factor, query, relevance);
                }

                return this;
            }

            internal RelevanceCalculator Consider(IEnumerable<string> factor, Importance relevance)
            {
                if (factor?.Any() == true)
                {
                    score += factor
                        .Max(value => CalculateFactorScore(value, query, relevance));
                }

                return this;
            }

            internal double Score() => score;

            static double CalculateFactorScore(string value, string query, Importance relevance)
            {
                return CalculateSimilarity(value, query) / (double)relevance;
            }

            /// <summary>
            /// Determines the similarity between a string and a query.
            /// </summary>
            /// <remarks>
            /// Extremely basic string comparer--simply checks if the value contains the query
            /// and returns a score based on the percentage of overlap.
            /// If one string contains the other, the score is better the more the two strings have in common.
            /// If the strings are exactly the same, it returns 1.
            /// </remarks>
            /// <param name="value">The source value.</param>
            /// <param name="query">The query.</param>
            /// <returns>A value between 0 (strings are not similar) and 1 (strings are exactly the same).</returns>
            static double CalculateSimilarity(string value, string query)
            {
                if (value.IndexOf(query, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    return query.Length / (double)value.Length;
                }

                return 0;
            }
        }
    }
}
