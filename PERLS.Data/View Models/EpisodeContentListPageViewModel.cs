using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Float.Core.Notifications;
using PERLS.Data.Commands;
using PERLS.Data.Converters;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A list of podcast episodes.
    /// </summary>
    public class EpisodeContentListPageViewModel : BasePageViewModel, IVariableItemViewModel
    {
        readonly string noContentLabel;
        readonly string emptyImageName;
        ISelectable selectedItem;
        Task loadingDataTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodeContentListPageViewModel"/> class.
        /// </summary>
        /// <param name="title">The list title.</param>
        /// <param name="func">The function responsible for getting the task responsible for resolving to the list of content.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="emptyLabel">The string containing the label should the collection returned be empty.</param>
        /// <param name="emptyImageName">The string the name of the image to display should the collection be empty.</param>
        public EpisodeContentListPageViewModel(string title, Func<Task<IEnumerable<IItem>>> func, ICommand selectItemCommand, IAsyncCommand<IItem> downloadContentCommand, string emptyLabel, string emptyImageName = "")
        {
            Title = title;
            noContentLabel = emptyLabel;
            SelectItemCommand = selectItemCommand;
            this.emptyImageName = emptyImageName;
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            ContentList = new EpisodeViewCollectionViewModel(func, downloadContentCommand);
            Refresh();
        }

        /// <summary>
        /// Gets the alternative color.
        /// </summary>
        /// <value>The alt color.</value>
        public Color AltColor
        {
            get
            {
                var altColorConverter = new AltColorConverter();
                var color = (Color)Application.Current.Resources["PodcastCardColor"];
                return (Color)altColorConverter.Convert(color, null, null, null);
            }
        }

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

                return noContentLabel;
            }
        }

        /// <summary>
        /// Gets the list of content.
        /// </summary>
        /// <value>The content list.</value>
        public IEnumerable<EpisodeViewModel> ContentList { get; }

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
        /// Gets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public IEnumerable Elements => ContentList;

        /// <summary>
        /// Gets the sizing strategy for the collection view.
        /// </summary>
        /// <value>The sizing strategy.</value>
        public ItemSizingStrategy ElementSizingStrategy => ItemSizingStrategy.MeasureAllItems;

        /// <inheritdoc />
        public override void Refresh()
        {
            base.Refresh();
            if (loadingDataTask != null)
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

                            OnPropertyChanged(nameof(Elements));
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
    }
}
