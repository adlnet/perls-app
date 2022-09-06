using System;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// The learner state for a single term.
    /// </summary>
    [Serializable]
    public class TermState : BaseState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermState"/> class.
        /// </summary>
        public TermState()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermState"/> class.
        /// </summary>
        /// <param name="term">A term whose state should be represented.</param>
        /// <param name="type">The type of state to apply.</param>
        public TermState(ITaxonomyTerm term, string type)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            Type = type;
            Tid = term.Tid;
            Id = term.Id;
        }

        /// <summary>
        /// Gets the term ID.
        /// </summary>
        /// <value>The term ID.</value>
        public int Tid { get; internal set; }
    }
}
