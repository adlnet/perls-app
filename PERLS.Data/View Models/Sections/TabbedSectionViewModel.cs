using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels.Blocks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PERLS.Data.ViewModels.Sections
{
    /// <summary>
    /// A section with a series of tabs containing blocks.
    /// </summary>
    public partial class TabbedSectionViewModel : SectionViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabbedSectionViewModel"/> class.
        /// </summary>
        /// <param name="model">The model to represent.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        public TabbedSectionViewModel(ISection model, ICommand selectItemCommand) : base(model, selectItemCommand)
        {
            BlockButtons = model.Blocks.Select((block, index) => new BlockButtonViewModel(block.Name, index)).ToList();
        }

        /// <summary>
        /// Gets the names of each block in this section.
        /// </summary>
        /// <value>The block names.</value>
        public IEnumerable<BlockButtonViewModel> BlockButtons { get; }

        /// <summary>
        /// Gets or sets the selected block.
        /// </summary>
        /// <value>
        /// The selected block.
        /// </value>
        public BlockButtonViewModel SelectedBlock { get; set; }

        /// <summary>
        /// Gets or sets the currently selected index of the block to show.
        /// </summary>
        /// <value>The selected block index.</value>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Gets the current block.
        /// </summary>
        BlockViewModel CurrentBlock => Blocks.Count > SelectedIndex ? Blocks.ElementAt(SelectedIndex) : null;

        /// <inheritdoc />
        public override void Refresh()
        {
            base.Refresh();

            // if no blocks are visible, make the first block visible
            if (!Blocks.Where(block => block.IsVisible).Any() && Blocks.FirstOrDefault() is BlockViewModel firstBlock)
            {
                firstBlock.IsVisible = true;
            }
        }

        /// <summary>
        /// Switches to the block button view model.
        /// </summary>
        /// <param name="blockButtonViewModel">The view model.</param>
        public void SwitchToBlock(BlockButtonViewModel blockButtonViewModel)
        {
            if (CurrentBlock is BlockViewModel previousBlock)
            {
                previousBlock.IsVisible = false;
            }

            SelectedIndex = BlockButtons.IndexOf(blockButtonViewModel);
            OnPropertyChanged(nameof(SelectedIndex));

            if (CurrentBlock is BlockViewModel currentBlock)
            {
                currentBlock.IsVisible = true;
            }
        }
    }
}
