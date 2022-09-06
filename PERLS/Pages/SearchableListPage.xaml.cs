using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Float.Core.Commands;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Pages
{
    /// <summary>
    /// A page containing a list of account items.
    /// </summary>
    public partial class SearchableListPage : BasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableListPage"/> class.
        /// </summary>
        public SearchableListPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableListPage"/> class.
        /// </summary>
        /// <param name="viewModel">View model.</param>
        public SearchableListPage(SearchableContentListPageViewModel viewModel)
        {
            BindingContext = viewModel;
            LoadMoreCommand = new DebounceCommand(LoadMore);
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the title should be shown
        /// in large text with the user avatar.
        /// </summary>
        /// <value>
        /// A value indicating whether the custom nav bar should be used.
        /// </value>
        /// <remarks>
        /// The SearchableListPage is used for the bookmarks and history screen which are
        /// instantiated by the CorpusShell. Since these screens should show the custom
        /// navigation bar, this value defaults to true.
        /// In most other cases (i.e. when showing a taxonomy term), it should be <c>false</c>.
        /// </remarks>
        public bool ShowsLargeTitleAndAvatar { get; set; } = true;

        /// <summary>
        /// Gets or sets a command to load more pages.
        /// </summary>
        /// <value>A command to load more pages.</value>
        public ICommand LoadMoreCommand { get; set; }

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => ShowsLargeTitleAndAvatar;

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Collection.SelectedItem = null;

            if (BindingContext is SearchableContentListPageViewModel accountListPageViewModel)
            {
                accountListPageViewModel.Refresh();
            }
        }

        async void LoadMore()
        {
            var viewModel = BindingContext as SearchableContentListPageViewModel;

            if (viewModel?.ContentList == null || !viewModel.ContentList.Any())
            {
                return;
            }

            if (!viewModel.IsLoadingNewPage && !viewModel.IsEndOfContent)
            {
                var position = viewModel.ContentList.Count();
                viewModel.IsLoadingNewPage = true;
                await viewModel.LoadMoreAsyncCommand.ExecuteAsync(Collection.ItemsSource);

                OnPropertyChanged(nameof(Collection.ItemsSource));

                // Android jumps to the top, needs to be scrolled to the previous position
                // iOS jumps to the bottom, needs to be scrolled back up
                // This scroll to function, while making the loading a bit jerky, is miles better
                // than having the user discombobulated and thrown all around the screen, as this
                // centers back on the point of loading.
                Collection.ScrollTo(position + 1, -1, ScrollToPosition.End, false);
            }
        }
    }
}
