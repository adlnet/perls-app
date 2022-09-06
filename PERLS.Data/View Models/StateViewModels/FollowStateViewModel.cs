using System;
using System.Windows.Input;
using Float.Core.Commands;
using Float.Core.Extensions;
using Float.Core.Notifications;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels.StateViewModels
{
    /// <summary>
    /// The state of a follow.
    /// </summary>
    public class FollowStateViewModel : TermStateViewModel, IActionableViewModel
    {
        readonly INotificationHandler notificationHandler = DependencyService.Get<INotificationHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FollowStateViewModel"/> class.
        /// </summary>
        /// <param name="term">The term.</param>
        public FollowStateViewModel(ITaxonomyTerm term) : base(term)
        {
            ActionCommand = new DebounceCommand(ToggleFollow, 10);
        }

        /// <summary>
        /// Gets a value indicating whether the current taxonomy term is followable.
        /// </summary>
        /// <value><c>true</c> if followable, <c>false</c> otherwise.</value>
        public virtual bool IsFollowable => Term is ITag && Constants.TagFollowing;

        /// <summary>
        /// Gets a value indicating whether the current taxonomy term is followed.
        /// </summary>
        /// <value><c>true</c> if bookmarked, <c>false</c> otherwise.</value>
        [NotifyWhenPropertyChanges(nameof(TaxonomyTermLearnerState.Following))]
        public bool IsFollowing => State?.Following == TaxonomyTermLearnerState.Status.Enabled;

        /// <inheritdoc />
        [NotifyWhenPropertyChanges(nameof(TaxonomyTermLearnerState.Following))]
        public string ActionLabel => IsFollowing ? Strings.UnfollowButtonLabel : Strings.FollowButtonLabel;

        /// <inheritdoc />
        public ICommand ActionCommand { get; }

        /// <summary>
        /// Gets parameter to prevent a binding error. The base page is expecting an ActionParameter
        /// from all "ActionableViewModels", even if they have no type.
        /// </summary>
        /// <value>Null.</value>
        public object ActionParameter => null;

        void ToggleFollow()
        {
            Provider.ToggleFollowing(Term).OnFailure(task =>
            {
                if (task?.Exception is Exception e)
                {
                    notificationHandler.NotifyException(e, Strings.FollowErrorMessage);
                }
            });
        }
    }
}
