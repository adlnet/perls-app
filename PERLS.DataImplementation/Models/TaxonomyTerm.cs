using System;
using System.ComponentModel;
using JsonSubTypes;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A Drupal taxonomy term.
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(JsonSubtypes), nameof(Type))]
    [JsonSubtypes.KnownSubType(typeof(Tag), "tags")]
    [JsonSubtypes.KnownSubType(typeof(Topic), "category")]
    public abstract class TaxonomyTerm : DrupalEntity, ITaxonomyTerm
    {
        /// <inheritdoc />
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the term ID.
        /// </summary>
        /// <value>The term id.</value>
        public int Tid { get; internal set; }
    }
}
