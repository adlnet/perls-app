using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.Data.ViewModels.Blocks
{
    /// <summary>
    /// A view model for a collection of blocks.
    /// </summary>
    public class BlockCollectionViewModel : RefreshableClearlessBaseCollectionViewModel<IBlock, BlockViewModel>, IEmptyCollectionViewModel
    {
        readonly ICommand selectItemCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCollectionViewModel"/> class.
        /// </summary>
        /// <param name="modelCollection">The blocks to represent.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        public BlockCollectionViewModel(IEnumerable<IBlock> modelCollection, ICommand selectItemCommand) : base(() => Task.FromResult(modelCollection))
        {
            this.selectItemCommand = selectItemCommand ?? throw new ArgumentNullException(nameof(selectItemCommand));
        }

        /// <summary>
        /// Gets a value indicating whether this collection is loading.
        /// </summary>
        /// <value><c>true</c> if the collection is loading, <c>false</c> otherwise.</value>
        public bool IsLoading { get; private set; }

        /// <summary>
        /// Gets the error in this view model, if one exists.
        /// </summary>
        /// <value>The current error.</value>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not there is currently an error in this view model.
        /// </summary>
        /// <value><c>true</c> if there is an error, <c>false</c> otherwise.</value>
        public bool IsError => Error != null;

        /// <inheritdoc />
        public string EmptyLabel => Strings.DefaultEmptyMessage;

        /// <inheritdoc />
        public string EmptyMessageTitle => Strings.EmptyViewErrorTitle;

        /// <inheritdoc />
        public string EmptyImageName => IsError ? "error" : "resource://PERLS.Data.Resources.empty_placeholder.svg?Assembly=PERLS.Data";

        /// <inheritdoc />
        public override async Task Refresh()
        {
            await base.Refresh().ConfigureAwait(false);
            await Task.WhenAll(Elements.Select(block => Task.Run(block.Refresh))).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override BlockViewModel ConvertModelToViewModel(IBlock model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return model.Template switch
            {
                BlockTemplate.Chip => new ChipBlockViewModel(model, selectItemCommand),
                BlockTemplate.Tile => new TileBlockViewModel(model, selectItemCommand),
                BlockTemplate.Card => new CardBlockViewModel(model, selectItemCommand),
                BlockTemplate.Banner => new BannerBlockViewModel(model, selectItemCommand),
                _ => throw new InvalidEnumArgumentException(),
            };
        }
    }
}
