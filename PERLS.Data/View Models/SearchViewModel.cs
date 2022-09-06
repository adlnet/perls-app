using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Commands;
using Float.Core.Notifications;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Cache;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The search view model.
    /// </summary>
    public class SearchViewModel : BasePageViewModel, IVariableItemViewModel
    {
        readonly ICommand searchCommand;
        readonly ICommand getContentCommand;
        readonly IAsyncCommand<IItem> downloadContentCommand;
        readonly ICorpusProvider corpusProvider;
        readonly IReportingService reportingService;
        ISelectable selectedItem;
        string query = string.Empty;
        IEnumerable<IEnumerable> browseList;
        IEnumerable<TeaserViewModel> searchList;
        Task currentSearchTask;
        bool showNoResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchViewModel"/> class.
        /// </summary>
        /// <param name="navigateCommand">The command to invoke when an item is selected.</param>
        /// <param name="suggestionCommand">The command to invoke when you select a command.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="reportingService">The reporting service to use for reporting activity.</param>
        public SearchViewModel(ICommand navigateCommand, ICommand suggestionCommand, IAsyncCommand<IItem> downloadContentCommand, IReportingService reportingService = null)
        {
            Title = Strings.TabSearchLabel;
            this.reportingService = reportingService ?? DependencyService.Get<IReportingService>();
            browseList = new List<TeaserGroupViewModel>();
            searchList = new List<TeaserViewModel>();
            NavigateCommand = new DebounceCommand(new MultiCommand(navigateCommand, new Command<TeaserViewModel>(OnItemSelected)));
            corpusProvider = DependencyService.Get<ICorpusProvider>();

            this.downloadContentCommand = downloadContentCommand ?? throw new ArgumentNullException(nameof(downloadContentCommand));

            getContentCommand = CreateCommand<object>(UpdateRelevantContent);
            getContentCommand.Execute(this);

            HandleMakeSuggestion = suggestionCommand;
            searchCommand = new ThrottleCommand(
                new MultiCommand(
                    RefreshCommand,
                    new ThrottleCommand(new Command<string>(this.reportingService.ReportSearchQuery), 3000)),
                800);
        }

        /// <summary>
        /// Gets a value indicating whether there is a query.
        /// </summary>
        /// <value><c>true</c> if there is a query, <c>false</c> otherwise.</value>
        public bool IsQuery => !string.IsNullOrWhiteSpace(Query);

        /// <summary>
        /// Gets a value indicating whether the feedback button should be shown.
        /// </summary>
        /// <value><c>true</c> if the feedback button should be shown.</value>
        public bool ShowsFeedbackButton => IsQuery && !ContainsCachedData && Constants.FeedbackAccess;

        /// <inheritdoc/>
        public string EmptyMessageTitle
        {
            get
            {
                if (IsLoading)
                {
                    return string.Empty;
                }

                if (Error != null)
                {
                    return Strings.EmptyViewErrorTitle;
                }

                return string.Empty;
            }
        }

        /// <inheritdoc/>
        public string EmptyImageName
        {
            get
            {
                if (DependencyService.Get<INetworkConnectionService>().IsReachable().GetAwaiter().GetResult())
                {
                    return "no_search_result";
                }
                else
                {
                    return "error";
                }
            }
        }

        /// <summary>
        /// Gets a command to invoke when a search result is selected.
        /// </summary>
        /// <value>The navigate command.</value>
        public ICommand NavigateCommand { get; }

        /// <summary>
        /// Gets a command to invoke when the user selects a suggestion.
        /// </summary>
        /// <value>The suggestion command.</value>
        public ICommand HandleMakeSuggestion { get; }

        /// <summary>
        /// Gets or sets the search query.
        /// </summary>
        /// <value>The search query.</value>
        public string Query
        {
            get => query;
            set
            {
                if (query == value)
                {
                    return;
                }

                if (value == null)
                {
                    query = string.Empty;
                }
                else
                {
                    query = value;
                }

                if (searchCommand.CanExecute(query))
                {
                    searchCommand.Execute(query);
                }

                OnPropertyChanged(nameof(IsQuery));
                OnPropertyChanged(nameof(EmptyLabel));
            }
        }

        /// <inheritdoc />
        public string EmptyLabel
        {
            get
            {
                if (Error != null)
                {
                    return DependencyService.Get<INotificationHandler>()?.FormatException(Error);
                }

                return !IsQuery ? Strings.SearchPrompt : string.Format(CultureInfo.InvariantCulture, Strings.SearchNoResultsMessage, Query);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data source is empty or not.
        /// </summary>
        /// <value><c>true</c> if the data source is empty, <c>false</c> otherwise.</value>
        public bool IsEmpty => Elements is not IEnumerable<object> theElements || !theElements.Any();

        /// <summary>
        /// Gets or sets a value indicating whether the no results view should be visible.
        /// </summary>
        /// <value><c>true</c> if it should not be shown, <c>false</c> otherwise.</value>
        public bool ShowNoResults
        {
            get => IsEmpty && showNoResults;

            set
            {
                showNoResults = value;
            }
        }

        /// <summary>
        /// Gets the list of items to display as the search results.
        /// </summary>
        /// <value>The list of items to display.</value>
        public IEnumerable Elements => IsQuery ? searchList : (IEnumerable)browseList;

        /// <summary>
        /// Gets the sizing strategy for the collection view.
        /// </summary>
        /// <value>The element sizing strategy.</value>
        public ItemSizingStrategy ElementSizingStrategy => ItemSizingStrategy.MeasureAllItems;

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        /// <value>The currently selected item.</value>
        public ISelectable SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem == value)
                {
                    return;
                }

                if (selectedItem != null)
                {
                    selectedItem.IsSelected = false;
                }

                SetField(ref selectedItem, value);

                if (selectedItem != null)
                {
                    selectedItem.IsSelected = true;
                }

                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        /// <inheritdoc />
        public override void Refresh()
        {
            base.Refresh();

            IsLoading = true;
            currentSearchTask = PerformSearch(Query);
            currentSearchTask.ContinueWith(
                task =>
                {
                    if (task == currentSearchTask)
                    {
                        currentSearchTask = null;
                    }

                    Error = task.Exception;
                    IsLoading = currentSearchTask != null;
                }, TaskScheduler.Current);
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public void ClearSelection()
        {
            SelectedItem = null;
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Query):
                    OnPropertyChanged(nameof(IsQuery));
                    break;
                case nameof(IsQuery):
                    OnPropertyChanged(nameof(IsEmpty));
                    OnPropertyChanged(nameof(Elements));
                    OnPropertyChanged(nameof(ElementSizingStrategy));
                    OnPropertyChanged(nameof(ShowsFeedbackButton));
                    if (!IsQuery)
                    {
                        OnPropertyChanged(nameof(EmptyLabel));
                        ShowNoResults = false;
                        OnPropertyChanged(nameof(ShowNoResults));
                    }

                    break;
                case nameof(ContainsCachedData):
                    OnPropertyChanged(nameof(ShowsFeedbackButton));
                    break;
                case nameof(Error):
                    OnPropertyChanged(nameof(Elements));
                    OnPropertyChanged(nameof(EmptyLabel));
                    break;
                case nameof(SelectedItem):
                    if ((SelectedItem as TeaserViewModel)?.ModelItem is IDocument)
                    {
                        SelectedItem = null;
                    }

                    break;
            }
        }

        async Task UpdateRelevantContent(object arg = null)
        {
            var content = await corpusProvider.GetRelevantContent().ConfigureAwait(false);
            browseList = content?.Select((itemGroup) => new TeaserGroupViewModel(itemGroup, downloadContentCommand))?.ToList();
            OnPropertyChanged(nameof(Elements));
        }

        /// <summary>
        /// Performs a search query.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <returns>The task for the query request.</returns>
        async Task PerformSearch(string searchQuery)
        {
            SelectedItem = null;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var results = await corpusProvider.SearchCorpus(searchQuery).ConfigureAwait(false);
                ContainsCachedData = results is CacheDerivedItems;
                searchList = results.Select((item) => new TeaserViewModel(item, downloadContentCommand));
            }
            else
            {
                searchList = null;
                ContainsCachedData = false;
                await UpdateRelevantContent().ConfigureAwait(false);
            }

            Error = null;
            OnPropertyChanged(nameof(Elements));
            OnPropertyChanged(nameof(EmptyLabel));
            OnPropertyChanged(nameof(IsEmpty));
            if (IsQuery)
            {
                ShowNoResults = true;
                OnPropertyChanged(nameof(ShowNoResults));
            }
        }

        void OnItemSelected(TeaserViewModel item)
        {
            if (!IsQuery || item == null)
            {
                return;
            }

            reportingService.ReportSearchResultSelected(Query, item.ModelItem);
        }
    }
}
