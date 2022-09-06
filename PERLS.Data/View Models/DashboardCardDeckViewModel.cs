using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Analytics;
using Float.Core.Extensions;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A deck of cards for the dashboard.
    /// </summary>
    public class DashboardCardDeckViewModel : CardDeckPageViewModel
    {
        readonly View view;
        PromptCollectionViewModel header;
        IEnumerable<IPrompt> headerPrompts;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardCardDeckViewModel"/> class.
        /// </summary>
        /// <param name="title">The deck title.</param>
        /// <param name="selectCardCommand">The command to invoke when a card is selected.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="view">The data to load.</param>
        /// <param name="emptyMessage">Message when content empty.</param>
        /// <param name="emptyTitle">The title to present when there is no content.</param>
        /// <param name="emptyImage">The image to present when there is no content.</param>
        public DashboardCardDeckViewModel(string title, ICommand selectCardCommand, IAsyncCommand<IItem> downloadContentCommand, View view = View.Recommendations, string emptyMessage = "", string emptyTitle = "", string emptyImage = "") : base(title, GetCards(view), selectCardCommand, downloadContentCommand, emptyMessage, emptyTitle, emptyImage)
        {
            this.view = view;
        }

        /// <summary>
        /// Represents the type of content being displayed.
        /// </summary>
        public enum View
        {
            /// <summary>
            /// The user's personalized recommendations.
            /// </summary>
            Recommendations,

            /// <summary>
            /// Trending content that is relevant to the user.
            /// </summary>
            Trending,

            /// <summary>
            /// Recently created content that is relevant to the user.
            /// </summary>
            Recent,
        }

        /// <summary>
        /// Gets or sets the Header for the collection.
        /// </summary>
        /// <value>
        /// The Header for the collection.
        /// </value>
        public PromptCollectionViewModel Header
        {
            get => header;
            set => SetField(ref header, value);
        }

        /// <inheritdoc/>
        public override void Refresh()
        {
            base.Refresh();

            if (view is View.Recommendations)
            {
                UpdateHeader().OnFailure(task =>
                {
                    DependencyService.Get<AnalyticsService>().TrackException(task.Exception);
                });
            }
        }

        /// <summary>
        /// Gets the cards for the dashboard.
        /// </summary>
        /// <param name="view">The data to load.</param>
        /// <returns>A task retrieving the card data.</returns>
        static Func<Task<IEnumerable<IItem>>> GetCards(View view)
        {
            var provider = DependencyService.Get<ICorpusProvider>();

            return view switch
            {
                View.Recommendations => provider.GetRecommendations,
                View.Trending => provider.GetTrendingContent,
                View.Recent => provider.GetRecentContent,
                _ => provider.GetRecentContent,
            };
        }

        async Task<PromptCollectionViewModel> UpdateHeader()
        {
            try
            {
                var provider = DependencyService.Get<ILearnerProvider>();
                var prompts = await provider.GetPrompts().ConfigureAwait(false);
                if (prompts.Any())
                {
                    if (headerPrompts == null || !headerPrompts.SequenceEqual(prompts))
                    {
                        headerPrompts = prompts;
                        Header = new PromptCollectionViewModel(prompts);
                        return Header;
                    }

                    return Header;
                }
                else
                {
                    headerPrompts = null;
                    Header = null;
                    return Header;
                }
            }
            catch (Exception e) when (e.IsOfflineException())
            {
                return Header;
            }
        }
    }
}
