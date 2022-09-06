using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <inheritdoc />
    [Serializable]
    public class Test : Node, ITest
    {
        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        /// <value>
        /// The questions.
        /// </value>
        public IEnumerable<Quiz> Questions { get; protected set; }

        /// <inheritdoc/>
        [JsonProperty("pass_mark")]
        public double PercentRequiredToPass { get; protected set; }

        /// <inheritdoc/>
        IEnumerable<IQuiz> ITest.Questions => Questions;
    }
}
