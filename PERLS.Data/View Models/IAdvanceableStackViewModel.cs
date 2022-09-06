using System;
using System.Windows.Input;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A stackable view model.
    /// </summary>
    public interface IAdvanceableStackViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether this is currently existing in a stackable.
        /// </summary>
        /// <value>
        /// If this is currently existing in a stackable.
        /// </value>
        bool IsStacked { get; set; }

        /// <summary>
        /// Gets or sets the page index.
        /// </summary>
        /// <value>
        /// The page index.
        /// </value>
        int? StackedPositionIndex { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        /// <value>
        /// The total number of pages.
        /// </value>
        int? TotalStackedCount { get; set; }

        /// <summary>
        /// Gets or sets the command called when the stack should go to the next card.
        /// </summary>
        /// <value>The command for going to the next card.</value>
        ICommand AdvanceStackCommand { get; set; }
    }
}
