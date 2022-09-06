using System;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class QuizOption : IQuizOption
    {
        /// <inheritdoc />
        [JsonProperty("answer")]
        public string Text { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("rationale")]
        public string Feedback { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("correct")]
        public bool IsCorrect { get; internal set; } = false;
    }
}
