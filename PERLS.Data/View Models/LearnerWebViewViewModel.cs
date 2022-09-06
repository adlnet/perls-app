using System;
using System.ComponentModel;
using System.Windows.Input;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A web view view model that can be used to observe changes to a Learner.
    /// </summary>
    public class LearnerWebViewViewModel : WebViewViewModel
    {
        ILearner learner;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerWebViewViewModel"/> class.
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="linkClicked">The link clicked command.</param>
        /// <param name="pageFailedToLoad">The page failed to load command.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="networkConnectionService">The network connection service.</param>
        /// <param name="title">The title.</param>
        public LearnerWebViewViewModel(ILearner learner, ICommand linkClicked, ICommand pageFailedToLoad, string destinationPath, INetworkConnectionService networkConnectionService = null, string title = null) : base(linkClicked, pageFailedToLoad, destinationPath, networkConnectionService, title)
        {
            if (learner == null)
            {
                throw new ArgumentNullException(nameof(learner));
            }

            this.learner = learner;
        }

        /// <inheritdoc/>
        protected override void OnObservingBegan()
        {
            base.OnObservingBegan();

            if (learner.LearnerGoals is INotifyPropertyChanged notifier)
            {
                notifier.PropertyChanged += OnModelPropertyChanged;
            }
        }

        /// <inheritdoc/>
        protected override void OnObservingEnded()
        {
            base.OnObservingEnded();

            if (learner.LearnerGoals is INotifyPropertyChanged notifier)
            {
                notifier.PropertyChanged -= OnModelPropertyChanged;
            }
        }

        /// <inheritdoc/>
        protected override void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnModelPropertyChanged(sender, e);
            Refresh();
        }
    }
}
