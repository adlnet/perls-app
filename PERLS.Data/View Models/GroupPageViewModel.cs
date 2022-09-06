using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Extensions;
using Float.Core.Notifications;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The individual group page view model.
    /// </summary>
    public class GroupPageViewModel : BasePageViewModel, IVariableItemViewModel, IActionableProvider, IActionableViewModel<IGroup>
    {
        readonly IAsyncCommand<IItem> downloadContentCommand;
        readonly ICommand closeGroupCommand;
        readonly IGroup group;
        readonly ILearnerStateProvider learnerStateProvider = DependencyService.Get<ILearnerStateProvider>();
        readonly ICorpusProvider corpusProvider = DependencyService.Get<ICorpusProvider>();
        IEnumerable<TeaserGroupViewModel> groupTopics = Enumerable.Empty<TeaserGroupViewModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupPageViewModel"/> class.
        /// </summary>
        /// <param name="group">The group to represent.</param>
        /// <param name="navigateCommand">The command to invoke when an item is selected.</param>
        /// <param name="closeGroupCommand">The command to invoke to close the detail view.</param>
        /// <param name="downloadContentCommand">A command to download content.</param>
        public GroupPageViewModel(IGroup group, ICommand navigateCommand, ICommand closeGroupCommand, IAsyncCommand<IItem> downloadContentCommand)
        {
            this.group = group ?? throw new ArgumentNullException(nameof(group));
            this.downloadContentCommand = downloadContentCommand ?? throw new ArgumentNullException(nameof(downloadContentCommand));
            this.closeGroupCommand = closeGroupCommand ?? throw new ArgumentNullException(nameof(closeGroupCommand));
            NavigateCommand = navigateCommand ?? throw new ArgumentNullException(nameof(navigateCommand));
            ActionCommand = CreateCommand(LeaveGroup);
        }

        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string Name => group.Name;

        /// <summary>
        /// Gets the description of the group.
        /// </summary>
        /// <value>The description of the group.</value>
        public string Description => group.Description;

        /// <summary>
        /// Gets an image representing this group.
        /// </summary>
        /// <value>The group image.</value>
        public ImageSource Image => group.Image?.Url is Uri uri ? ImageSource.FromUri(uri) : ImageSource.FromFile("placeholder");

        /// <summary>
        /// Gets a command to invoke when an item is selected.
        /// </summary>
        /// <value>The navigate command.</value>
        public ICommand NavigateCommand { get; }

        /// <inheritdoc />
        public IEnumerable Elements => groupTopics;

        /// <summary>
        /// Gets the sizing strategy for the collection view.
        /// </summary>
        /// <value>The element sizing strategy.</value>
        public ItemSizingStrategy ElementSizingStrategy => ItemSizingStrategy.MeasureAllItems;

        /// <summary>
        /// Gets a value indicating whether there are any items in the page.
        /// Used to resolve an issue where collection views do not update when the number of elements change.
        /// </summary>
        /// <value>
        /// <c>true</c> if there are no items, <c>false</c> otherwise.
        /// </value>
        public bool IsEmpty => !Elements.Any();

        /// <summary>
        /// Gets a value indicating whether there was an error loading the badges.
        /// </summary>
        /// <value>
        /// <c>true</c> if there was an error, <c>false</c> otherwise.
        /// </value>
        public bool IsError => Error != null;

        /// <inheritdoc />
        public string EmptyLabel => IsError ? DependencyService.Get<INotificationHandler>()?.FormatException(Error) : StringsSpecific.GroupTopicsEmptyMessage;

        /// <inheritdoc />
        public string EmptyMessageTitle => IsError ? Strings.EmptyViewErrorTitle : Strings.GroupTopicsEmptyTitle;

        /// <inheritdoc />
        public string EmptyImageName => IsError ? "error" : "resource://PERLS.Data.Resources.empty_placeholder.svg?Assembly=PERLS.Data";

        /// <inheritdoc />
        public IActionableViewModel Action => group.Visibility == IGroupVisibility.Public ? this : null;

        /// <inheritdoc />
        public string ActionLabel => Strings.LeaveGroupButtonLabel;

        /// <inheritdoc />
        public ICommand ActionCommand { get; }

        /// <inheritdoc />
        public IGroup ActionParameter => group;

        /// <inheritdoc />
        public override void Refresh()
        {
            base.Refresh();

            if (IsLoading)
            {
                return;
            }

            // Only show the loading indicator if we don't already have content to show.
            IsLoading = !groupTopics.Any();

            GetTopics().ContinueWith(
                task =>
                {
                    IsLoading = false;
                    ContainsCachedData = task.Exception != null && task.Exception.IsOfflineException();
                    Error = task.Exception;
                    OnPropertyChanged(nameof(Elements));
                    OnPropertyChanged(nameof(IsEmpty));
                }, TaskScheduler.Current);
        }

        async Task GetTopics()
        {
            var topics = await corpusProvider.GetGroupTopics(group).ConfigureAwait(false);
            groupTopics = topics.Select(itemGroup => new TeaserGroupViewModel(itemGroup, downloadContentCommand)
            {
                ShowsViewMore = true,
                EmptyMessageTitle = Strings.EmptyGroupTopicTitle,
                EmptyLabel = Strings.EmptyGroupTopicMessage,
                EmptyImageName = "resource://PERLS.Data.Resources.empty_placeholder.svg?Assembly=PERLS.Data",
            })?.ToList();
        }

        async Task LeaveGroup()
        {
            await learnerStateProvider.LeaveGroup(group).ConfigureAwait(false);
            closeGroupCommand.Execute(null);
        }
    }
}
