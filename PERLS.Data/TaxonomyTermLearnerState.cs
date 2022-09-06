using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PERLS.Data.Definition;

namespace PERLS.Data
{
    /// <summary>
    /// Represents a single learner's state on a taxonomy term.
    /// </summary>
    [Serializable]
    public class TaxonomyTermLearnerState : INotifyPropertyChanged
    {
        Status following;

        internal TaxonomyTermLearnerState(ITaxonomyTerm term, Status following = Status.Unknown)
        {
            if (term == null)
            {
                throw new ArgumentNullException(nameof(term));
            }

            TermId = term.Tid;
            this.following = following;
        }

        /// <inheritdoc />
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Represents the state of a corpus item.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// State is enabled.
            /// </summary>
            Enabled,

            /// <summary>
            /// State is disabled.
            /// </summary>
            Disabled,

            /// <summary>
            /// State is being updated.
            /// </summary>
            Retrieving,

            /// <summary>
            /// State is unknown.
            /// </summary>
            Unknown,
        }

        /// <summary>
        /// Gets the term ID of the term to which this state relates.
        /// </summary>
        /// <value>The taxonomy term ID.</value>
        public int TermId { get; }

        /// <summary>
        /// Gets the current followed status.
        /// </summary>
        /// <value>The followed status.</value>
        public Status Following
        {
            get => following;
            internal set
            {
                if (value == following)
                {
                    return;
                }

                following = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the state is fully known.
        /// </summary>
        internal bool IsUnknown => Following == Status.Unknown;

        /// <summary>
        /// Sets the Following status to Unknown.
        /// </summary>
        public void SetFollowingStatusUnknown()
        {
            Following = Status.Unknown;
        }

        internal void MarkAsRetrievingUpdatedState()
        {
            if (Following == Status.Unknown)
            {
                Following = Status.Retrieving;
            }
        }

        internal void Reset()
        {
            Following = Status.Unknown;
        }

        /// <summary>
        /// Notifies observers that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
