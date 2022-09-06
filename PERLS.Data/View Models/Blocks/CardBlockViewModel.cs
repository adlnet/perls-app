using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PERLS.Data.Definition;
using PERLS.Data.Factories;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels.Blocks
{
    /// <summary>
    /// A view model for a block of cards.
    /// </summary>
    public class CardBlockViewModel : BlockViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardBlockViewModel"/> class.
        /// </summary>
        /// <param name="block">The block to represent.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        public CardBlockViewModel(IBlock block, ICommand selectItemCommand) : base(block)
        {
            Contents = new CardBlockViewModelCollection(GetContents);
            OnSelectionChanged = selectItemCommand ?? throw new ArgumentNullException(nameof(selectItemCommand));
        }

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public TeaserViewModel SelectedItem { get; set; }

        /// <summary>
        /// Gets the contents to display in this block.
        /// </summary>
        /// <value>The contents of this block.</value>
        public RefreshableClearlessBaseCollectionViewModel<IItem, CardViewModel> Contents { get; }

        /// <summary>
        /// Gets a value indicating whether this has more items.
        /// </summary>
        /// <value>
        /// A value indicating whether this has more items.
        /// </value>
        public new bool HasMore => Model.More != null && (Contents.Count == 0 || Contents.Count >= 10);

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
                }, TaskScheduler.Default).ConfigureAwait(false);
        }

        async Task<IEnumerable<IItem>> GetContents()
        {
            return await Model.GetContentsTask().ConfigureAwait(false) as IEnumerable<IItem>;
        }

        class CardBlockViewModelCollection : RefreshableClearlessBaseCollectionViewModel<IItem, CardViewModel>
        {
            readonly INavigationCommandProvider navProvider = DependencyService.Get<INavigationCommandProvider>();

            internal CardBlockViewModelCollection(Func<Task<IEnumerable<IItem>>> modelCollectionTaskAction) : base(modelCollectionTaskAction)
            {
            }

            protected override CardViewModel ConvertModelToViewModel(IItem model)
            {
                return ItemToViewModelFactory.CreateViewModelFromItem(model, navProvider.DownloadRequested);
            }
        }
    }
}
