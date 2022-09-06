using System;
using System.ComponentModel;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The Learner implementation.
    /// </summary>
    [Serializable]
    public class Learner : DrupalEntity, ILearner
    {
        /// <inheritdoc />
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public new string Name => string.IsNullOrWhiteSpace(DisplayName) ? Username : DisplayName;

        /// <inheritdoc />
        [JsonProperty("mail")]
        public string Email { get; internal set; }

        /// <summary>
        /// Gets the user's full name.
        /// </summary>
        /// <value>The user's full name.</value>
        [JsonProperty("name")]
        public string DisplayName { get; internal set; }

        /// <summary>
        /// Gets the Drupal username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; internal set; }

        /// <summary>
        /// Gets the learner's avatar.
        /// </summary>
        /// <remarks>It is known that only the Url gets populated on the file object--it's not an issue.</remarks>
        /// <value>The avatar.</value>
        public File Avatar { get; internal set; }

        /// <summary>
        /// Gets the learner's goals.
        /// </summary>
        /// <value>
        /// The learner's goals.
        /// </value>
        [JsonProperty("goals")]
        public LearnerGoals LearnerGoals { get; internal set; }

        /// <inheritdoc />
        IFile ILearner.Avatar => Avatar;

        /// <inheritdoc />
        public string EditPath => Url?.OriginalString is string url ? $"{url}/edit" : string.Empty;

        /// <inheritdoc/>
        ILearnerGoals ILearner.LearnerGoals => LearnerGoals;

        /// <inheritdoc/>
        [JsonProperty("preferred_langcode")]
        public string PreferredLanguage { get; internal set; }
    }
}
