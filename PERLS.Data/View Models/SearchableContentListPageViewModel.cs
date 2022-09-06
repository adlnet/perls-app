using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Commands;
using Float.Core.Notifications;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A page containing a list of content which can be filtered.
    /// </summary>
    /// <remarks>TODO: This should (probably) inherit from ContentListPageViewModel.</remarks>
    public class SearchableContentListPageViewModel : BasePageViewModel, IVariableItemViewModel
    {
        readonly ICommand filterCommand;
        readonly string noContentLabel;
        ISelectable selectedItem;
        string filter = string.Empty;
        IEnumerable<TeaserViewModel> filterList;
        Task loadingDataTask;
        string emptyImageName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableContentListPageViewModel"/> class.
        /// </summary>
        /// <param name="title">The list title.</param>
        /// <param name="func">The function responsible for getting the task responsible for resolving to the list of content.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="emptyLabel">The string containing the label should the collection returned be empty.</param>
        /// <param name="emptyImageName">The string the name of the image to display should the collection be empty.</param>
        public SearchableContentListPageViewModel(string title, Func<Task<IEnumerable<IItem>>> func, ICommand selectItemCommand, IAsyncCommand<IItem> downloadContentCommand, string emptyLabel, string emptyImageName = "")
        {
            Title = title;
            noContentLabel = emptyLabel;
            SelectItemCommand = selectItemCommand;
            this.emptyImageName = emptyImageName;
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            ContentList = new TeaserViewCollectionViewModel(func, downloadContentCommand);

            Refresh();

            var command = new Command(PerformFilter);
            filterCommand = new ThrottleCommand(command, 800);
            IsLoadingNewPage = false;
            LoadMoreAsyncCommand = new AsyncCommand(LoadMore);
        }

        /// <summary>
        /// Gets or sets a command to load more items.
        /// </summary>
        /// <value>Returns a command to load more items.</value>
        public AsyncCommand LoadMoreAsyncCommand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page is loading new items or not.
        /// </summary>
        /// <value>True if loading, else false.</value>
        public bool IsLoadingNewPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page should load new content anymore.
        /// </summary>
        /// <value>True if more content is available, else false.</value>
        public bool IsEndOfContent { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a query.
        /// </summary>
        /// <value><c>true</c> if there is a query, <c>false</c> otherwise.</value>
        public bool IsFilter => !string.IsNullOrWhiteSpace(Filter);

        /// <inheritdoc/>
        public string EmptyImageName => Error == null ? emptyImageName : "error";

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

        /// <summary>
        /// Gets the empty label.
        /// </summary>
        /// <value>The empty label.</value>
        public string EmptyLabel
        {
            get
            {
                if (IsLoading)
                {
                    return string.Empty;
                }

                if (Error != null)
                {
                    return DependencyService.Get<INotificationHandler>()?.FormatException(Error);
                }

                if (ContentList.Any() && IsFilter)
                {
                    return Strings.NoResultsMessage;
                }

                return noContentLabel;
            }
        }

        /// <summary>
        /// Gets or sets the list of content.
        /// </summary>
        /// <value>The content list.</value>
        public IEnumerable<TeaserViewModel> ContentList { get; set; }

        /// <summary>
        /// Gets the command to invoke when an item is selected.
        /// </summary>
        /// <value>The selected command.</value>
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
                if (value is ISelectable selectable)
                {
                    if (selectedItem != null && selectedItem != value)
                    {
                        selectedItem.IsSelected = false;
                    }

                    selectedItem = selectable;
                    selectedItem.IsSelected = true;
                }
                else
                {
                    if (selectedItem != null)
                    {
                        selectedItem.IsSelected = false;
                    }

                    selectedItem = null;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter query.
        /// </summary>
        /// <value>The filter query.</value>
        public string Filter
        {
            get => filter;
            set
            {
                filter = value;
                if (filterCommand.CanExecute(value))
                {
                    filterCommand.Execute(value);
                }
            }
        }

        /// <summary>
        /// Gets the list of items to display as the filter results.
        /// </summary>
        /// <value>The results elements.</value>
        public IEnumerable Elements
        {
            get => string.IsNullOrWhiteSpace(Filter) ? ContentList : filterList;
        }

        /// <summary>
        /// Gets the sizing strategy for the collection view.
        /// </summary>
        /// <value>The sizing strategy.</value>
        public ItemSizingStrategy ElementSizingStrategy => ItemSizingStrategy.MeasureAllItems;

        /// <inheritdoc />
        public override void Refresh()
        {
            base.Refresh();
            if (loadingDataTask != null || ContentList.Any())
            {
                return;
            }

            if (ContentList is IRefreshableViewModel refreshableViewModel)
            {
                IsLoading = ContentList.Any() == false || ContainsCachedData;
                loadingDataTask = refreshableViewModel.Refresh()
                    .ContinueWith(
                        (task) =>
                        {
                            ContainsCachedData = refreshableViewModel.IsCacheDerived;
                            IsLoading = false;
                            loadingDataTask = null;
                            Error = task.Exception;
                            if (Error != null)
                            {
                                IsEndOfContent = true;
                            }

                            OnContentLoaded();
                        },
                        CancellationToken.None,
                        TaskContinuationOptions.AttachedToParent,
                        TaskScheduler.Current);
            }
        }

        /// <summary>
        /// Invoked when the content has finished loading or has failed to load.
        /// </summary>
        /// <remarks>The content can be accessed from <see cref="ContentList"/> or the error from <see cref="BasePageViewModel.Error"/>.</remarks>
        protected virtual void OnContentLoaded()
        {
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(Filter))
            {
                OnPropertyChanged(nameof(IsFilter));
            }
            else if (propertyName == nameof(IsFilter))
            {
                OnPropertyChanged(nameof(Elements));
                OnPropertyChanged(nameof(ElementSizingStrategy));
                if (!IsFilter)
                {
                    OnPropertyChanged(nameof(EmptyLabel));
                }
            }
        }

        /// <summary>
        /// Performs a filter query.
        /// </summary>
        /// <param name="arg">The string for the filter query.</param>
        void PerformFilter(object arg)
        {
            if (arg is string filterQuery)
            {
                filterList = ContentList.Where(a => a.Name.ToUpperInvariant().Contains(filterQuery.ToUpperInvariant()));
            }

            OnPropertyChanged(nameof(Elements));
            OnPropertyChanged(nameof(EmptyLabel));
        }

        /// <summary>
        /// Method to load more content.
        /// </summary>
        /// <returns>A task to load more content.</returns>
        async Task LoadMore()
        {
            var content = ContentList as TeaserViewCollectionViewModel;
            if (content?.PagedResponse == null)
            {
                IsEndOfContent = true;
                IsLoadingNewPage = false;
                OnPropertyChanged(nameof(IsLoadingNewPage));
                return;
            }

            OnPropertyChanged(nameof(IsLoadingNewPage));
            IEnumerable<TeaserViewModel> response;

            try
            {
                response = await content.LoadMore().ConfigureAwait(false);
                IsEndOfContent = content.PagedResponse.IsLastPage;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Error = e;
                IsEndOfContent = true;

                IsLoadingNewPage = false;
                OnPropertyChanged(nameof(IsLoadingNewPage));
                return;
            }

            await Device.InvokeOnMainThreadAsync(() =>
            {
                foreach (var item in response)
                {
                    content.Add(item);
                }

                // This block is for an issue with updating Enumerables. I was unable to find a way
                // to add elements and update the UI with the new elements. Instead, I clear the current
                // Enumerable and re-add it, which updates the UI with the new elements.
                var temp = ContentList;

                ContentList = null;
                OnPropertyChanged(nameof(ContentList));
                OnPropertyChanged(nameof(Elements));

                ContentList = temp;
                OnPropertyChanged(nameof(ContentList));
                OnPropertyChanged(nameof(Elements));

                IsLoadingNewPage = false;
                OnPropertyChanged(nameof(IsLoadingNewPage));
            }).ConfigureAwait(false);
        }
    }
}
