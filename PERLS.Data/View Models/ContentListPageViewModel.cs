using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.ViewModels;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A page containing a list of content.
    /// </summary>
    public class ContentListPageViewModel : BasePageViewModel
    {
        ISelectable selectedItem;
        Task loadingDataTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentListPageViewModel"/> class.
        /// </summary>
        /// <param name="title">The list title.</param>
        /// <param name="list">The list of view models.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        public ContentListPageViewModel(string title, IEnumerable<ViewModel<IItem>> list, ICommand selectItemCommand)
        {
            Title = title;
            SelectItemCommand = selectItemCommand;
            List = list;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentListPageViewModel"/> class.
        /// </summary>
        /// <param name="title">The list title.</param>
        /// <param name="func">The function responsible for getting the task responsible for resolving to the list of content.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        public ContentListPageViewModel(string title, Func<Task<IEnumerable<IItem>>> func, ICommand selectItemCommand)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            Title = title;
            SelectItemCommand = selectItemCommand;
            List = new RefreshableBaseCollectionViewModel<IItem, TeaserViewModel>(func);
            Refresh();
        }

        /// <summary>
        /// Gets the empty label.
        /// </summary>
        /// <value>The empty label.</value>
        public string EmptyLabel => Strings.DefaultEmptyMessage;

        /// <summary>
        /// Gets the list of content.
        /// </summary>
        /// <value>The content.</value>
        public IEnumerable<ViewModel<IItem>> List { get; }

        /// <summary>
        /// Gets the command to invoke when an item is selected.
        /// </summary>
        /// <value>The select item command.</value>
        public ICommand SelectItemCommand { get; }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }

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

                SetField(ref selectedItem, value as ISelectable);

                if (selectedItem != null)
                {
                    selectedItem.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Refresh this instance.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();

            if (loadingDataTask != null)
            {
                return;
            }

            if (List is IRefreshableViewModel refreshableViewModel)
            {
                IsLoading = List.Any() == false;
                loadingDataTask = refreshableViewModel.Refresh()
                    .ContinueWith(
                        (task) =>
                        {
                            IsLoading = false;
                            loadingDataTask = null;
                            Error = task.Exception;

                            OnPropertyChanged(nameof(EmptyLabel));
                        },
                        CancellationToken.None,
                        TaskContinuationOptions.AttachedToParent,
                        TaskScheduler.Current);
            }
        }
    }
}
