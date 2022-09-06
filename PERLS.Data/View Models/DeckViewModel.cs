using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Float.Core.Notifications;
using PERLS.Data.Commands;
using PERLS.Data.Definition;
using PERLS.Data.Factories;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A deck of cards.
    /// </summary>
    public class DeckViewModel : RefreshableBaseCollectionViewModel<IItem, CardViewModel>, IVariableItemViewModel<CardViewModel>
    {
        readonly IAsyncCommand<IItem> downloadContentCommand;

        bool isLoading;
        string emptyMessage;
        string emptyImage;
        string emptyTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckViewModel"/> class.
        /// </summary>
        /// <param name="items">The items in the deck.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="emptyMessage">The message to present when there is no content.</param>
        /// <param name="emptyTitle">The title to present when there is no content.</param>
        /// <param name="emptyImage">The image to present when there is no content.</param>
        public DeckViewModel(IEnumerable<IItem> items, IAsyncCommand<IItem> downloadContentCommand, string emptyMessage = null, string emptyTitle = null, string emptyImage = null) : this(StaticDeck(items), downloadContentCommand, emptyMessage, emptyTitle, emptyImage)
        {
            this.downloadContentCommand = downloadContentCommand;
            Refresh();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckViewModel"/> class.
        /// </summary>
        /// <param name="modelCollectionTaskFunc">The function responsible for returning the task responsible for resolving to the model collection.</param>
        /// <param name="downloadContentCommand">A command to invoke to download content locally.</param>
        /// <param name="emptyMessage">The message to present when there is no content.</param>
        /// <param name="emptyTitle">The title to present when there is no content.</param>
        /// <param name="emptyImage">The image to present when there is no content.</param>
        public DeckViewModel(Func<Task<IEnumerable<IItem>>> modelCollectionTaskFunc, IAsyncCommand<IItem> downloadContentCommand, string emptyMessage = null, string emptyTitle = null, string emptyImage = null) : base(modelCollectionTaskFunc)
        {
            this.downloadContentCommand = downloadContentCommand;
            this.emptyMessage = emptyMessage ?? Strings.DefaultEmptyMessage;
            this.emptyTitle = emptyTitle;
            this.emptyImage = emptyImage;
        }

        /// <summary>
        /// Gets the error encountered when loading content.
        /// </summary>
        /// <value>The error if one was encountered while loading content.</value>
        public Exception Error { get; private set; }

        /// <inheritdoc />
        public string EmptyLabel
        {
            get
            {
                if (Error != null)
                {
                    return DependencyService.Get<INotificationHandler>().FormatException(Error);
                }

                if (isLoading)
                {
                    // Hiding the empty label while it is loading content should
                    // be handled by the view layer. To the next developer:
                    // Avoid making additional changes to the logic here and
                    // investigate enhancing the view layer to hide the empty message
                    // if the view is actively loading.
                    return string.Empty;
                }

                return emptyMessage;
            }
            set => SetField(ref emptyMessage, value);
        }

        /// <inheritdoc />
        public string EmptyImageName
        {
            get => Error == null ? isLoading ? string.Empty : emptyImage : "error";
            set => SetField(ref emptyImage, value);
        }

        /// <inheritdoc />
        public string EmptyMessageTitle
        {
            get => Error == null ? isLoading ? string.Empty : emptyTitle : Strings.EmptyViewErrorTitle;
            set => SetField(ref emptyTitle, value);
        }

        /// <inheritdoc />
        public override Task Refresh()
        {
            Error = null;
            isLoading = true;

            return base.Refresh()
                .ContinueWith(
                task =>
                {
                    isLoading = false;
                    Error = task.Exception;
                    OnPropertyChanged(nameof(EmptyLabel));
                    OnPropertyChanged(nameof(EmptyMessageTitle));
                    OnPropertyChanged(nameof(EmptyImageName));
                }, TaskScheduler.Current);
        }

        /// <inheritdoc />
        protected override CardViewModel ConvertModelToViewModel(IItem model)
        {
            return ItemToViewModelFactory.CreateViewModelFromItem(model, downloadContentCommand);
        }

        static Func<Task<IEnumerable<IItem>>> StaticDeck(IEnumerable<IItem> items)
        {
            return () =>
            {
                return Task.FromResult(items);
            };
        }
    }
}
