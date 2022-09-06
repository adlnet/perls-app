using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PERLS.Data.Definition;

namespace PERLS.Data
{
    /// <summary>
    /// Represents a single learner's state on a corpus item.
    /// </summary>
    [Serializable]
    public class CorpusItemLearnerState : INotifyPropertyChanged
    {
        Status bookmarked;
        Status completed;
        string recommendationReason;
        string feedback;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorpusItemLearnerState"/> class.
        /// </summary>
        /// <param name="item">The corpus item.</param>
        /// <param name="bookmarked">The initial bookmarked status.</param>
        /// <param name="completed">The initial completion status.</param>
        internal CorpusItemLearnerState(IItem item, Status bookmarked = Status.Unknown, Status completed = Status.Unknown)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            ItemId = item.Id;
            this.bookmarked = bookmarked;
            this.completed = completed;
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
        /// Gets the ID of the corpus item to which this state relates.
        /// </summary>
        /// <value>The corpus item ID.</value>
        public Guid ItemId { get; }

        /// <summary>
        /// Gets the current bookmarked status.
        /// </summary>
        /// <value>The bookmarked status.</value>
        public Status Bookmarked
        {
            get => bookmarked;
            internal set
            {
                if (value == bookmarked)
                {
                    return;
                }

                bookmarked = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current completion status.
        /// </summary>
        /// <value>The completion status.</value>
        public Status Completed
        {
            get => completed;
            internal set
            {
                if (value == completed)
                {
                    return;
                }

                completed = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the recommendation resaon.
        /// </summary>
        /// <value>The recommendation resaon.</value>
        public string RecommendationReason
        {
            get => recommendationReason;
            internal set
            {
                if (value == recommendationReason)
                {
                    return;
                }

                recommendationReason = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets feedback for this item.
        /// </summary>
        /// <value>The feedback.</value>
        /// <remarks>This is currently only used for tests.</remarks>
        public string Feedback
        {
            get => feedback;
            internal set
            {
                if (value == feedback)
                {
                    return;
                }

                feedback = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the state is fully known.
        /// </summary>
        internal bool IsUnknown => Bookmarked == Status.Unknown || Completed == Status.Unknown;

        /// <summary>
        /// Sets the Completed status to Unknown.
        /// </summary>
        public void SetCompletionStatusUnknown()
        {
            if (Completed != Status.Enabled)
            {
                Completed = Status.Unknown;
            }
        }

        /// <summary>
        /// Sets the <see cref="Completed"/> status to <see cref="Status.Enabled"/>.
        /// </summary>
        public void MarkAsComplete()
        {
            Completed = Status.Enabled;
        }

        internal void MarkAsRetrievingUpdatedState()
        {
            if (Bookmarked == Status.Unknown)
            {
                Bookmarked = Status.Retrieving;
            }

            if (Completed == Status.Unknown)
            {
                Completed = Status.Retrieving;
            }
        }

        internal void Reset()
        {
            Bookmarked = Status.Unknown;
            Completed = Status.Unknown;
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
