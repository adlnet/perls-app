using System.ComponentModel;
using System.Windows.Input;
using Float.Core.ViewModels;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Top-level view model representing the corpus.
    /// </summary>
    public class CorpusShellViewModel : BaseViewModel
    {
        readonly IFeatureFlagService featureFlagService = DependencyService.Get<IFeatureFlagService>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CorpusShellViewModel"/> class.
        /// </summary>
        /// <param name="selectItemCommand">The command to invoke when a content item is selected.</param>
        /// <param name="makeSuggestionCommand">The command to invoke when the user wants to make a suggestion.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="adjustGoalCommand">The adjust goal command.</param>
        /// <param name="setGoalReminderCommand">The set goal reminder command.</param>
        /// <param name="gotoAnnotationCommand">Go to annotation command.</param>
        /// <param name="viewGoalDetailsCommand">The view goal details command.</param>
        public CorpusShellViewModel(ICommand selectItemCommand, ICommand makeSuggestionCommand, IAsyncCommand<IItem> downloadContentCommand, ICommand setGoalReminderCommand, ICommand adjustGoalCommand, ICommand gotoAnnotationCommand, ICommand viewGoalDetailsCommand)
        {
            if (featureFlagService.IsFlagEnabled(FeatureFlagName.EnhancedDashboard))
            {
                Dashboard = new EnhancedDashboardViewModel(selectItemCommand);
            }
            else
            {
                Dashboard = new DashboardViewModel(selectItemCommand, downloadContentCommand);
            }

            Podcasts = new PodcastsViewModel(selectItemCommand, downloadContentCommand);
            Search = new SearchViewModel(selectItemCommand, makeSuggestionCommand, downloadContentCommand);
            Account = new AccountViewModel(selectItemCommand, downloadContentCommand, setGoalReminderCommand, adjustGoalCommand, gotoAnnotationCommand, viewGoalDetailsCommand);
            Learner = DependencyService.Get<IAppContextService>().CurrentLearner;
            Groups = new GroupsViewModel(selectItemCommand);

            if (Learner is INotifyPropertyChanged learner)
            {
                learner.PropertyChanged += OnModelPropertyChanged;
            }
        }

        /// <summary>
        /// Gets the current learner.
        /// </summary>
        /// <value>The current learner.</value>
        public ILearner Learner { get; }

        /// <summary>
        /// Gets the view model for the dashboard.
        /// </summary>
        /// <value>The dashboard view model.</value>
        public BasePageViewModel Dashboard { get; }

        /// <summary>
        /// Gets the view model for the podcasts page.
        /// </summary>
        /// <value>The podcasts view model.</value>
        public PodcastsViewModel Podcasts { get; }

        /// <summary>
        /// Gets the view model for the search page.
        /// </summary>
        /// <value>The search view model.</value>
        public SearchViewModel Search { get; }

        /// <summary>
        /// Gets the view model for the groups page.
        /// </summary>
        /// <value>The groups view model.</value>
        public GroupsViewModel Groups { get; }

        /// <summary>
        /// Gets the view model associated with the account page.
        /// </summary>
        /// <value>The account view model.</value>
        public AccountViewModel Account { get; }

        /// <summary>
        /// Refresh this instance.
        /// </summary>
        public void Refresh()
        {
            if (Dashboard is BasePageViewModel refreshableDashboard)
            {
                refreshableDashboard.Refresh();
            }

            if (Account.Bookmarks is BasePageViewModel refreshableBookmarks)
            {
                refreshableBookmarks.Refresh();
            }

            if (Account.History is BasePageViewModel refreshableHistory)
            {
                refreshableHistory.Refresh();
            }
        }
    }
}
