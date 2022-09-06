using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Notifications;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The following view model.
    /// </summary>
    public class FollowingViewModel : BasePageViewModel, IVariableItemViewModel
    {
        readonly IReportingService reportingService;
        readonly ICorpusProvider corpusProvider;
        readonly IAsyncCommand<IItem> downloadContentCommand;
        IEnumerable<IEnumerable> browseList;
        ISelectable selectedItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="FollowingViewModel"/> class.
        /// </summary>
        /// <param name="navigateCommand">The command to invoke when an item is selected.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        public FollowingViewModel(ICommand navigateCommand, IAsyncCommand<IItem> downloadContentCommand)
        {
            Title = Strings.TabFollowingLabel;
            this.reportingService = reportingService ?? DependencyService.Get<IReportingService>();
            browseList = new List<TeaserGroupViewModel>();
            NavigateCommand = navigateCommand;
            corpusProvider = DependencyService.Get<ICorpusProvider>();
            this.downloadContentCommand = downloadContentCommand ?? throw new ArgumentNullException(nameof(downloadContentCommand));

            GetContentCommand = CreateCommand<object>(UpdateRelevantContent);
            GetContentCommand.Execute(this);
        }

        /// <summary>
        /// Gets a command to invoke when a search result is selected.
        /// </summary>
        /// <value>The navigate command.</value>
        public ICommand NavigateCommand { get; }

        /// <summary>
        /// Gets a command to invoke when retrieving content.
        /// </summary>
        /// <value>The command to retrieve content.</value>
        public ICommand GetContentCommand { get; }

        /// <summary>
        /// Gets a value indicating whether the data source is empty or not.
        /// </summary>
        /// <value><c>true</c> if the data source is empty, <c>false</c> otherwise.</value>
        public bool IsEmpty => Elements is not IEnumerable<object> theElements || !theElements.Any();

        /// <summary>
        /// Gets the list of items to display as the search results.
        /// </summary>
        /// <value>The list of items to display.</value>
        public IEnumerable Elements => browseList;

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
            }
        }

        /// <inheritdoc/>
        public string EmptyMessageTitle
        {
            get
            {
                if (Error != null)
                {
                    return Strings.EmptyViewErrorTitle;
                }

                return Strings.EmptyFollowingTitle;
            }
        }

        /// <inheritdoc/>
        public string EmptyImageName
        {
            get
            {
                return "icon_awesome_hashtag";
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

                return Strings.EmptyFollowingMessage;
            }
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public void ClearSelection()
        {
            SelectedItem = null;
        }

        async Task UpdateRelevantContent(object arg = null)
        {
            IsLoading = true;
            var content = await corpusProvider.GetFollowedContent().ConfigureAwait(false);
            browseList = content?.Select((itemGroup) => new TeaserGroupViewModel(itemGroup, downloadContentCommand)
            {
                EmptyLabel = string.Format(CultureInfo.InvariantCulture, Strings.EmptyTagMessage, itemGroup.Name),
                EmptyImageName = EmptyImageName,
            })?.ToList();

            // We use IsEmpty to determine whether or not to show the EmptyView. Noramlly, this is done by Xamarin itself, but for some reason
            // when we update the content of Elements, the EmptyView sticks around. This causes the EmptyView to appear over the following page
            // even when content is available. By using IsEmpty, we can set the EmptyView to be visible or not.
            OnPropertyChanged(nameof(Elements));
            OnPropertyChanged(nameof(IsEmpty));
            IsLoading = false;
        }
    }
}
