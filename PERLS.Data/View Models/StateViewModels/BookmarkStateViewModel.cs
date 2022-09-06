using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Commands;
using Float.Core.Extensions;
using Float.Core.Notifications;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The state of a bookmark.
    /// </summary>
    public class BookmarkStateViewModel : ItemStateViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkStateViewModel"/> class.
        /// </summary>
        /// <param name="item">The corpus item.</param>
        public BookmarkStateViewModel(IItem item) : base(item)
        {
            // this is arbitrarily low for now
            ToggleBookmarkCommand = new DebounceCommand(ToggleBookmark, 10);
        }

        /// <summary>
        /// Gets a command to toggle the bookmark state of this corpus item.
        /// </summary>
        /// <value>The toggle command.</value>
        public ICommand ToggleBookmarkCommand { get; }

        /// <summary>
        /// Gets a value indicating whether the current corpus item is bookmarkable.
        /// </summary>
        /// <value><c>true</c> if bookmarked, <c>false</c> otherwise.</value>
        public virtual bool IsBookmarkable => !(Item is IQuiz || Item is ITest);

        /// <summary>
        /// Gets a value indicating whether the current corpus item is bookmarked.
        /// </summary>
        /// <value><c>true</c> if bookmarked, <c>false</c> otherwise.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.Bookmarked))]
        public bool IsBookmarked => State?.Bookmarked == CorpusItemLearnerState.Status.Enabled;

        /// <summary>
        /// Gets an image to use as the bookmark icon.
        /// The image updates to reflect the current bookmark state.
        /// </summary>
        /// <value>The bookmark icon.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.Bookmarked))]
        public ImageSource BookmarkIcon
        {
            get
            {
                if (!IsBookmarkable || State == null)
                {
                    return null;
                }

                if (State.Bookmarked == CorpusItemLearnerState.Status.Enabled)
                {
                    return ImageSource.FromFile("bookmark_filled");
                }

                return ImageSource.FromFile("bookmark_unfilled");
            }
        }

        /// <summary>
        /// Gets an image to use as the bookmark icon for learning object.
        /// The image updates to reflect the current bookmark state.
        /// </summary>
        /// <value>The toolbar bookmark icon.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.Bookmarked))]
        public ImageSource ToolbarBookmarkIcon
        {
            get
            {
                if (!IsBookmarkable || State == null)
                {
                    return null;
                }

                if (State.Bookmarked == CorpusItemLearnerState.Status.Enabled)
                {
                    return ImageSource.FromFile("bookmark_toolbar_filled");
                }

                return ImageSource.FromFile("bookmark_toolbar_unfilled");
            }
        }

        /// <summary>
        /// Gets an image to use as the bookmark icon for learning object.
        /// The image updates to reflect the current bookmark state.
        /// </summary>
        /// <value>The colorized bookmark icon.</value>
        [NotifyWhenPropertyChanges(nameof(CorpusItemLearnerState.Bookmarked))]
        public ImageSource DarkendBookmarkIcon
        {
            get
            {
                if (!IsBookmarkable || State == null)
                {
                    return null;
                }

                if (State.Bookmarked == CorpusItemLearnerState.Status.Enabled)
                {
                    return ImageSource.FromFile("bookmark_toolbar_filled");
                }

                return ImageSource.FromFile("bookmark_toolbar_unfilled");
            }
        }

        /// <summary>
        /// Toggles the bookmark status of the corpus item.
        /// </summary>
        protected virtual void ToggleBookmark()
        {
            HandleToggleBookmark()
                .OnFailure((task) =>
                {
                    DependencyService.Get<INotificationHandler>().NotifyException(task.Exception, StringsSpecific.BookmarkErrorMessage);
                });
        }

        Task HandleToggleBookmark()
        {
            if (DependencyService.Get<ILearnerStateProvider>() is not ILearnerStateProvider provider)
            {
                return Task.CompletedTask;
            }

            return provider.ToggleBookmark(Item);
        }
    }
}
