using System.Windows.Input;
using Float.Core.ViewModels;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.Extensions;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Account view model.
    /// </summary>
    public sealed class AccountViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountViewModel"/> class.
        /// </summary>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="setGoalReminderCommand">The set goal reminder command.</param>
        /// <param name="adjustGoalCommand">The adjust goal command.</param>
        /// <param name="gotoAnnotationCommand">The goto annotation command.</param>
        /// <param name="viewGoalDetailsCommand">The view goal details command.</param>
        internal AccountViewModel(ICommand selectItemCommand, IAsyncCommand<IItem> downloadContentCommand, ICommand setGoalReminderCommand, ICommand adjustGoalCommand, ICommand gotoAnnotationCommand, ICommand viewGoalDetailsCommand)
        {
            var corpusProvider = DependencyService.Get<ILearnerStateProvider>();
            var learnerProvider = DependencyService.Get<ILearnerProvider>();

            Bookmarks = new SearchableContentListPageViewModel(Strings.TabMyContentLabel, corpusProvider.GetBookmarks, selectItemCommand, downloadContentCommand, StringsSpecific.BookmarksEmptyMessage, "emptyBookmark");
            History = new SearchableContentListPageViewModel(Strings.TabMyContentLabel, corpusProvider.GetHistory, selectItemCommand, downloadContentCommand, Strings.HistoryEmptyMessage.AddAppName(), "emptyHistory");
            var context = DependencyService.Get<IAppContextService>();
            Goals = new LearnerStatsViewModel(learnerProvider, setGoalReminderCommand, adjustGoalCommand, viewGoalDetailsCommand);
            Notes = new LearnerNotesViewModel(corpusProvider, gotoAnnotationCommand);
            Certificates = new CertificatesViewModel(selectItemCommand, learnerProvider);
            Badges = new BadgesCollectionViewModel(learnerProvider.GetBadges, selectItemCommand);
        }

        /// <summary>
        /// Gets the view model for the bookmarks tab.
        /// </summary>
        /// <value>The bookmarks view model.</value>
        public IPageViewModel Bookmarks { get; }

        /// <summary>
        /// Gets the view model for the history tab.
        /// </summary>
        /// <value>The view model.</value>
        public IPageViewModel History { get; }

        /// <summary>
        /// Gets the view model for the certificates tab.
        /// </summary>
        /// <value>
        /// The view model for the certificates tab.
        /// </value>
        public CertificatesViewModel Certificates { get; }

        /// <summary>
        /// Gets the view model for the badges tab.
        /// </summary>
        /// <value>
        /// The view model for the badges tab.
        /// </value>
        public IPageViewModel Badges { get; }

        /// <summary>
        /// Gets the view model for the goals tab.
        /// </summary>
        /// <value>The view model.</value>
        public IPageViewModel Goals { get; }

        /// <summary>
        /// Gets the view model for the Notes tab.
        /// </summary>
        /// <value>
        /// The view model for the Notes tab.
        /// </value>
        public IPageViewModel Notes { get; }
    }
}
