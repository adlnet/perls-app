using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Commands;
using Float.Core.Notifications;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// The groups view model.
    /// </summary>
    public class GroupsViewModel : BasePageViewModel, IVariableItemViewModel
    {
        readonly ILearnerStateProvider learnerStateProvider;
        readonly ICorpusProvider corpusProvider;
        readonly GroupTeaserGroupViewModel joinedTeaser;
        readonly GroupTeaserGroupViewModel joinableTeaser;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsViewModel"/> class.
        /// </summary>
        /// <param name="navigateCommand">The navigation command.</param>
        public GroupsViewModel(ICommand navigateCommand)
        {
            Title = StringsSpecific.TabGroupsLabel;
            NavigateCommand = navigateCommand;
            learnerStateProvider = DependencyService.Get<ILearnerStateProvider>();
            corpusProvider = DependencyService.Get<ICorpusProvider>();
            JoinGroupCommand = new SelectViewModelCommand<IGroup>(HandleJoinGroup);

            SelectGroupCommand = new DebounceCommand(new Command<GroupTeaserViewModel>(HandleSelectGroup));

            joinedTeaser = new GroupTeaserGroupViewModel(corpusProvider.GetGroups, StringsSpecific.JoinedGroupsTitle);
            joinableTeaser = new GroupTeaserGroupViewModel(corpusProvider.GetJoinableGroups, StringsSpecific.JoinableGroupsTitle, StringsSpecific.MoreGroupsSubtitle, 500, true);
        }

        /// <summary>
        /// Gets a command to navigate to a joined group.
        /// </summary>
        /// <value>The navigate command.</value>
        public ICommand NavigateCommand { get; }

        /// <summary>
        /// Gets a command to invoke when joining a new group.
        /// </summary>
        /// <value>The command to join a new group.</value>
        public ICommand JoinGroupCommand { get; }

        /// <summary>
        /// Gets a command to invoke when any group is selected.
        /// </summary>
        /// <value>The group selected command.</value>
        public ICommand SelectGroupCommand { get; }

        /// <summary>
        /// Gets a value indicating whether the data source is empty or not.
        /// </summary>
        /// <value><c>true</c> if the data source is empty, <c>false</c> otherwise.</value>
        public bool IsEmpty => Elements is not IEnumerable<object> theElements || !theElements.Any();

        /// <summary>
        /// Gets the list of joined groups.
        /// </summary>
        /// <value>The list of joined groups to display.</value>
        public IEnumerable Elements
        {
            get
            {
                if (joinableTeaser.Any() && Error == null)
                {
                    return new List<GroupTeaserGroupViewModel> { joinedTeaser, joinableTeaser };
                }

                if (joinedTeaser.Any())
                {
                    return new List<GroupTeaserGroupViewModel> { joinedTeaser };
                }

                return new List<GroupTeaserGroupViewModel>();
            }
        }

        /// <summary>
        /// Gets the sizing strategy for the collection view.
        /// </summary>
        /// <value>The element sizing strategy.</value>
        public ItemSizingStrategy ElementSizingStrategy => ItemSizingStrategy.MeasureAllItems;

        /// <inheritdoc/>
        public string EmptyMessageTitle
        {
            get
            {
                if (Error != null)
                {
                    return Strings.EmptyViewErrorTitle;
                }

                return StringsSpecific.EmptyGroupsTitle;
            }
        }

        /// <inheritdoc/>
        public string EmptyImageName
        {
            get
            {
                return "resource://PERLS.Data.Resources.empty_placeholder.svg?Assembly=PERLS.Data";
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

                return Strings.EmptyGroupsMessage;
            }
        }

        /// <inheritdoc />
        public override void Refresh()
        {
            base.Refresh();

            IsLoading = true;
            Task.WhenAll(joinedTeaser.Refresh(), joinableTeaser.Refresh()).ContinueWith(
                (task) =>
            {
                IsLoading = false;
                Error = task.Exception;
                OnPropertyChanged(nameof(Elements));
                OnPropertyChanged(nameof(IsEmpty));
            }, TaskScheduler.Current);
        }

        void HandleSelectGroup(GroupTeaserViewModel group)
        {
            if (joinedTeaser.Contains(group))
            {
                NavigateCommand.Execute(group);
            }
            else
            {
                JoinGroupCommand.Execute(group);
            }
        }

        void HandleJoinGroup(IGroup group)
        {
            IsLoading = true;
            learnerStateProvider.JoinGroup(group).ContinueWith(
                (task) =>
            {
                IsLoading = false;
                Error = task.Exception;
                Refresh();
            }, TaskScheduler.Current);
        }
    }
}
