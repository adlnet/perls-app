using System.Linq;
using System.Windows.Input;
using PERLS.Data.Definition;

namespace PERLS.Data.ViewModels.Sections
{
    /// <summary>
    /// A view model for a single column of blocks.
    /// </summary>
    public class OneColumnSectionViewModel : SectionViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneColumnSectionViewModel"/> class.
        /// </summary>
        /// <param name="model">The model to represent.</param>
        /// <param name="selectItemCommand">The command to invoke when an item is selected.</param>
        public OneColumnSectionViewModel(ISection model, ICommand selectItemCommand) : base(model, selectItemCommand)
        {
        }

        /// <summary>
        /// Gets the name of this section.
        /// </summary>
        /// <value>The name of the first block (with a valid name).</value>
        public string Name => Model.Blocks.FirstOrDefault(block => !string.IsNullOrWhiteSpace(block.Name))?.Name;
    }
}
