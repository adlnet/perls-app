using System;
using System.Linq;
using System.Windows.Input;
using Float.Core.Definitions;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using PERLS.Data.ViewModels.StateViewModels;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model containing the items associated with the specified term.
    /// </summary>
    public class TermContentListViewModel : SearchableContentListPageViewModel, IActionableProvider
    {
        readonly IReportingService reportingService = DependencyService.Get<IReportingService>();
        ITaxonomyTerm term;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermContentListViewModel"/> class.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="selectItemCommand">The command to invoke when a user selects an item.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="emptyLabel">The string containing the label should the collection returned be empty.</param>
        /// <param name="emptyImageName">The string the name of the image to display should the collection be empty.</param>
        public TermContentListViewModel(ITaxonomyTerm term, ICommand selectItemCommand, IAsyncCommand<IItem> downloadContentCommand, string emptyLabel, string emptyImageName)
            : base(Strings.DefaultLoadingMessage, () => DependencyService.Get<ICorpusProvider>().GetTermItems(term), selectItemCommand, downloadContentCommand, emptyLabel, emptyImageName)
        {
            this.term = term ?? throw new ArgumentNullException(nameof(term));
            UpdatePageTitle();
            FollowState = new FollowStateViewModel(term);
        }

        /// <summary>
        /// Gets the current follow state.
        /// </summary>
        /// <value>The follow state.</value>
        public FollowStateViewModel FollowState { get; }

        /// <inheritdoc />
        public IActionableViewModel Action => FollowState.IsFollowable ? FollowState : null;

        /// <summary>
        /// Gets or sets the remote resource represented by this view.
        /// </summary>
        /// <remarks>
        /// Currently this is always going to be a tag. But someday this class could be split
        /// to handle other remote resource collection types too (i.e. topics, groups, etc.).
        /// </remarks>
        /// <value>The remote resource.</value>
        protected ITaxonomyTerm Term
        {
            get => term;
            set
            {
                if (term?.Id != value?.Id)
                {
                    reportingService.ReportResourceViewed(value);
                }

                SetField(ref term, value);
                UpdatePageTitle();
            }
        }

        /// <inheritdoc />
        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();
            UpdateResource();
        }

        /// <summary>
        /// Based on the loaded content, updated the resource definition for this page.
        /// </summary>
        protected virtual void UpdateResource()
        {
            var localTerm = ContentList
                .SelectMany(item => item.ModelItem.Tags)
                .FirstOrDefault(t => t.Tid == term.Tid);

            if (localTerm != null)
            {
                Term = localTerm;
            }
            else
            {
                UpdatePageTitle();
            }
        }

        /// <summary>
        /// Formats the page title.
        /// </summary>
        /// <param name="namedCollection">The named element serving as the container for the content.</param>
        /// <returns>The formatted page title.</returns>
        /// <remarks>This adds the "#" in front of a tag.</remarks>
        protected virtual string FormatPageTitle(INamed namedCollection) => namedCollection switch
        {
            ITag tag => $"#{tag.Name}",
            INamed named => named.Name,
            _ => Error != null ? Strings.DefaultErrorTitle : string.Empty,
        };

        void UpdatePageTitle()
        {
            Title = FormatPageTitle(Term);
        }
    }
}
