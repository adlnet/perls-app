using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels.Blocks
{
    /// <summary>
    /// A view model for blocks composed of chips.
    /// </summary>
    public class ChipBlockViewModel : BlockViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChipBlockViewModel"/> class.
        /// </summary>
        /// <param name="block">The block to represent in this view model.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        public ChipBlockViewModel(IBlock block, ICommand selectItemCommand) : base(block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            Contents = new RefreshableClearlessBaseCollectionViewModel<IRemoteResource, ChipViewModel>(block.GetContentsTask);
            OnSelectionChanged = selectItemCommand ?? throw new ArgumentNullException(nameof(selectItemCommand));
        }

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public ChipViewModel SelectedItem { get; set; }

        /// <summary>
        /// Gets the contents to display in this block.
        /// </summary>
        /// <value>The contents of this block.</value>
        public RefreshableClearlessBaseCollectionViewModel<IRemoteResource, ChipViewModel> Contents { get; }

        /// <summary>
        /// Gets the empty image name, null to avoid issues with limited space.
        /// </summary>
        /// <value>
        /// The empty image name, null to avoid issues with limited space.
        /// </value>
        public new string EmptyImageName => null;

        /// <summary>
        /// Gets the empty message title, null to avoid issues with limited space.
        /// </summary>
        /// <value>
        /// The empty message title, null to avoid issues with limited space.
        /// </value>
        public new string EmptyMessageTitle => null;

        /// <summary>
        /// Gets the empty label, changed to what would otherwise be the typical title.
        /// </summary>
        /// <value>
        /// The empty label, changed to what would otherwise be the typical title.
        /// </value>
        public new string EmptyLabel => Strings.EmptyViewErrorTitle;

        /// <summary>
        /// Gets a value indicating whether the contents are currently empty.
        /// </summary>
        /// <value>A value indicating whether the contents are empty.</value>
        public bool IsEmpty => !Contents.Any();

        /// <summary>
        /// Gets a value indicating whether this has more items.
        /// </summary>
        /// <value>
        /// A value indicating whether this has more items.
        /// </value>
        public new bool HasMore => Model.More != null && !IsLoading && (Contents.Count == 0 || Contents.Count >= 10);

        /// <summary>
        /// Gets the contents to display.
        /// </summary>
        /// <value>
        /// The contents to display.
        /// </value>
        public IEnumerable<ChipViewModel> FirstContents => Contents.Take(10);

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
                    OnPropertyChanged(nameof(IsEmpty));
                    OnPropertyChanged(nameof(FirstContents));
                }, TaskScheduler.Default).ConfigureAwait(false);
        }
    }
}
