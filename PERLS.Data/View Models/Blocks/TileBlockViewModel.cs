using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels.Blocks
{
    /// <summary>
    /// A view model for blocks composed of tiles.
    /// </summary>
    public class TileBlockViewModel : BlockViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileBlockViewModel"/> class.
        /// </summary>
        /// <param name="block">The block to present in this view model.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        public TileBlockViewModel(IBlock block, ICommand selectItemCommand) : base(block)
        {
            Contents = new TileBlockViewModelCollection(GetContents);
            OnSelectionChanged = selectItemCommand ?? throw new ArgumentNullException(nameof(selectItemCommand));
            ViewMoreCommand = OnSelectionChanged;
        }

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public TeaserViewModel SelectedItem { get; set; }

        /// <summary>
        /// Gets a command to handle the "view more" button.
        /// </summary>
        /// <value>The view more command.</value>
        public ICommand ViewMoreCommand { get; }

        /// <summary>
        /// Gets the contents to display in the view.
        /// </summary>
        /// <value>The contents in this block.</value>
        public RefreshableClearlessBaseCollectionViewModel<IItem, TeaserViewModel> Contents { get; }

        /// <summary>
        /// Gets a value indicating whether this has more items.
        /// </summary>
        /// <value>
        /// A value indicating whether this has more items.
        /// </value>
        public new bool HasMore => Model.More != null && !IsLoading && (Contents.Count == 0 || Contents.Count >= 10);

        /// <inheritdoc />
        public override void Refresh()
        {
            IsLoading = !Contents.Any();

            Contents.Refresh().ContinueWith(
                task =>
                {
                    Error = task.Exception;
                    IsLoading = false;
                    OnPropertyChanged(nameof(HasMore));
                }, TaskScheduler.Current).ConfigureAwait(false);
        }

        async Task<IEnumerable<IItem>> GetContents()
        {
            return await Model.GetContentsTask().ConfigureAwait(false) as IEnumerable<IItem>;
        }

        class TileBlockViewModelCollection : RefreshableClearlessBaseCollectionViewModel<IItem, TeaserViewModel>
        {
            readonly INavigationCommandProvider navProvider = DependencyService.Get<INavigationCommandProvider>();

            internal TileBlockViewModelCollection(Func<Task<IEnumerable<IItem>>> modelCollectionTaskAction) : base(modelCollectionTaskAction)
            {
            }

            protected override TeaserViewModel ConvertModelToViewModel(IItem model)
            {
                return new TeaserViewModel(model, navProvider.DownloadRequested);
            }
        }
    }
}
