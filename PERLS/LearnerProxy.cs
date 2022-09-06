using System;
using System.ComponentModel;
using PERLS.Data.Definition;
using PERLS.DataImplementation.Providers;

namespace PERLS
{
    /// <summary>
    /// Provides a proxy to the rest of the app of the learner so the app can be notified of changes to the learner.
    /// </summary>
    public class LearnerProxy : ILearner, INotifyPropertyChanged
    {
        const string FileName = "cached_learner";
        const string CurrentLearnerKey = "current_learner";
        readonly DiskProviderCache learnerCache;
        ILearner source;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerProxy"/> class.
        /// </summary>
        /// <param name="original">The source learner object.</param>
        public LearnerProxy(ILearner original = null)
        {
            learnerCache = new DiskProviderCache(FileName);

            if (original != null)
            {
                this.source = original;
            }
            else if (original == null)
            {
                if (learnerCache.ContainsKey(CurrentLearnerKey))
                {
                    this.source = learnerCache.Get<ILearner>(CurrentLearnerKey);
                }
                else
                {
                    this.source = default;
                }
            }
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the source learner.
        /// </summary>
        /// <value>The source learner.</value>
        public ILearner Source
        {
            get => source;
            set
            {
                if (source != value)
                {
                    source = value;
                    HandleSourceChanged();
                }
            }
        }

        /// <inheritdoc />
        public Guid Id => IsSourceAvailable ? Source.Id : default;

        /// <inheritdoc />
        public string Email => IsSourceAvailable ? Source.Email : string.Empty;

        /// <inheritdoc />
        public IFile Avatar => IsSourceAvailable ? Source.Avatar : null;

        /// <inheritdoc />
        public string Name => IsSourceAvailable ? Source.Name : "Anonymous";

        /// <inheritdoc />
        public Uri Url => IsSourceAvailable ? Source.Url : null;

        /// <inheritdoc />
        public string EditPath => Source.EditPath;

        /// <inheritdoc/>
        public string PreferredLanguage => IsSourceAvailable ? Source.PreferredLanguage : default;

        /// <inheritdoc/>
        public ILearnerGoals LearnerGoals => Source.LearnerGoals;

        private bool IsSourceAvailable => Source != null;

        void HandleSourceChanged()
        {
            NotifyPropertyChanged(nameof(Name));
            NotifyPropertyChanged(nameof(Email));
            NotifyPropertyChanged(nameof(Avatar));
            learnerCache.Put(CurrentLearnerKey, this.source);
        }

        void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
