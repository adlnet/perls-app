using System;
using System.Collections.Generic;
using System.ComponentModel;
using Float.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The Prompt implementation.
    /// </summary>
    [Serializable]
    public class Prompt : IPrompt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Prompt"/> class.
        /// </summary>
        /// <param name="uuid">The UUID.</param>
        /// <param name="form">The "form" which is based on the return from the CMS.</param>
        [JsonConstructor]
        public Prompt(string uuid, Dictionary<string, object> form)
        {
            if (uuid == null)
            {
                throw new ArgumentNullException(nameof(uuid));
            }

            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            Id = new Guid(uuid);
            var questions = form["questions"] as JArray;
            var questionDictionary = questions[0] as JObject;
            var questionQuestionArray = questionDictionary["question"] as JArray;
            Question = (string)questionQuestionArray[0];

            var optionsDictionary = questionDictionary["options"] as JObject;

            Options = new List<PromptOption>();

            optionsDictionary.Children().ForEach(each =>
            {
                Options.Add(new PromptOption()
                {
                    Key = (each as JProperty).Name,
                    Value = (string)((each as JProperty).Value as JValue).Value,
                });
            });
        }

        /// <inheritdoc />
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public string Question { get; internal set; }

        /// <inheritdoc/>
        IEnumerable<IPromptOption> IPrompt.Options => Options;

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public List<PromptOption> Options { get; }

        /// <inheritdoc/>
        public Guid Id { get; protected set; }

        /// <inheritdoc/>
        public Uri Url => new Uri($"submission/{Id}", UriKind.Relative);

        /// <inheritdoc/>
        public string Name => Question;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is Prompt prompt)
            {
                return this.Id.Equals(prompt.Id);
            }

            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Id.GetHashCode();
    }
}
