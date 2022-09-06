using System.Windows.Input;
using Float.Core.ViewModels;
using PERLS.Data.Commands;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// Dashboard view model.
    /// </summary>
    public sealed class DashboardViewModel : BasePageViewModel, IPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
        /// </summary>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        internal DashboardViewModel(ICommand selectItemCommand, IAsyncCommand<IItem> downloadContentCommand)
        {
            Recommendations = new DashboardCardDeckViewModel(Strings.TabRecommendationsLabel, selectItemCommand, downloadContentCommand, DashboardCardDeckViewModel.View.Recommendations, Strings.RecommendationsEmptyMessage, Strings.RecommendationsEmptyTitle, "resource://PERLS.Data.Resources.empty_placeholder.svg?Assembly=PERLS.Data");
            New = new DashboardCardDeckViewModel(Strings.TabNewLabel, selectItemCommand, downloadContentCommand, DashboardCardDeckViewModel.View.Recent, Strings.NewEmptyMessage);
            Trending = new DashboardCardDeckViewModel(Strings.TabTrendingLabel, selectItemCommand, downloadContentCommand, DashboardCardDeckViewModel.View.Trending, Strings.TrendingEmptyMessage);
            Following = new FollowingViewModel(selectItemCommand, downloadContentCommand);
        }

        /// <summary>
        /// Gets the page view model for the user's recommendations.
        /// </summary>
        /// <value>The view model for recommendations.</value>
        public IPageViewModel Recommendations { get; }

        /// <summary>
        /// Gets the page view model for the new tab.
        /// </summary>
        /// <value>The new tab view model.</value>
        public IPageViewModel New { get; }

        /// <summary>
        /// Gets the page view model for the trending tab.
        /// </summary>
        /// <value>The trending view model.</value>
        public IPageViewModel Trending { get; }

        /// <summary>
        /// Gets the page view model for the following tab.
        /// </summary>
        /// <value>The following view model.</value>
        public IPageViewModel Following { get; }

        /// <summary>
        /// Refresh this instance.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();

            if (Recommendations is DashboardCardDeckViewModel cardDeckViewModel)
            {
                cardDeckViewModel.Refresh();
            }

            if (New is DashboardCardDeckViewModel newCardDeckViewModel)
            {
                newCardDeckViewModel.Refresh();
            }

            if (Trending is DashboardCardDeckViewModel trendingCardDeckViewModel)
            {
                trendingCardDeckViewModel.Refresh();
            }
        }
    }
}
