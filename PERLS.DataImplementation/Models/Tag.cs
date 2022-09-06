using System;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class Tag : TaxonomyTerm, ITag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tag"/> class.
        /// </summary>
        /// <remarks>
        /// Tags constructed this way will be missing UUID and other properties. The URL will be generated from the provided term ID.
        /// </remarks>
        /// <param name="tid">The term ID of the tag.</param>
        /// <param name="name">The name of the tag.</param>
        public Tag(int tid, string name) : base()
        {
            this.Tid = tid;
            this.OriginalName = name;
            this.Url = new Uri($"/taxonomy/term/{tid}", UriKind.Relative);
        }

        /// <inheritdoc />
        /// <remarks>
        /// This removes a leading hashtag (#).
        /// A tag should _not_ include the hashtag in its name, the hashtag is considered presentation.
        /// However, they do occasionally slip through, so we apply some normalization here.
        /// </remarks>
        public new string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(OriginalName) || OriginalName.Substring(0, 1) != "#")
                {
                    return OriginalName;
                }

                return OriginalName.Substring(1);
            }
        }

        /// <summary>
        /// Gets the original name as returned by the API.
        /// </summary>
        /// <value>The original name.</value>
        [JsonProperty("name")]
        public string OriginalName { get; internal set; }
    }
}
